using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CookComputing.XmlRpc;
using log4net;

namespace subdown.Providers.OpenSubtitles
{
    public static class XmlRpcConverter
    {
        public static string GetStatus(XmlRpcStruct xml)
        {
            if (xml.ContainsKey(RpcTag.Status))
            {
                return xml[RpcTag.Status] as string;
            }
            return "";
        }
        public static bool StatusOk(XmlRpcStruct xml)
        {
            string status = GetStatus(xml);
            return status.Contains("200");
        }

        public class SafeDictionary<TKey, TValue> : Dictionary<TKey, TValue>
        {
            public new TValue this[TKey key]
            {
                get
                {
                    return base[key];
                }
                set
                {
                    if (!ContainsKey(key))
                    {
                        Add(key, value);
                    }
                    else
                    {
                        base[key] = value;
                    }
                }
            }
        }

        /// <exception cref="Exception"></exception>
        public static SafeDictionary<OSPMovieDetails, List<OSPSubtitle>> GetSubtitlesResponse(XmlRpcStruct searchSubtitlesResponse, List<OSPMovieDetails> details)
        {
            var movieSubtitles = new SafeDictionary<OSPMovieDetails, List<OSPSubtitle>>();


            if (StatusOk(searchSubtitlesResponse))
            {
                var data = searchSubtitlesResponse[RpcTag.Data] as object[];
                if (data != null)
                {
                    var ospSubtitles = data.OfType<XmlRpcStruct>().Select(resultRpc => resultRpc.ConvertTo<OSPSubtitle>()).ToList();
                    foreach (var ospSubtitle in ospSubtitles)
                    {
                        var movieDetails = details.FirstOrDefault(x => x.MovieHash == ospSubtitle.MovieHash);
                        if (movieDetails != null)
                        {
                            if (movieSubtitles.ContainsKey(movieDetails) == false)
                            {
                                movieSubtitles.Add(movieDetails, new List<OSPSubtitle>());
                            }
                            movieSubtitles[movieDetails].Add(ospSubtitle);
                        }
                    }
                }
                return movieSubtitles;
            }
            throw new Exception("Whoa!!! Search request not ok!! Response status: " + GetStatus(searchSubtitlesResponse));
        }

        public static List<OSPMovieDetails> DetailsNotFound = new List<OSPMovieDetails>();

        public static List<OSPMovieDetails> GetMovieDetails(XmlRpcStruct checkHashesResponse, Dictionary<string, string> hashFiles)
        {
            var details = new List<OSPMovieDetails>();

            var response = checkHashesResponse;
            if (StatusOk(response) && response.ContainsKey(RpcTag.Data))
            {
                var rpcStruct = response[RpcTag.Data] as XmlRpcStruct;
                if (rpcStruct != null)
                {
                    foreach (DictionaryEntry kvp in rpcStruct)
                    {
                        var movie = kvp.Value as XmlRpcStruct;
                        if (movie != null)
                        {
                            var detail = movie.ConvertTo<OSPMovieDetails>();
                            detail.MovieHash = kvp.Key.ToString();
                            if (hashFiles.ContainsKey(detail.MovieHash))
                            {
                                detail.VideoFilename = hashFiles[detail.MovieHash];
                            }
                            else
                            {
                                DetailsNotFound.Add(detail);
                            }
                            details.Add(detail);
                        }
                    }
                }
            }
            if (DetailsNotFound.Count > 0)
            {
                Log.WarnFormat("Couldn't find responses for {0} files.", DetailsNotFound.Count);
                Log.DebugFormat("Couldn't find hashes for:\r\n{0}", DetailsNotFound.Aggregate("", (s, movieDetails) => s + "\r\n" + movieDetails.MovieHash + " :: " + movieDetails.MovieName));
                DetailsNotFound.Clear();
            }

            return details;
        }

        public static ILog Log = LogManager.GetLogger(typeof(XmlRpcConverter));
    }
}