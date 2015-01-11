using System;
using System.Collections.Generic;
using CookComputing.XmlRpc;

namespace subdown.Providers.OpenSubtitles
{
    public class OSPMovieDetails
    {
        public string MovieImdbID;
        public string MovieName;
        public string SeriesEpisode;
        public string MovieKind;
        public string MovieHash;
        public string MovieYear;
        public string SeriesSeason;
        public string VideoFilename;

        public static XmlRpcStruct ToXmlRpcStruct(List<OSPMovieDetails> list)
        {
            var xml = new XmlRpcStruct();
            foreach (var details in list)
            {
                var val = new XmlRpcStruct();

                if (!String.IsNullOrWhiteSpace(details.MovieImdbID))
                {
                    val.Add("MovieImdbID", details.MovieImdbID);
                }
                if (!String.IsNullOrWhiteSpace(details.MovieKind))
                {
                    val.Add("MovieKind", details.MovieKind);
                }
                if (!String.IsNullOrWhiteSpace(details.MovieName))
                {
                    val.Add("MovieName", details.MovieName);
                }
                if (!String.IsNullOrWhiteSpace(details.MovieYear))
                {
                    val.Add("MovieYear", details.MovieYear);
                }
                if (!String.IsNullOrWhiteSpace(details.SeriesEpisode))
                {
                    val.Add("SeriesEpisode", details.SeriesEpisode);
                }
                if (!String.IsNullOrWhiteSpace(details.SeriesSeason))
                {
                    val.Add("SeriesSeason", details.SeriesSeason);
                }



                xml.Add(details.MovieHash, val);
            }
            return xml;
        }
    }
}