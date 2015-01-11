using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace subdown
{
    public static class GZipUtility
    {

        /// <see cref="http://stackoverflow.com/questions/3722192/how-do-i-use-gzipstream-with-system-io-memorystream#3722263"/>
        public static string Compress(String inputString, Encoding encoding = null)
        {
            encoding = encoding ?? Encoding.Default;

            using (var outStream = new MemoryStream())
            {
                using (var tinyStream = new GZipStream(outStream, CompressionMode.Compress))
                using (var mStream = new MemoryStream(Encoding.UTF8.GetBytes(inputString)))
                {
                    mStream.CopyTo(tinyStream);
                }

                var compressed = encoding.GetString(outStream.GetBuffer());

                return compressed;
            }
        }



        public static string Decompress(String input, Encoding encoding = null)
        {
            encoding = encoding ?? Encoding.Default;
            var bytes = Decompress(encoding.GetBytes(input), encoding);

            return bytes;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="compressed"></param>
        /// <param name="encoding"> </param>
        /// <returns></returns>
        /// <remarks>Ionic GZipStream can handle headerless GZip</remarks>
        /// <see cref="http://stackoverflow.com/questions/3722192/how-do-i-use-gzipstream-with-system-io-memorystream#3722263"/>
        public static string Decompress(byte[] compressed, Encoding encoding)
        {
            encoding = encoding ?? Encoding.Default;
            string output;
            using (var inStream = new MemoryStream(compressed))
            using (var bigStream = new GZipStream(inStream, CompressionMode.Decompress))
            using (var bigStreamOut = new MemoryStream())
            {
                bigStream.CopyTo(bigStreamOut);
                output = encoding.GetString(bigStreamOut.ToArray());
            }
            return output;
        }

    }
}

