using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;


namespace subdown.Providers.OpenSubtitles
{
    public class Utils
    {
        /// <summary>
        /// The specific compression required by OpenSubtitles.org
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string Uncompress(string input)
        {
            var compressed = GZipUtility.Compress(input);
            var base64Encoded = subdown.Utils.Base64Encode(compressed);
            return base64Encoded;
        }

        /// <summary>
        /// The specific Decompression required by OpenSubtitles.org
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string Decompress(string input)
        {
            var base64Decoded = subdown.Utils.Base64Decode(input);
            var decompressed = GZipUtility.Decompress(base64Decoded);
            return decompressed;
        }

        public static string ComputeHash(string filename)
        {
            return ToHex(ComputeMovieHash(filename));
        }

        public static string[] ComputeHashes(string[] filenames)
        {
            var hashes = new string[filenames.Length];
            for (int i = 0; i < hashes.Length; i++)
            {
                hashes[i] = ComputeHash(filenames[i]);
            }
            return hashes;
        }

        public static byte[] ComputeMovieHash(string filename)
        {
            byte[] result;
            using (Stream input = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                result = ComputeMovieHash(input);
            }
            return result;
        }

        public static byte[] ComputeMovieHash(Stream input)
        {
            long lhash, streamsize;
            streamsize = input.Length;
            lhash = streamsize;

            long i = 0;
            byte[] buffer = new byte[sizeof(long)];
            while (i < 65536 / sizeof(long) && (input.Read(buffer, 0, sizeof(long)) > 0))
            {
                i++;
                lhash += BitConverter.ToInt64(buffer, 0);
            }

            input.Position = Math.Max(0, streamsize - 65536);
            i = 0;
            while (i < 65536 / sizeof(long) && (input.Read(buffer, 0, sizeof(long)) > 0))
            {
                i++;
                lhash += BitConverter.ToInt64(buffer, 0);
            }
            input.Close();

            byte[] result = BitConverter.GetBytes(lhash);
            Array.Reverse(result);
            return result;
        }

        public static string ToHex(byte[] bytes)
        {
            var hexBuilder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                hexBuilder.Append(bytes[i].ToString("x2"));
            }
            return hexBuilder.ToString();
        }
    }
}
