using System;
using System.Collections.Generic;
using System.Text;

namespace ProductDb.Data.OnnetDb
{
    public class OnnetProduct
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ShortDescription { get; set; }
        public string Description { get; set; }
        public string LogoCode { get; set; }
        public string Barcode { get; set; }
        public string Model { get; set; }
        public int? Stock { get; set; }
        public decimal? Desi { get; set; }
        public decimal? BuyingPrice { get; set; }
        public decimal? MarketPrice { get; set; }
        public decimal? SalePrice { get; set; }
        public int? VatRateId { get; set; }
        public int? UnrealStock { get; set; }
    }
}
