using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace M3u8Downloader_H._555dd7.plugin.Utils
{
    internal static class StringExtensions
    {
        public static string GetMD5HexString(this string s)
        {
            byte[] data = s.GetMd5Bytes();
            return Convert.ToHexString(data);
        }

        public static byte[] GetMd5Bytes(this string s )
        {
            using MD5 md5 = MD5.Create();
            return  md5.ComputeHash(Encoding.UTF8.GetBytes(s));
        }
    }
}
