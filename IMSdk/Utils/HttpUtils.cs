using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using IMSdk.Result;

namespace IMSdk.Utils
{
    public class HttpUtils
    {
        private int timeout = 30000;
        private string _version;

        public string Version
        {
            get { return _version; }
            set { _version = value; }
        }

        public HttpUtils()
        {
        }

        public ResponseModel doPost(string url, Dictionary<string, string> param, Dictionary<string, string> header)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

            // header
            SetHeaderValue(request.Headers, "Content-Type", "application/x-www-form-urlencoded");
            SetHeaderValue(request.Headers, "User-Agent", $"Golang SDK v {Version ?? ""}");
            if (header != null)
            {
                foreach (var kv in header)
                {
                    SetHeaderValue(request.Headers, kv.Key, kv.Value);
                }
            }

            request.Method = "POST";
            request.Timeout = timeout;

            // param
            var postData = string.Join("&", param.Select(item => $"{item.Key}={item.Value}"));
            var bytes = Encoding.UTF8.GetBytes(postData);
            var writer = request.GetRequestStream();
            writer.Write(bytes, 0, bytes.Length);
            writer.Close();

            var response = (HttpWebResponse)request.GetResponse();
            var result = string.Empty;
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var reader = new StreamReader(response.GetResponseStream() ?? throw new InvalidOperationException(), Encoding.UTF8);
                result = reader.ReadToEnd();
            }

            var data = new ResponseModel
            {
                Result = result,
                StatusCode = response.StatusCode,
                Message = response.StatusDescription
            };

            response.Close();


            return data;
        }

        public void SetHeaderValue(WebHeaderCollection header, string name, string value)
        {
            var property = typeof(WebHeaderCollection).GetProperty("InnerCollection", BindingFlags.Instance | BindingFlags.NonPublic);
            if (property != null)
            {
                if (property.GetValue(header, null) is NameValueCollection collection)
                {
                    collection[name] = value;
                }
            }
        }
    }
}
