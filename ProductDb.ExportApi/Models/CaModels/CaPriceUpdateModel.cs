using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductDb.ExportApi.Models.CaModels
{
    public class CaPriceUpdateModel
    {
        public int ProductId { get; set; }
        public CaPrice UpdatePrice { get; set; }
    }
    public class CaPrice
    {
        public decimal BuyItNowPrice { get; set; }
    }
}
