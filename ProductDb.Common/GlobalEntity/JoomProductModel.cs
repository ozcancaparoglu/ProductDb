using System;
using System.Collections.Generic;
using System.Text;

namespace ProductDb.Common.GlobalEntity
{
    public class JoomProductModel
    {
        public string name { get; set; }
        public string description { get; set; }
        public List<string> extraImages { get; set; }
        public string landingPageUrl { get; set; } = "";
        public string gtin { get; set; }
        public List<string> tags { get; set; }
        public bool enabled { get; set; } = true;
        public string dangerousKind { get; set; } = "notDangerous";
        public string brand { get; set; }
        public string mainImage { get; set; }
        //public string categoryId { get; set; }
        public string sku { get; set; }
        public List<JoomProductVariant> variants { get; set; }
        public List<JoomUpdateModel> joomUpdateModels { get; set; }
    }

    public class JoomProductVariant
    {
        public string mainImage { get; set; }
        public bool enabled { get; set; } = true;
        public int inventory { get; set; }
        public string price { get; set; }
        public string msrPrice { get; set; }
        //public string shippingPrice { get; set; }
        //public string declaredValue { get; set; }
        public string currency { get; set; } = "EUR";
        public string colors { get; set; }
        public string size { get; set; }
        //public List<string> extraImages { get; set; }
        //public decimal shippingWeight { get; set; }
        //public decimal shippingLength { get; set; }
        //public decimal shippingWidth { get; set; }
        //public decimal shippingHeight { get; set; }
        public string gtin { get; set; }
        public string sku { get; set; }
    }
}
