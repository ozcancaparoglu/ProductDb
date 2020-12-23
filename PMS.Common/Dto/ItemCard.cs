using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.Common.Dto
{
    public class ItemCard
    {
        public string Sku { get; set; }
        public string Title { get; set; }
        public string Barcode { get; set; }
        public string Gtip { get; set; }
        public string Description { get; set; }
        public string Brand { get; set; }
        public string Name { get; set; }
        public string Model { get; set; }
        public string ShortDescription { get; set; }
        public string Alias { get; set; }
        public string MetaKeywords { get; set; }
        public string MetaTitle { get; set; }
        public string MetaDescription { get; set; }
        public string SearchEngineTerms { get; set; }
        public int VatRate { get; set; }
        public decimal? Desi { get; set; }
        public decimal? AbroadDesi { get; set; }
        public decimal? BuyingPrice { get; set; }
        public decimal? PsfPrice { get; set; }
        public decimal? CorporatePrice { get; set; }
    }
}
