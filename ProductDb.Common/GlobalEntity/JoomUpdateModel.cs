using System;
using System.Collections.Generic;
using System.Text;

namespace ProductDb.Common.GlobalEntity
{
    public class JoomUpdateModel
    {
        public bool enabled { get; set; } = true;
        public string color { get; set; }
        public string hs_code { get; set; }
        public string main_image { get; set; }
        public string msrp { get; set; }
        public string price { get; set; }
        public string shipping { get; set; }
        public string shipping_height { get; set; }
        public string shipping_length { get; set; }
        public string shipping_weight { get; set; }
        public string shipping_width { get; set; }
        public string size { get; set; }
        public string declaredValue { get; set; }
        public string gtin { get; set; }
        public int inventory { get; set; }
        public string sku { get; set; }
        public string access_token { get; set; }
    }
}
