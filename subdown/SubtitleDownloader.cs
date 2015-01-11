using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using log4net;

namespace subdown
{
    public class SubtitleDownloader
    {
        private ISubtitleProvider SubtitleProvider { get; set; }
        public virtual SubtitleDownloaderOptions Options { get; set; }

        private static readonly ILog Log = LogManager.GetLogger(typeof(SubtitleDownloader));

        public SubtitleDownloader(ISubtitleProvider subtitleProvider)
        {
            SubtitleProvider = subtitleProvider;
        }

        public static readonly string[] VideoFileExtensions = new[] { "264", "3g2", "3gp", "3gp2", "3gpp", "3gpp2", "3mm", "3p2", "60d", "787", "890", "aaf", "aec", "aep", "aepx", "aet", "aetx", "ajp", "ale", "am", "amc", "amv", "amx", "anim", "aqt", "arcut", "arf", "asf", "asx", "avb", "avc", "avchd", "avd", "avi", "avp", "avs", "avs", "avv", "awlive", "axm", "bdm", "bdmv", "bdt2", "bdt3", "bik", "bin", "bix", "bmc", "bmk", "bnp", "box", "bs4", "bsf", "bvr", "byu", "camproj", "camrec", "camv", "ced", "cel", "cine", "cip", "clpi", "cmmp", "cmmtpl", "cmproj", "cmrec", "cpi", "cst", "cvc", "cx3", "d2v", "d3v", "dat", "dav", "dce", "dck", "dcr", "dcr", "ddat", "dif", "dir", "divx", "dlx", "dmb", "dmsd", "dmsd3d", "dmsm", "dmsm3d", "dmss", "dmx", "dnc", "dpa", "dpg", "dream", "dsy", "dv", "dv-avi", "dv4", "dvdmedia", "dvr", "dvr-ms", "dvx", "dxr", "dzm", "dzp", "dzt", "edl", "evo", "eye", "ezt", "f4f", "f4p", "f4v", "fbr", "fbr", "fbz", "fcp", "fcproject", "ffd", "flc", "flh", "fli", "flv", "flx", "ftc", "gcs", "gfp", "gl", "gom", "grasp", "gts", "gvi", "gvp", "h264", "hdmov", "hdv", "hkm", "ifo", "imovieproj", "imovieproject", "ircp", "irf", "ism", "ismc", "ismclip", "ismv", "iva", "ivf", "ivr", "ivs", "izz", "izzy", "jmv", "jss", "jts", "jtv", "k3g", "kdenlive", "kmv", "ktn", "lrec", "lrv", "lsf", "lsx", "m15", "m1pg", "m1v", "m21", "m21", "m2a", "m2p", "m2t", "m2ts", "m2v", "m4e", "m4u", "m4v", "m75", "mani", "meta", "mgv", "mj2", "mjp", "mjpg", "mk3d", "mkv", "mmv", "mnv", "mob", "mod", "modd", "moff", "moi", "moov", "mov", "movie", "mp21", "mp21", "mp2v", "mp4", "mp4", "infovid", "mp4v", "mpe", "mpeg", "mpeg1", "mpeg4", "mpf", "mpg", "mpg2", "mpgindex", "mpl", "mpl", "mpls", "mpsub", "mpv", "mpv2", "mqv", "msdvd", "mse", "msh", "mswmm", "mts", "mtv", "mvb", "mvc", "mvd", "mve", "mvex", "mvp", "mvp", "mvy", "mxf", "mxv", "mys", "ncor", "nsv", "nut", "nuv", "nvc", "ogm", "ogv", "ogx", "orv", "osp", "otrkey", "pac", "par", "pds", "pgi", "photoshow", "piv", "pjs", "playlist", "plproj", "pmf", "pmv", "pns", "ppj", "prel", "pro", "pro4dvd", "pro5dvd", "proqc", "prproj", "prtl", "psb", "psh", "pssd", "pva", "pvr", "pxv", "qt", "qtch", "qtindex", "qtl", "qtm", "qtz", "r3d", "rcd", "rcproject", "rdb", "rec", "rm", "rmd", "rmd", "rmp", "rms", "rmv", "rmvb", "roq", "rp", "rsx", "rts", "rts", "rum", "rv", "rvid", "rvl", "sbk", "sbt", "scc", "scm", "scm", "scn", "screenflow", "sdv", "sec", "sedprj", "seq", "sfd", "sfvidcap", "siv", "smi", "smi", "smil", "smk", "sml", "smv", "snagproj", "spl", "sqz", "ssf", "ssm", "stl", "str", "stx", "svi", "swf", "swi", "swt", "tda3mt", "tdx", "thp", "tivo", "tix", "tod", "tp", "tp0", "tpd", "tpr", "trp", "ts", "tsp", "ttxt", "tvs", "usf", "usm", "vc1", "vcpf", "vcr", "vcv", "vdo", "vdr", "vdx", "veg", "vem", "vep", "vf", "vft", "vfw", "vfz", "vgz", "vid", "video", "viewlet", "viv", "vivo", "vlab", "vob", "vp3", "vp6", "vp7", "vpj", "vro", "vs4", "vse", "vsp", "w32", "wcp", "webm", "wlmp", "wm", "wmd", "wmmp", "wmv", "wmx", "wot", "wp3", "wpl", "wtv", "wve", "wvx", "xej", "xel", "xesc", "xfl", "xlmv", "xmv", "xvid", "y4m", "yog", "yuv", "zeg", "zm1", "zm2", "zm3", "zmv" };

        /// <exception cref="FileNotFoundException"></exception>
        public static IEnumerable<FileInfo> GatherFiles(SubtitleDownloaderOptions options)
        {
            Log.Info(String.Format("Searching in {0}", options.SearchDirectory));

            var directoryInfo = new DirectoryInfo(options.SearchDirectory);

            IEnumerable<FileInfo> fileInfos = directoryInfo.EnumerateFiles("*", options.Recurse ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
            var selectedFiles = new List<FileInfo>();
            if (options.VideoExtensions != null)
            {
                selectedFiles = fileInfos.Where(fileInfo => options.VideoExtensions.Contains(fileInfo.Extension.Replace(".", ""))).ToList();
            }

            Log.Info(String.Format("Found {0} video files.", selectedFiles.Count));
            Log.Debug(selectedFiles.Aggregate("Files found:\r\n", (a, e) => a + "\r\n" + e));

            if (options.Filenames != null)
            {
                return selectedFiles.Union(options.Filenames.Select(x => new FileInfo(x)));
            }
            return selectedFiles.ToList();
        }

        public IEnumerable<ISubtitleResult> DownloadSubtitles(SubtitleDownloaderOptions options)
        {
            var filenames = GatherFiles(options).ToList();
            if (options.List)
            {
                Log.Info("Files:");
                Log.Info(filenames.Aggregate("", (s, f) => s + f.FullName + "\r\n"));
            }
            else
            {
                ISubtitleProvider osp = SubtitleProvider;
                if (filenames.Count > 0)
                {
                    Log.Info(String.Format("Using subtitle provider {0}", osp.GetType()));
                    var subtitles = osp.DownloadSubtitles(filenames, options, SaveSubtitle).ToList();
                    Log.Info(String.Format("Found {0} subtitles", subtitles.Count().ToString(CultureInfo.InvariantCulture)));
                    return subtitles;
                }
                else
                {
                    Log.Info(String.Format("No vide files found at {0}", options.SearchDirectory));
                }
            }

            return new List<ISubtitleResult>();

        }

        public IEnumerable<ISubtitleResult> SearchSubtitles(SubtitleDownloaderOptions options)
        {
            var filenames = GatherFiles(options).ToList();
            var osp = SubtitleProvider;
            if (filenames.Count > 0)
            {
                var searchSubtitles = osp.SearchSubtitles(filenames, options.Language);
                return searchSubtitles;
            }
            return new List<ISubtitleResult>();
        }

        private static readonly Dictionary<string, int> SubtitlesPerFile = new Dictionary<string, int>();

        // this should be initialized sometime ... 
        private static readonly Dictionary<string, int> FileCount = new Dictionary<string, int>();

        public static void SaveSubtitle(SubtitleDownloaderOptions options, List<ISubtitleResult> subtitles, string videoFilename)
        {
            if (!SubtitlesPerFile.ContainsKey(videoFilename))
            {
                SubtitlesPerFile.Add(videoFilename, 0);
            }
            // if subtitle count exceeds limit, we shouldn't save them to disk
            // TODO: it would be smarter not to download them at all ...
            if (SubtitlesPerFile[videoFilename] >= options.MaximumSubtitlesPerFile) return;

            SubtitlesPerFile[videoFilename]++;
            string path = "";
            if (options.DownloadType == SubtitleDownloadType.Static)
            {
                path = options.DownloadDirectory;
            }
            else if (options.DownloadType == SubtitleDownloadType.RelativeToFile)
            {
                var videofileInfo = new FileInfo(videoFilename);
                if (!String.IsNullOrWhiteSpace(videofileInfo.DirectoryName))
                {
                    // this should resolve a path relative to options.DownloadDirectory
                    path = Path.Combine(videofileInfo.DirectoryName, options.DownloadDirectory);
                }
            }
            else if (options.DownloadType == SubtitleDownloadType.RelativeToDirectory)
            {
                path = Path.Combine(options.SearchDirectory, options.DownloadDirectory);
            }
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            foreach (var subtitle in subtitles)
            {
                string filename = path + Path.DirectorySeparatorChar + subtitle.SubtitleFileName;
                if (!subtitle.SubtitleFileName.ToLower().EndsWith(subtitle.SubtitleFormat.ToLower()))
                {
                    filename += "." + subtitle.SubtitleFormat;
                }
                FileInfo fileInfo = CreateUniqueFile(filename);
                if (!String.IsNullOrWhiteSpace(fileInfo.DirectoryName))
                {
                    Directory.CreateDirectory(fileInfo.DirectoryName);
                }
                if (!String.IsNullOrWhiteSpace(subtitle.SubtitleDecoded))
                {
                    try
                    {
                        using (var sw = new StreamWriter(fileInfo.OpenWrite()) { AutoFlush = true })
                        {
                            sw.Write(subtitle.SubtitleDecoded);
                        }
                    }
                    catch (Exception exception)
                    {
                        Log.Error(String.Format("Subtitle ID:{0} -- {1}", subtitle.Id, subtitle.SubtitleFileName), exception);
                    }

                }
                else
                {
                    FaultyDecodings++;
                    Log.WarnFormat("Subtitle decoding failed for {0}", subtitle.SubtitleFileName);

                }
            }
        }

        private static FileInfo CreateUniqueFile(string filename)
        {
            return CreateUniqueFile(new FileInfo(filename));
        }

        private static FileInfo CreateUniqueFile(FileInfo file)
        {
            var newFile = new FileInfo(file.FullName);
            if (file.DirectoryName != null)
            {

                var dir = new DirectoryInfo(file.DirectoryName);
                var searchPattern = file.Name.Substring(0, file.Name.LastIndexOf(file.Extension, StringComparison.Ordinal)) + "*";
                var files = dir.EnumerateFiles(searchPattern, SearchOption.TopDirectoryOnly);

                var fileInfos = files as List<FileInfo> ?? files.ToList();
                if (fileInfos.Count > 0)
                {
                    var filename = file.Name.Substring(0, file.Name.LastIndexOf(file.Extension, System.StringComparison.Ordinal));
                    var extension = file.Extension.StartsWith(".") ? file.Extension : "." + file.Extension;
                    newFile = new FileInfo(String.Format("{0}{1}{2} ({3}){4}", file.DirectoryName, Path.DirectorySeparatorChar, filename, fileInfos.Count, extension));
                }
            }
            return newFile;

            //while (newFile.Exists)
            //{
            //    if (FileCount.ContainsKey(file.FullName))
            //    {
            //        FileCount.Add(file.FullName, 0);
            //    }
            //}

            //return newFile;
        }

        protected static FileInfo CreateUniqueFile(string path, string filename, ISubtitleResult subtitle)
        {
            var fileInfo = new FileInfo(filename);
            // generates a new file in format "name {cnt}.ext" when a file with exact filename exists
            while (fileInfo.Exists)
            {
                if (FileCount.ContainsKey(filename) == false)
                {
                    FileCount.Add(filename, 0);
                }
                FileCount[filename]++;
                filename = path + Path.DirectorySeparatorChar + subtitle.SubtitleFileName + " (" +
                           FileCount[filename] + ") " + "." + subtitle.SubtitleFormat;
                fileInfo = new FileInfo(filename);
            }
            return fileInfo;
        }

        public static int FaultyDecodings;

        public static void SaveSubtitles(IEnumerable<ISubtitleResult> subtitleResults, SubtitleDownloaderOptions options)
        {
            if (options.DownloadType == SubtitleDownloadType.RelativeToDirectory)
            {
                Directory.CreateDirectory(options.DownloadDirectory);
            }

            foreach (var subtitle in subtitleResults)
            {
                // if a file with same name exists, append a number to this name
                string filename = options.DownloadDirectory + "/" + subtitle.SubtitleFileName + "." + subtitle.SubtitleFormat;
                using (var sw = new StreamWriter(filename) { AutoFlush = true })
                {
                    sw.Write(subtitle.SubtitleDecoded);
                }
            }
        }

        /// <exception cref="FileNotFoundException"></exception>
        public static List<string> GetFilesFromDir(string dirName, IEnumerable<string> exts = null, bool recurse = false)
        {
            var directoryInfo = new DirectoryInfo(dirName);

            IEnumerable<FileInfo> fileInfos = directoryInfo.EnumerateFiles("*", recurse ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
            List<string> selectedFiles;
            if (exts == null)
            {
                selectedFiles = fileInfos.Select(file => file.FullName).ToList();
            }
            else
            {
                var fn = fileInfos.Where(fileInfo => exts.Contains(fileInfo.Extension.Replace(".", ""))).Select(fileInfo => fileInfo.FullName);
                selectedFiles = fn.ToList();
            }

            if (!selectedFiles.Any())
            {
                throw new FileNotFoundException("No files were found in directory " + dirName + (exts == null ? "" : " matching extension list"));
            }
            return selectedFiles;
        }
    }
}
