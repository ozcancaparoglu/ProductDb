using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductDb.Admin.PageModels.Currency
{
    public class CurrencyViewModel
    {
        public string Name { get; set; }
        public string Abbrevation { get; set; }
        public decimal Value { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
