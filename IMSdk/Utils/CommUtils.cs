using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using IMSdk.Model;

namespace IMSdk.Utils
{
    public class CommUtils
    {

        public static string GenUniqueIDString(string appKey)
        {
            var nlen = 9;
            var appKeyBytes = Encoding.UTF8.GetBytes(appKey);
            var container = $"{Encoding.UTF8.GetString(appKeyBytes.Skip(2).Take(appKeyBytes.Length - 2).ToArray())}-";
            var str = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";

            var cache = new List<char>();
            var random = new Random();
            var maxRandomValue = str.Length;
            for (var i = 0; i < nlen; i++)
            {
                cache.Add(str[random.Next(maxRandomValue)]);
            }

            return string.Join("", container, string.Join("", cache));
        }

        public static string GenRandomString()
        {
            var nlen = 16;
            var str = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";

            var maxRandomValue = str.Length;
            var random = new Random();
            var cache = new List<string>();

            for (var i = 0; i < nlen; i++)
            {
                var idx = random.Next(maxRandomValue);
                cache.Add(str[idx].ToString());
                if (i == 7)
                {
                    cache.Add(CurrentTimeMillis().ToString());
                }
            }

            return string.Join("", cache);
        }

        public static string genCheckSum(string str, CheckSumAlgoType checkSumAlgo)
        {
            return genCheckSum(str, checkSumAlgo, true);
        }

        public static string genCheckSum(string str, CheckSumAlgoType checkSumAlgo, bool isLowerCase)
        {
            var messageDigest = MessageDigest.GetInstance(checkSumAlgo.ToString());
            messageDigest.Update(Encoding.UTF8.GetBytes(str));
            var digestBytes = messageDigest.Digest();

            var checkSum = messageDigest.PrintHexBinary(digestBytes);
            return isLowerCase ? checkSum.ToLower() : checkSum.ToUpper();
        }

        public static string GenSignature(Dictionary<string, string> urlValues, string md5Secret)
        {
            var data = $"{encode(urlValues)}&key={md5Secret}";
            var hex = MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(data)).Select(item => item.ToString("x2"));
            return string.Join("", hex);
        }

        private static string encode(Dictionary<string, string> urlValues)
        {
            if (urlValues == null || urlValues.Count == 0) return "";

            var sortedDic = urlValues.OrderBy(item => item.Key);
            var param = sortedDic.Select(item => $"{item.Key}={HttpUtility.UrlDecode(item.Value, Encoding.UTF8)}");
            return string.Join("&", param);
        }

        public static string RandomString(int nlen)
        {
            var str = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            var buffer = new List<char>();

            var len = str.Length;
            var random = new Random();
            for (var i = 0; i < nlen; i++)
            {
                buffer.Add(str[random.Next(len)]);
            }

            return string.Join("", buffer);
        }

        public static string GenGUID()
        {
            return string.Join("-", RandomString(9), RandomString(4), RandomString(4), RandomString(12));
        }

        /// <summary>
        /// Answers the current time expressed as milliseconds since the time 00:00:00 UTC on January 1, 1970.
        /// Returns: the time in milliseconds
        /// </summary>
        /// <returns></returns>
        public static long CurrentTimeMillis()
        {
            long currTicks = DateTime.UtcNow.Ticks;
            DateTime dtFrom = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            var temp = (currTicks - dtFrom.Ticks) / 10000L / 1000L;
            return temp;
        }
    }
}
