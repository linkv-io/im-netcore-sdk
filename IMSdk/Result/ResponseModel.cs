using System;
using System.Net;

namespace IMSdk.Result
{
    public class ResponseModel
    {
        public ResponseModel()
        {
        }

        public string Result;
        public HttpStatusCode StatusCode;
        public string Message;
    }
}
