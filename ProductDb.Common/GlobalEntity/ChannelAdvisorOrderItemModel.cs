using System;
using System.Collections.Generic;
using System.Text;

namespace ProductDb.Common.GlobalEntity
{
    public class ChannelAdvisorOrderItemModel
    {
        public string odatacontext { get; set; }
        public List<OrderItemInfo> value { get; set; }
    }
    public class OrderItemInfo
    {
        public int ID { get; set; }
        public int ProfileID { get; set; }
        public int OrderID { get; set; }
        public int ProductID { get; set; }
        public string SiteOrderItemID { get; set; }
        public int? SellerOrderItemID { get; set; }
        public string SiteListingID { get; set; }
        public string Sku { get; set; }
        public string Title { get; set; }
        public int Quantity { get; set; }
        public double UnitPrice { get; set; }
        public double? TaxPrice { get; set; }
        public double? ShippingPrice { get; set; }
        public double? ShippingTaxPrice { get; set; }
        public double? RecyclingFee { get; set; }
        public double? UnitEstimatedShippingCost { get; set; }
        public string GiftMessage { get; set; }
        public string GiftNotes { get; set; }
        public double? GiftPrice { get; set; }
        public double? GiftTaxPrice { get; set; }
        public bool IsBundle { get; set; }
        public string ItemURL { get; set; }
        public string HarmonizedCode { get; set; }
    }
}

