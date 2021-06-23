using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using IMSdk.Model;
using IMSdk.Utils;
using Newtonsoft.Json;

namespace IMSdk
{
    public class DataPush
    {
        private IMModel _im;

        public DataPush(IMModel im)
        {
            _im = im;
        }

        public class DataModel
        {
            public int Code { get; set; }
        }

        public Tuple<bool, string> PushConverseData(string fromUID, string toUID, string objectName, string content, string pushContent,
                                                  string pushData, string deviceID, string toAppID, string toUserExtSysUserID, string isCheckSensitiveWords)
        {
            var nonce = CommUtils.GenGUID();
            var timestamp = CommUtils.CurrentTimeMillis().ToString();

            var arr = new string[] { nonce, timestamp, _im.ImAppSecret };
            arr = arr.OrderBy(item => item).ToArray();
            var cmimToken = CommUtils.genCheckSum(string.Join("", arr), CheckSumAlgoType.MD5);
            var sign = CommUtils.genCheckSum(string.Join("|", _im.ImAppId, _im.ImAppKey, timestamp, nonce), CheckSumAlgoType.SHA_1, false);

            var headers = new Dictionary<string, string>();
            headers.Add("nonce", nonce);
            headers.Add("timestamp", timestamp);
            headers.Add("cmimToken", cmimToken);
            headers.Add("sign", sign);
            headers.Add("appkey", _im.ImAppKey);
            headers.Add("appId", _im.ImAppId);
            headers.Add("appUid", fromUID);

            var param = new Dictionary<string, string>();
            param.Add("fromUserId", fromUID);
            param.Add("toUserId", toUID);
            param.Add("objectName", objectName);
            param.Add("content", content);
            param.Add("appId", _im.ImAppId);

            Action<Dictionary<string, string>, string, string> addDict = (Dictionary<string, string> dict, string key, string value) =>
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    param.Add(key, value);
                }
            };

            addDict(param, "pushContent", pushContent);
            addDict(param, "pushData", pushData);
            addDict(param, "deviceId", deviceID);
            addDict(param, "toUserAppid", toAppID);
            addDict(param, "toUserExtSysUserId", toUserExtSysUserID);
            addDict(param, "isCheckSensitiveWords", isCheckSensitiveWords);

            var uri = $"{_im.ImHost}/api/rest/message/converse/pushConverseData";
            var err = "";

            for (var i = 0; i < 3; i++)
            {
                var result = new HttpUtils().doPost(uri, param, headers);
                if (result == null || result.StatusCode != HttpStatusCode.OK)
                {
                    return new Tuple<bool, string>(false, $"httpStatusCode({result.StatusCode}) != 200");
                }

                var model = JsonConvert.DeserializeObject<DataModel>(result.Result);
                if (model.Code == 200)
                {
                    return new Tuple<bool, string>(true, null);
                }

                if (model.Code == 500)
                {
                    err = $"message({model.Code})";
                    Thread.Sleep(300);
                    continue;
                }

                return new Tuple<bool, string>(false, $"code not 200({model.Code})");
            }

            return new Tuple<bool, string>(false, err);
        }

        public Tuple<bool, string> PushEventData(string fromUID, string toUID, string objectName, string content, string pushData,
                                               string toAppID, string toUserExtSysUserID, string isCheckSensitiveWords)
        {
            var nonce = CommUtils.GenGUID();
            var timestamp = CommUtils.CurrentTimeMillis().ToString();

            var arr = new string[] { nonce, timestamp, _im.ImAppSecret };
            arr = arr.OrderBy(item => item).ToArray();
            var cmimToken = CommUtils.genCheckSum(string.Join("", arr), CheckSumAlgoType.MD5);
            var sign = CommUtils.genCheckSum(string.Join("|", _im.ImAppId, _im.ImAppKey, timestamp, nonce), CheckSumAlgoType.SHA_1, false);

            var headers = new Dictionary<string, string>();
            headers.Add("nonce", nonce);
            headers.Add("timestamp", timestamp);
            headers.Add("cmimToken", cmimToken);
            headers.Add("sign", sign);
            headers.Add("appkey", _im.ImAppKey);
            headers.Add("appId", _im.ImAppId);
            headers.Add("appUid", fromUID);

            var param = new Dictionary<string, string>();
            param.Add("fromUserId", fromUID);
            param.Add("toUserId", toUID);
            param.Add("objectName", objectName);
            param.Add("content", content);
            param.Add("appId", _im.ImAppId);


            Action<Dictionary<string, string>, string, string> addDict = (Dictionary<string, string> dict, string key, string value) =>
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    param.Add(key, value);
                }
            };

            addDict(param, "pushData", pushData);
            addDict(param, "toUserAppid", toAppID);
            addDict(param, "toUserExtSysUserId", toUserExtSysUserID);
            addDict(param, "isCheckSensitiveWords", isCheckSensitiveWords);

            var uri = $"{_im.ImHost}/api/rest/sendEventMsg";
            var err = "";

            for (var i = 0; i < 3; i++)
            {
                var result = new HttpUtils().doPost(uri, param, headers);
                if (result == null || result.StatusCode != HttpStatusCode.OK)
                {
                    return new Tuple<bool, string>(false, $"httpStatusCode({result.StatusCode}) != 200");
                }

                var model = JsonConvert.DeserializeObject<DataModel>(result.Result);
                if (model.Code == 200)
                {
                    return new Tuple<bool, string>(true, null);
                }

                if (model.Code == 500)
                {
                    err = $"message({model.Code})";
                    Thread.Sleep(300);
                    continue;
                }

                return new Tuple<bool, string>(false, $"code not 200({model.Code})");
            }

            return new Tuple<bool, string>(false, err);
        }
    }
}
