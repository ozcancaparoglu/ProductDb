using System;
using System.Collections.Generic;
using System.Text;

namespace ProductDb.Common.GlobalEntity
{
    public class ChannelAdvisorTrackingNumberModel
    {
        public TrackingNumberData Value { get; set; }
    }
    public class TrackingNumberData
    {
        public DateTime ShippedDateUtc { get; set; }
        public string TrackingNumber { get; set; }
        public string SellerFulfillmentID { get; set; }
        public int DistributionCenterID { get; set; }
        public string DeliveryStatus { get; set; }
        public string ShippingCarrier { get; set; }
        public string ShippingClass { get; set; }
    }

}
