using System;
using System.Collections.Generic;
using System.Text;

namespace ProductDb.Common.GlobalEntity
{
    public class JoomTokenRequestModel
    {
        public int Code { get; set; }
        public Data Data { get; set; }
    }

    public class Data
    {
        public string access_token { get; set; }
        public string refresh_token { get; set; }
        public int expires_in { get; set; }
        public int expiry_time { get; set; }
        public string merchant_user_id { get; set; }
    }
}
