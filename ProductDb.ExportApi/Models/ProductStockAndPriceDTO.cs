using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductDb.ExportApi.Models
{
    public class ProductStockAndPriceDTO
    {
        public string Sku { get; set; }
        public int Stock { get; set; }
        public decimal Price { get; set; }
    }
}
