using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Authentication;
using System.Text;
using System.Threading;
using System.Xml.Serialization;
using CookComputing.XmlRpc;
using Ionic.Zlib;
using MoreLinq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using log4net;

namespace subdown.Providers.OpenSubtitles
{
    public class OpenSubtitlesProvider : ISubtitleProvider
    {
        private ILog Log;

        public static int HttpStatusMessageOK = 200;

        public static Dictionary<int, string> HttpStatusMessages = new Dictionary<int, string> { { 301, "Moved (host)" }, { 200, "OK" }, { 206, "Partial content; message" }, { 503, "Service Unavailable" }, { 401, "Unauthorized" }, { 402, "Subtitles has invalid format" }, { 403, "SubHashes (content and sent subhash) are not same!" }, { 404, "Subtitles has invalid language!" }, { 405, "Not all mandatory parameters was specified" }, { 406, "No session" }, { 407, "Download limit reached" }, { 408, "Invalid parameters" }, { 409, "Method not found" }, { 410, "Other or unknown error" }, { 411, "Empty or invalid useragent" }, { 412, "%s has invalid format (reason)" }, { 413, "Invalid ImdbID" }, { 414, "Unknown User Agent" }, { 415, "Disabled user agent" } };

        public const string APIUrl = "http://api.opensubtitles.org/xml-rpc";

        public const string TestUserAgent = "OS Test User Agent - sample";

        public readonly IOpenSubstitlesProxy Proxy;

        // on first remote call check and set token
        // create a timer that will call remote procedure NoOperation on token every 15 minutes
        // in order to reset token expiration
        private string _token;


        public OpenSubtitlesProvider()
        {
            Log = LogManager.GetLogger(typeof(OpenSubtitlesProvider));
            Proxy = XmlRpcProxyGen.Create<IOpenSubstitlesProxy>();
            Proxy.UserAgent = TestUserAgent;


            //Logger.WriteLine("LogIn: ");

            ////XmlRpcUtils.PrintStruct(LogIn("", "", "en", TestUserAgent));

            //Logger.WriteLine("\r\nNoOperation: ");
            ////XmlRpcUtils.PrintStruct(Proxy.NoOperation(Token));

            //Logger.WriteLine("Initialize connection refresh timer");
            ////Timer timer = new Timer(ResetTokenExpiration, null, 0, 15);
        }

        private XmlRpcStruct LogIn(string username = "", string password = "", string language = "en", string userAgent = TestUserAgent)
        {
            if (Proxy != null)
            {
                Log.DebugFormat("Login with credentials {0}:{1} lang={2}, ua={3}", username, password, language, userAgent);
                XmlRpcStruct rpcStruct = Proxy.LogIn(username, password, language, userAgent);
                var status = XmlRpcConverter.GetStatus(rpcStruct);
                if (XmlRpcConverter.StatusOk(rpcStruct))
                {
                    if (rpcStruct.ContainsKey("token"))
                    {
                        _token = (string)rpcStruct["token"];
                        Log.DebugFormat("Login successfull! Token = {0}", _token);
                        return rpcStruct;
                    }
                    else
                    {
                        Log.ErrorFormat("Login failed for user {0}. No token received!", username);
                    }
                }
                else
                {
                    Log.ErrorFormat("Login failed for user {0}. Reason: {1}", username, status);
                }

            }
            return null;
        }


        public void SetLogger(TextWriter logger = null)
        {
            //Log = logger;
        }

        /// <exception cref="Exception"></exception>
        public IEnumerable<ISubtitleResult> SearchSubtitles(IEnumerable<FileInfo> filenames, string language)
        {
            var subtitlesResponse = GetSubtitleDetails(filenames, language);
            var subtitles = subtitlesResponse.Values.SelectMany(x => x);

            return subtitles;
        }

        /// <exception cref="Exception"></exception>
        protected Dictionary<OSPMovieDetails, List<OSPSubtitle>> GetSubtitleDetails(IEnumerable<FileInfo> fileinfos, string language)
        {
            Authenticate();
            var filenames = fileinfos.ToList();

            var details = CheckMovieHashes(filenames);

            var searchOptions = new List<OSPSubtitleSearchOption>();

            searchOptions.AddRange(details.Select(detail => new OSPSubtitleSearchOption
                                                                {
                                                                    episode = detail.SeriesEpisode,
                                                                    imdbid = detail.MovieImdbID,
                                                                    season = detail.SeriesSeason,
                                                                    sublanguageid = language
                                                                }));
            Log.Debug("Added search options {episode, imdbid, season, sublanguageid}");
            foreach (FileInfo filename in filenames)
            {
                var movieHash = Utils.ComputeHash(filename.FullName);
                long movieByteSize;
                using (var s = File.OpenRead(filename.FullName)) { movieByteSize = s.Length; }

                searchOptions.Add(new OSPSubtitleSearchOption { moviehash = movieHash, moviebytesize = movieByteSize, sublanguageid = language });
                searchOptions.Add(new OSPSubtitleSearchOption { tag = filename.FullName, sublanguageid = language });
            }
            var searchRequest = searchOptions.ToArray();

            Log.DebugFormat("Searching for subtitles. Total searchOptions {0}", searchRequest.Length);
            XmlRpcStruct response;
            try
            {
                response = Proxy.SearchSubtitles(_token, searchRequest);
            }
            catch (Exception exception)
            {
                Log.Error("SearchSubtitles proxy error", exception);
                throw;
            }
            try
            {
                var subtitlesResponse = XmlRpcConverter.GetSubtitlesResponse(response, details);
                return subtitlesResponse;
            }
            catch (Exception exception)
            {
                Log.Error("SearchSubtitleResponse conversion error", exception);
                throw;
            }
        }

        public List<OSPSubtitle> GetSubField(IEnumerable<OSPSubtitle> subs, string property, object value)
        {
            var fi = typeof(OSPSubtitle).GetField(property);

            return fi != null ? subs.Where(ospSubtitle => fi.GetValue(ospSubtitle).Equals(value)).ToList() : null;
        }

        public List<OSPSubtitle> GetSubProp(IEnumerable<OSPSubtitle> subs, string property, object value)
        {
            var fi = typeof(OSPSubtitle).GetProperty(property);
            return fi != null ? subs.Where(x => fi.GetValue(x, null).Equals(value)).ToList() : null;
        }

        public static Dictionary<string, int> SubtitlesPerFile = new Dictionary<string, int>();

        private void HandleException(Exception exception)
        {
            Log.Error("Some", exception);
        }

        public int ErrorCount = 0;

        /// <summary>
        /// Stateful method, depends on this.Token
        /// </summary>
        /// <param name="options"> </param>
        /// <param name="subtitles"></param>
        /// <param name="saveSubtitleDelegate"> </param>
        /// <returns></returns>
        private IEnumerable<ISubtitleResult> DownloadSubtitleChunks(SubtitleDownloaderOptions options, List<OSPSubtitle> subtitles, SaveSubtitleDelegate saveSubtitleDelegate)
        {
            if (!options.ForceDirectDownload)
            {
                var subtitleIDs = (subtitles.Where(searchsub => !String.IsNullOrWhiteSpace(searchsub.IDSubtitleFile)).Select(searchsub => searchsub.IDSubtitleFile)).ToArray();
                XmlRpcStruct downloadSubtitlesResponse = null;
                try
                {
                    downloadSubtitlesResponse = Proxy.DownloadSubtitles(_token, subtitleIDs);
                }
                catch (Exception exception)
                {
                    Log.Warn("Proxy subtitle download failed:", exception);
                    downloadSubtitlesResponse = null;
                }

                if (downloadSubtitlesResponse != null && XmlRpcConverter.StatusOk(downloadSubtitlesResponse))
                {
                    ParseAndSaveSubtitles(downloadSubtitlesResponse, options, subtitles, saveSubtitleDelegate);
                }
                else
                {
                    Log.Warn("Failed Proxy API download, attempting direct download");
                    DownloadThroughHTTP(subtitles, options, saveSubtitleDelegate);
                }
            }
            else
            {
                DownloadThroughHTTP(subtitles, options, saveSubtitleDelegate);
            }
            return subtitles;
        }


        private void ParseAndSaveSubtitles(XmlRpcStruct downloadSubtitlesResponse, SubtitleDownloaderOptions options, List<OSPSubtitle> subtitles, SaveSubtitleDelegate saveSubtitleDelegate)
        {
            var data = downloadSubtitlesResponse["data"] as object[];
            if (data == null) return;
            var rpc = data.OfType<XmlRpcStruct>();
            var xmlRpcStructs = rpc as List<XmlRpcStruct> ?? rpc.ToList();
            foreach (var subtitleDownload in xmlRpcStructs)
            {
                try
                {
                    var idSubtitleFile = Convert.ToString(subtitleDownload[RpcTag.IDSubtitleFile]);
                    var sub = subtitles.FirstOrDefault(x => x.IDSubtitleFile == idSubtitleFile);
                    if (sub != null)
                    {
                        sub.SubtitleEncoded = Convert.ToString(subtitleDownload[RpcTag.Data]);
                        sub.SubtitleDecoded = Utils.Decompress(sub.SubtitleEncoded);
                        saveSubtitleDelegate(options, new List<ISubtitleResult> { sub }, sub.VideoFileInfo.FullName);

                    }
                }
                catch (Exception ex)
                {
                    Log.Warn("subtitle response conversion failed", ex);
                }
            }
        }

        private void DownloadThroughHTTP(IEnumerable<OSPSubtitle> subtitles, SubtitleDownloaderOptions options, SaveSubtitleDelegate saveSubtitleDelegate)
        {
            // TODO: the SubtitleProvider shouldn't download 

            // download the gz files directly from links
            // TODO: download in memory, using HttpWebRequest & HttpWebResponse
            foreach (var subtitle in subtitles)
            {
                try
                {
                    var webClient = new WebClient();
                    Log.DebugFormat("Downloading subtitle {0} from {1}", subtitle.SubtitleFileName, subtitle.SubDownloadLink);

                    webClient.DownloadFile(subtitle.SubDownloadLink, "temp.gz");
                    Thread.Sleep(options.TimeToWaitBetweenSubtitles);
                    using (var g = new StreamReader(new Ionic.Zlib.GZipStream(new FileStream("temp.gz", FileMode.Open), CompressionMode.Decompress)))
                    {
                        subtitle.SubtitleDecoded = g.ReadToEnd();
                        saveSubtitleDelegate(options, new List<ISubtitleResult> { subtitle }, subtitle.VideoFileInfo.FullName);
                    }
                    // TODO: where to call external save subtitles to disk ?
                }
                catch (Exception exception)
                {
                    Log.Warn("Direct subtitle download failed", exception);
                }
            }


        }

        public IEnumerable<ISubtitleResult> DownloadSubtitles(IEnumerable<FileInfo> filenames, SubtitleDownloaderOptions options, SaveSubtitleDelegate saveSubtitle)
        {
            Authenticate();

            Dictionary<OSPMovieDetails, List<OSPSubtitle>> subtitles = GetSubtitleDetails(filenames, options.Language);
            Log.DebugFormat("Trimming subtitles per file to {0}", options.MaximumSubtitlesPerFile);
            // some trimming
            subtitles = subtitles.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Take(options.MaximumSubtitlesPerFile).ToList());

            foreach (var kvp in subtitles)
            {
                var localKvp = kvp;
                foreach (var sub in kvp.Value.Where(sub => !String.IsNullOrWhiteSpace(localKvp.Key.VideoFilename)))
                {
                    sub.VideoFileInfo = new FileInfo(kvp.Key.VideoFilename);
                }
            }

            // filter results

            var flattenedSubtitles = subtitles.SelectMany(x => x.Value).ToList();

            // TODO: string distance could be used to see how much the subtitle name and video file name match
            // this is not actually that much of a good idea ... 
            //flattenedSubtitles.ForEach(x => x.NameFit = LevenshteinDistance.Compute(x.SubtitleFileName, x.VideoFileInfo.Name));
            //flattenedSubtitles = flattenedSubtitles.OrderByDescending(x => x.NameFit).ToList();

            var results = new List<ISubtitleResult>();
            var chunkSize = options.DownloadChunkSize;
            // a pseudo progress reporting can be manufactured here ...

            for (int i = 0; i < flattenedSubtitles.Count; i += chunkSize)
            {
                try
                {

                    var subtitlesChunk = flattenedSubtitles.Skip(i).Take(chunkSize).ToList();
                    var subtitleResults = DownloadSubtitleChunks(options, subtitlesChunk, saveSubtitle);
                    if (subtitleResults != null)
                    {
                        results.AddRange(subtitleResults);
                    }
                    Thread.Sleep(options.TimeToWaitBetweenChunks);
                    Log.InfoFormat("Downloaded {0} subtitle files", i);
                }
                catch (Exception ex)
                {
                    HandleException(ex);
                }
            }
            return flattenedSubtitles;
        }

        public string ProviderName { get { return "osp"; } }
        public bool SupportsDirectDownload { get { return true; } }

        public static void ResetTokenExpiration(object state)
        {
            var s = state as OpenSubtitlesProvider;
            if (s != null)
            {
                s.Proxy.NoOperation(s._token);
            }
        }

        /*
         work with default nulls in this order only if all parameters are nulled by default
         */
        private bool IsAuthenticated { get; set; }

        private readonly TimeSpan TokenExpirationPeriod = new TimeSpan(0, 0, 0, 60);

        private readonly DateTime TokenActivationDate = DateTime.MinValue;
        private string Username = "";
        private string Password = "";
        private string Language = "";

        private void Authenticate()
        {
            DoAuthenticate(Username, Password, Language, TestUserAgent);
        }

        private void DoAuthenticate(string username = null, string password = null, string language = null, string userAgent = null)
        {
            if (IsAuthenticated) return;
            if (String.IsNullOrEmpty(userAgent))
            {
                userAgent = TestUserAgent;
            }

            bool tokenExpired = (DateTime.Now - TokenActivationDate) > TokenExpirationPeriod;

            if (String.IsNullOrEmpty(_token) || tokenExpired)
            {
                XmlRpcStruct xmlRpcStruct = LogIn(username, password, language, userAgent);
                if (xmlRpcStruct == null)
                {
                    throw new AuthenticationException("Can't log in");
                }
                _token = xmlRpcStruct[RpcTag.Token] as string;
            }
            IsAuthenticated = !String.IsNullOrWhiteSpace(_token);
        }

        private List<OSPMovieDetails> CheckMovieHashes(IEnumerable<FileInfo> fileinfos)
        {
            Authenticate();
            var filenames = fileinfos.ToArray();
            var hashes = new string[filenames.Length];

            var hashFiles = new Dictionary<string, string>();

            for (int i = 0; i < filenames.Length; i++)
            {
                hashes[i] = Utils.ComputeHash(filenames[i].FullName);
                hashFiles.Add(hashes[i], filenames[i].FullName);
            }

            var checkMovieHashesResponse = Proxy.CheckMovieHash(_token, hashes);
            var results = XmlRpcConverter.GetMovieDetails(checkMovieHashesResponse, hashFiles);

            return results;
        }


        private static string DebugDataDir = "osp_down";

        private readonly string _downloadSubtitlesResponseFilename = "downloadSubtitlesResponseFilename.json";
        private readonly string _checkMovieHashResponseFilename = "checkMovieHashResponse.json";
        private readonly string _searchSubtitlesResponseFilename = "searchSubtitlesResponseFilename.json";

        public T DebuggingPersisted<T>(string filename, Func<T> downloadFunction) where T : class
        {
            string fullname = DebugDataDir + Path.DirectorySeparatorChar + filename;
            if (Debugger.IsAttached)
            {

                if (File.Exists(fullname))
                {

                    var fileinfo = new FileInfo(fullname);

                    using (var j = new JsonTextReader(fileinfo.OpenText()))
                    {
                        var json = JsonSerializer.Create();
                        var result = json.Deserialize(j);
                        var deserialize = result as T;
                        return deserialize;
                    }
                }
            }
            var xml = downloadFunction();
            if (Debugger.IsAttached)
            {
                Directory.CreateDirectory(DebugDataDir);
                using (var j = new JsonTextWriter(new StreamWriter(fullname)))
                {
                    var settings = JsonConvert.DefaultSettings();
                    settings.Formatting = Formatting.Indented;
                    var json = JsonSerializer.Create(settings);
                    json.Serialize(j, xml);
                }
            }
            return xml;

        }


    }
}

