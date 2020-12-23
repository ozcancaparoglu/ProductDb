using System;
using System.Collections.Generic;
using System.Text;

namespace ProductDb.Common.GlobalEntity
{
    public class ChannelAdvisorGetProduct
    {
        public string odatacontext { get; set; }
        public List<ProductInfo> value{ get; set; }
    }
    public class ProductInfo
    {
        public string Sku{ get; set; }
        public int ID { get; set; }
    }
}
