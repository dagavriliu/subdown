using System.Collections;
using System.IO;
using CookComputing.XmlRpc;

namespace subdown.Providers.OpenSubtitles
{
    public static class XmlRpcUtils
    {
        public static XmlRpcStruct ConvertTo<T>(this T source) where T : new()
        {
            var x = new XmlRpcStruct();
            return x;

        }

        public static T ConvertTo<T>(this XmlRpcStruct rpc) where T : new()
        {
            var md = new T();

            var fields = typeof(T).GetFields();
            foreach (var field in fields)
            {
                if (rpc.ContainsKey(field.Name))
                {
                    field.SetValue(md, rpc[field.Name]);
                }
            }
            return md;
        }

        public static void PrintStruct(TextWriter tw, object rpcStruct, string padding = "")
        {
            //_printStruct(rpcStruct, padding);

            if (rpcStruct.GetType() == typeof(XmlRpcStruct) == false)
            {
                tw.WriteLine(rpcStruct);
            }
            if (rpcStruct.GetType() == typeof(object[]))
            {
                tw.Write("[");
                var objects = rpcStruct as object[];
                if (objects != null)
                {
                    foreach (var o in objects)
                    {
                        PrintStruct(tw, o, padding + " ");
                    }
                }
                tw.Write("]");
            }
            else
            {
                var xmlRpcStruct = rpcStruct as XmlRpcStruct;
                if (xmlRpcStruct != null)
                    foreach (DictionaryEntry dictionaryEntry in xmlRpcStruct)
                    {
                        tw.Write(padding + dictionaryEntry.Key + " = ");
                        if (dictionaryEntry.Value.GetType() == typeof(XmlRpcStruct))
                        {
                            tw.WriteLine("{");
                            PrintStruct(tw, dictionaryEntry.Value as XmlRpcStruct, padding + " ");
                            tw.WriteLine("}");
                        }
                        else if (dictionaryEntry.Value.GetType() == typeof(object[]))
                        {
                            tw.WriteLine("[");
                            var objects = dictionaryEntry.Value as object[];
                            if (objects != null)
                            {
                                foreach (var o in objects)
                                {
                                    var d = o as XmlRpcStruct;
                                    if (d != null)
                                    {
                                        PrintStruct(tw, d, padding + " ");
                                    }
                                }
                            }
                            tw.Write("]");
                        }
                        else
                        {
                            tw.Write(dictionaryEntry.Value + "\r\n");
                        }

                    }
            }
        }
    }
}