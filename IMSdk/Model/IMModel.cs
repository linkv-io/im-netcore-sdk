using System;
using Newtonsoft.Json;

namespace IMSdk.Model
{
    public class IMModel
    {
        public IMModel()
        {
        }

        [JsonProperty("app_key")]
        public string AppKey { get; set; }

        [JsonProperty("app_secret")]
        public string AppSecret { get; set; }

        [JsonProperty("im_app_id")]
        public string ImAppId { get; set; }

        [JsonProperty("im_app_key")]
        public string ImAppKey { get; set; }

        [JsonProperty("im_app_secret")]
        public string ImAppSecret { get; set; }

        [JsonProperty("im_host")]
        public string ImHost { get; set; }
    }
}
