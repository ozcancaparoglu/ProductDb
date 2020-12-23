using System;
using System.Collections.Generic;
using System.Text;

namespace ProductDb.Mapping.BiggBrandDbModels.ImportModels
{
    public class StoreProductFixingModel
    {
        public int MapId { get; set; }
        public string CatalogCode { get; set; }
        public decimal ErpPrice { get; set; }
        public bool IsFixed { get; set; }
        public string CatalogName { get; set; }
        public bool? IsFixedPoint { get; set; } = false;
        public decimal? ErpPoint { get; set; }
        public decimal? Point { get; set; }
        public decimal? Price { get; set; }
        public Nullable<int> VatRate { get; set; } = null;
    }
}
