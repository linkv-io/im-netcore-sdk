using System;
using System.Security.Cryptography;
using System.Text;

namespace IMSdk.Utils
{
    public sealed class MessageDigest
    {
        private MessageDigest()
        {

        }

        private string disgestType;

        public static MessageDigest GetInstance(string disgestType)
        {
            var messageDisgest = new MessageDigest();
            messageDisgest.disgestType = disgestType.ToLower();
            return messageDisgest;
        }

        private byte[] input;
        public void Update(byte[] input)
        {
            this.input = input;
        }

        private HashAlgorithm hashDigestFactory()
        {
            if (disgestType.Equals("md5")) return new MD5CryptoServiceProvider();
            if (disgestType.Equals("sha_256")) return new SHA256Managed();
            if (disgestType.Equals("sha_1")) return new SHA1CryptoServiceProvider();
            return null;
        }

        public byte[] Digest()
        {
            HashAlgorithm instance = hashDigestFactory();
            if (instance == null) throw new NotImplementedException($"{disgestType} 摘要算法未实现");
            byte[] digestBytes = instance.ComputeHash(input);
            instance.Clear();
            return digestBytes;
        }

        public string PrintHexBinary(byte[] bytes)
        {
            var sb = new StringBuilder();
            for (var i = 0; i < bytes.Length; i++)
            {
                sb.Append(bytes[i].ToString("x2"));
            }
            return sb.ToString();
        }
    }
}
