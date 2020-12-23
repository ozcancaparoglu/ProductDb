using System;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace PMS.LogoService
{
    public static class EncodeHelper
    {
        public static string Base64Encode(string plainText)
        {
            if (string.IsNullOrWhiteSpace(plainText)) return null;

            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64Decode(string base64EncodedData)
        {
            if (string.IsNullOrWhiteSpace(base64EncodedData)) return null;

            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
    }

    /////////////////////
    ///
    public static class StringCompressor
    {
        private static void CopyTo(Stream src, Stream dest)
        {
            byte[] bytes = new byte[4096];
            int cnt;
            while ((cnt = src.Read(bytes, 0, bytes.Length)) != 0)
            {
                dest.Write(bytes, 0, cnt);
            }
        }

        public static byte[] Zip(string str)
        {
            var bytes = Encoding.UTF8.GetBytes(str);
            using (var msi = new MemoryStream(bytes))
            using (var mso = new MemoryStream())
            {
                using (var gs = new GZipStream(mso, CompressionMode.Compress))
                {
                    CopyTo(msi, gs);
                }
                return mso.ToArray();
            }
        }

        public static string Unzip(byte[] bytes)
        {
            using (var msi = new MemoryStream(bytes))
            using (var mso = new MemoryStream())
            {
                using (var gs = new GZipStream(msi, CompressionMode.Decompress))
                {
                    CopyTo(gs, mso);
                }
                return Encoding.UTF8.GetString(mso.ToArray());
            }
        }

        // Base64
        public static string ZipBase64(string compress)
        {
            var bytes = Zip(compress);
            var encoded = Convert.ToBase64String(bytes, Base64FormattingOptions.None);
            return encoded;
        }

        public static string UnzipBase64(string compressRequest)
        {
            var bytes = Convert.FromBase64String(compressRequest);
            var unziped = Unzip(bytes);
            return unziped;
        }
    }
}