using System;
using System.Collections.Generic;
using System.Text;

namespace ProductDb.Common.GlobalEntity
{
    [Serializable]
    public class ProductStockAndPriceJsonModel
    {
        public string Sku { get; set; }
        public int Stock { get; set; }
        public decimal Price { get; set; }
    }
}
