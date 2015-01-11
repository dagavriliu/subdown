using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CookComputing.XmlRpc;
using System.Collections;
using subdown;
using subdown.Providers.OpenSubtitles;

namespace xml_rpc_opensubs_test
{
    // should be wrapped around a class 



    class Program
    {

        static void Main(string[] args)
        {
            try
            {
                var osp = new OpenSubtitlesProvider();
                var result = osp.Proxy.ServerInfo() as XmlRpcStruct;

                if (result != null)
                {
                    foreach (DictionaryEntry r in (result))
                    {
                        Console.WriteLine(r.Key + "=" + r.Value);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
