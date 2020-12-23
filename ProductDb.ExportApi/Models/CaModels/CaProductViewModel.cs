using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductDb.ExportApi.Models.CaModels
{
    public class CaProductViewModel
    {
        public CaParentProductModel channelAdvisorProductModel { get; set; }
        public CaProductModel channelAdvisorChildModel { get; set; }
        public List<Image> Images { get; set; }
        public DCQuantity DCQuantitys { get; set; }
        public Label labels { get; set; }
        public int StoreProductId { get; set; }
    }

    public class Image
    {
        public int ProductID { get; set; } = 0;
        public string PlacementName { get; set; }
        public UrlPath UrlPath { get; set; }
    }



    public class Label
    {
        public int ProductID { get; set; } = 0;
        public string Name { get; set; }
    }

    public class UrlPath
    {
        public string Url { get; set; }
    }

    public class DCQuantityandID
    {
        public int ProductID { get; set; }
        public DateTime? Time { get; set; }
        public DCQuantity dCQuantity { get; set; }
    }

    public class DCQuantity
    {
        public Value Value { get; set; }
    }

    public class Value
    {
        public string UpdateType { get; set; } = "InStock";
        public List<Update> Updates { get; set; }
    }

    public class Update
    {
        public int DistributionCenterID { get; set; } = 0;
        public int Quantity { get; set; }
    }
}
