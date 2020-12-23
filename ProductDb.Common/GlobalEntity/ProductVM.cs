using System;
using System.Collections.Generic;
using System.Text;

namespace ProductDb.Common.GlobalEntity
{
    public class ProductVM
    {
        public List<ProductJsonModel> products { get; set; }
        public List<ProductStockAndPriceJsonModel> priceandStock { get; set; }
    }
}
