using System;
using System.Diagnostics;
using System.Text;
using IMSdk;
using IMSdk.Model;
using IMSdk.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace Test
{
    [TestClass]
    public class Test
    {
        [TestMethod]
        public void TestStart()
        {
            try
            {
                var base64Secret = "";
                if (base64Secret.Length % 4 > 0)
                {
                    base64Secret = base64Secret.PadRight(base64Secret.Length + 4 - base64Secret.Length % 4, '=');
                }

                var outpuBytes = Convert.FromBase64String(base64Secret);
                var rawSecret = Encoding.UTF8.GetString(outpuBytes);
                var im = JsonConvert.DeserializeObject<IMModel>(rawSecret);

                var thirdUID = "test-go-tob";
                var aID = "test";

                var result = new Account(im).GetTokenByThirdUID(thirdUID, aID, "test-go", SexEnum.Unknown, "http://xxxxx/app/rank-list/static/img/defaultavatar.cd935fdb.png", "", "", "");
                if (!string.IsNullOrWhiteSpace(result.Item3))
                {
                    Debug.WriteLine(result.Item3);
                    return;
                }

                Debug.WriteLine($"token:{result.Item1},openID:{result.Item2}");

                var toUID = "1100";
                var objectName = "RC:textMsg";
                var content = "测试单聊";

                var dataPush = new DataPush(im);
                var res = dataPush.PushConverseData(thirdUID, toUID, objectName, content, "", "", "", "", "", "");
                if (!res.Item1)
                {
                    Debug.WriteLine($"PushConverseData exeute fail.{res.Item2 ?? ""}");
                    return;
                }

                Debug.WriteLine("PushConverseData exeute finish.");

                content = "测试 事件";
                res = dataPush.PushEventData(thirdUID, toUID, objectName, content, "", "", "", "");
                if (!res.Item1)
                {
                    Debug.WriteLine($"PushEventData exeute fail. {res.Item2}");
                    return;
                }

                Debug.WriteLine("PushEventData exeute finish.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Debug.WriteLine(ex.StackTrace);
            }
        }

        [TestMethod]
        public void TestBase64()
        {
            try
            {

                var str = "i love you";
                var base64Encoder = new Base64Encoder();
                var encodingStr = base64Encoder.GetEncoded(Encoding.UTF8.GetBytes(str));

                Debug.WriteLine($"encoding:{encodingStr}");

                var base64Decoder = new Base64Decoder();
                var decodingStr = Encoding.UTF8.GetString(base64Decoder.GetDecoded(encodingStr));

                Debug.WriteLine($"decoding:{ decodingStr}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        [TestMethod]
        public void TestBase64Convert()
        {
            byte[] val1 = { 5, 10, 15, 20, 25, 30 };
            string str = Convert.ToBase64String(val1);
            Debug.WriteLine($"Base 64 string: '{str}'");
            byte[] val2 = Convert.FromBase64String(str);
            Debug.WriteLine($"Converted byte value: {BitConverter.ToString(val2)}");
        }

        [TestMethod]
        public void TestUTC()
        {
            long currTicks = DateTime.UtcNow.Ticks;
            DateTime dtFrom = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            var temp = (currTicks - dtFrom.Ticks) / 10000L / 1000L;
            Debug.WriteLine(temp);
        }
    }

    ///   <summary>
    ///  Base64编码类。
    ///  将byte[]类型转换成Base64编码的string类型。
    ///   </summary>
    public class Base64Encoder
    {
        byte[] source;
        int length, length2;
        int blockCount;
        int paddingCount;
        public static Base64Encoder Encoder = new Base64Encoder();
        public Base64Encoder()
        { }
        private void init(byte[] input)
        {
            source = input;
            length = input.Length;
            if ((length % 3) == 0)
            {
                paddingCount = 0;
                blockCount = length / 3;
            }
            else
            {
                paddingCount = 3 - (length % 3);
                blockCount = (length + paddingCount) / 3;
            }
            length2 = length + paddingCount;
        }
        public string GetEncoded(byte[] input)
        {
            // 初始化
            init(input);
            byte[] source2;
            source2 = new byte[length2];
            for (int x = 0; x < length2; x++)
            {
                if (x < length)
                {
                    source2[x] = source[x];
                }
                else
                {
                    source2[x] = 0;
                }
            }
            byte b1, b2, b3;
            byte temp, temp1, temp2, temp3, temp4;
            byte[] buffer = new byte[blockCount * 4];
            char[] result = new char[blockCount * 4];
            for (int x = 0; x < blockCount; x++)
            {
                b1 = source2[x * 3];
                b2 = source2[x * 3 + 1];
                b3 = source2[x * 3 + 2];
                temp1 = (byte)((b1 & 252) >> 2);
                temp = (byte)((b1 & 3) << 4);
                temp2 = (byte)((b2 & 240) >> 4);
                temp2 += temp;
                temp = (byte)((b2 & 15) << 2);
                temp3 = (byte)((b3 & 192) >> 6);
                temp3 += temp;
                temp4 = (byte)(b3 & 63);
                buffer[x * 4] = temp1;
                buffer[x * 4 + 1] = temp2;
                buffer[x * 4 + 2] = temp3;
                buffer[x * 4 + 3] = temp4;
            }
            for (int x = 0; x < blockCount * 4; x++)
            {
                result[x] = sixbit2char(buffer[x]);
            }
            switch (paddingCount)
            {
                case 0:
                    break;
                case 1:
                    result[blockCount * 4 - 1] = '=';
                    break;
                case 2:
                    result[blockCount * 4 - 1] = '=';
                    result[blockCount * 4 - 2] = '=';
                    break;
                default:
                    break;
            }
            return new string(result);
        }
        private char sixbit2char(byte b)
        {
            char[] lookupTable = new char[64]
            {
                'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N',
                'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', 'a', 'b',
                'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p',
                'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '0', '1', '2', '3',
                '4', '5', '6', '7', '8', '9', '+', '/'
            };
            if ((b >= 0) && (b <= 63))
            {
                return lookupTable[(int)b];
            }
            else
            {
                return ' ';
            }
        }
    }

    ///   <summary>
    ///  Base64解码类
    ///  将Base64编码的string类型转换成byte[]类型
    ///   </summary>
    public class Base64Decoder
    {
        char[] source;
        int length, length2, length3;
        int blockCount;
        int paddingCount;
        public static Base64Decoder Decoder = new Base64Decoder();
        public Base64Decoder()
        { }
        private void init(char[] input)
        {
            int temp = 0;
            source = input;
            length = input.Length;
            for (int x = 0; x < 2; x++)
            {
                if (input[length - x - 1] == '=') temp++;
            }
            paddingCount = temp;
            blockCount = length / 4;
            length2 = blockCount * 3;
        }
        public byte[] GetDecoded(string strInput)
        {
            // 初始化
            init(strInput.ToCharArray());
            byte[] buffer = new byte[length];
            byte[] buffer2 = new byte[length2];
            for (int x = 0; x < length; x++)
            {
                buffer[x] = char2sixbit(source[x]);
            }
            byte b, b1, b2, b3;
            byte temp1, temp2, temp3, temp4;
            for (int x = 0; x < blockCount; x++)
            {
                temp1 = buffer[x * 4];
                temp2 = buffer[x * 4 + 1];
                temp3 = buffer[x * 4 + 2];
                temp4 = buffer[x * 4 + 3];
                b = (byte)(temp1 << 2);
                b1 = (byte)((temp2 & 48) >> 4);
                b1 += b;
                b = (byte)((temp2 & 15) << 4);
                b2 = (byte)((temp3 & 60) >> 2);
                b2 += b;
                b = (byte)((temp3 & 3) << 6);
                b3 = temp4;
                b3 += b;
                buffer2[x * 3] = b1;
                buffer2[x * 3 + 1] = b2;
                buffer2[x * 3 + 2] = b3;
            }
            length3 = length2 - paddingCount;
            byte[] result = new byte[length3];
            for (int x = 0; x < length3; x++)
            {
                result[x] = buffer2[x];
            }
            return result;
        }
        private byte char2sixbit(char c)
        {
            char[] lookupTable = new char[64]
            {
                'A','B','C','D','E','F','G','H','I','J','K','L','M','N',
                'O','P','Q','R','S','T','U','V','W','X','Y','Z','a','b',
                'c','d','e','f','g','h','i','j','k','l','m','n','o','p',
                'q','r','s','t','u','v','w','x','y','z','0','1','2','3',
                '4','5','6','7','8','9','+','/'
            };
            if (c == '=') return 0;
            else
            {
                for (int x = 0; x < 64; x++)
                {
                    if (lookupTable[x] == c) return (byte)x;
                }
                return 0;
            }
        }
    }
}
