using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace subdown
{
    public static class Utils
    {
        
        public static string Base64Encode(string plainString)
        {
            var bytes = Encoding.Default.GetBytes(plainString);
            var encoded = Convert.ToBase64String(bytes);
            return encoded;
        }

        public static string Base64Decode(string base64String)
        {
            var bytes = Convert.FromBase64String(base64String);
            var decoded = Encoding.Default.GetString(bytes);
            return decoded;
        }
    }
}
