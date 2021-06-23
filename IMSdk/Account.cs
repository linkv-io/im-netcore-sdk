using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using IMSdk.Model;
using IMSdk.Utils;
using Newtonsoft.Json;

namespace IMSdk
{
    public class Account
    {
        private IMModel _im;
        public Account(IMModel im)
        {
            _im = im;
        }


        public class RespStatusModel
        {
            public int Status { get; set; }
            public string Msg { get; set; }
            public RespDataModel Data { get; set; }

        }

        public class RespDataModel
        {
            public string Token { get; set; }
            public string OpenID { get; set; }
            [JsonProperty("im_token")]
            public string IMToken { get; set; }
        }

        public Tuple<string, string, string> GetTokenByThirdUID(string thirdUID, string aID, string userName, SexEnum sex,
            string portraitURI, string userEmail, string countryCode, string birthday)
        {
            if (string.IsNullOrWhiteSpace(thirdUID)
                || string.IsNullOrWhiteSpace(aID))
            {
                return new Tuple<string, string, string>("", "", "param error");
            }

            var param = new Dictionary<string, string>();
            var nonce = CommUtils.GenRandomString();
            param.Add("nonce_str", nonce);
            param.Add("app_id", _im.AppKey);

            param.Add("userId", thirdUID);
            param.Add("aid", aID);

            Action<Dictionary<string, string>, string, string> addDict = (Dictionary<string, string> dict, string key, string value) =>
               {
                   if (!string.IsNullOrWhiteSpace(value))
                   {
                       param.Add(key, value);
                   }
               };

            addDict(param, "name", userName);
            addDict(param, "portraitUri", portraitURI);
            addDict(param, "email", userEmail);
            addDict(param, "countryCode", countryCode);
            addDict(param, "birthday", birthday);

            if (sex == SexEnum.Male) { param.Add("sex", "1"); }
            if (sex == SexEnum.Female) { param.Add("sex", "0"); }

            param.Add("sign", CommUtils.GenSignature(param, _im.AppSecret));

            var err = string.Empty;
            for (var i = 0; i < 3; i++)
            {
                var result = new HttpUtils().doPost("http://thr.linkv.sg/open/v0/thGetToken", param, null);
                if (result.StatusCode != HttpStatusCode.OK)
                {
                    return new Tuple<string, string, string>("", "", result.Message);
                }

                var model = JsonConvert.DeserializeObject<RespStatusModel>(result.Result);
                if (model.Status == 200)
                {
                    return new Tuple<string, string, string>(model.Data.IMToken, model.Data.OpenID, null);
                }

                if (model.Status == 500)
                {
                    err = $"message({model.Msg})";
                    Thread.Sleep(300);
                    continue;
                }

                return new Tuple<string, string, string>("", "", $"message({model.Msg})");
            }

            return new Tuple<string, string, string>("", "", err);
        }
    }
}
