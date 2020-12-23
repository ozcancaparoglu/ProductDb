using System;
using System.Collections.Generic;
using System.Text;

namespace ApiClient.HttpClient
{
    //[Obsolete("Deprecated use service result.")]
    public class ApiResult
    {
        public int ResponseCode { get; set; }
        public string JsonContent { get; set; }
    }

}
