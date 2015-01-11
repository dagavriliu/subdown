using System;
using System.Collections.Generic;
using CookComputing.XmlRpc;

namespace subdown.Providers.OpenSubtitles
{
    /// <summary>
    /// Properties are lowercase for reflected serialization
    /// </summary>
    [XmlRpcMissingMapping(MappingAction.Ignore)]
    public class OSPSubtitleSearchOption
    {

        //public string SubLanguageId = "", MovieHash = "", MovieByteSize = "", ImdbId = "", query = "", season = "", episode = "", tag = "";
        public string query;
        public string season;
        public string episode;
        public string tag;
        public string sublanguageid;
        public string moviehash;
        public double? moviebytesize;
        public string imdbid;

        //public static List<XmlRpcStruct> ToXmlRpcStruct(List<OSPSubtitleSearchOption> list)
        //{
        //    var xmlList = new List<XmlRpcStruct>();
        //    foreach (var option in list)
        //    {
        //        foreach (var field in option.GetType().GetFields())
        //        {
        //            if (field.FieldType == typeof(string))
        //            {
        //                if (!String.IsNullOrWhiteSpace(field.GetValue(option) as string))
        //                {

        //                }
        //            }
        //            else if (field.FieldType == typeof(double?))
        //            {
        //                var f = field.GetValue(option) as double?;
        //                if (f != null)
        //                {

        //                }
        //            }
        //        }

        //    }
        //    return xmlList;
        //}
    }
}