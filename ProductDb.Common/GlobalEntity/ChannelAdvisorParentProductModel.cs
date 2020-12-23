using System;
using System.Collections.Generic;
using System.Text;

namespace ProductDb.Common.GlobalEntity
{
    public class ChannelAdvisorParentProductModel
    {
        public string ProfileID { get; set; }
        public string Sku { get; set; }
        public bool IsParent { get; set; } = true;
        public bool IsInRelationship { get; set; } = true;
        public string RelationshipName { get; set; }
        public string Title { get; set; }
        public string Brand { get; set; }
        public string Manufacturer { get; set; }
        public string Condition { get; set; } = "New";
        public string Description { get; set; }
        public decimal BuyItNowPrice { get; set; }
        public string ShortDescription { get; set; }
        public string Warranty { get; set; } = "2 Year";
    }
}
