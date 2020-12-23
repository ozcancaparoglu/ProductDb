using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductDb.ExportApi.DTOs
{
    public class ProductVariantDTO
    {
        public int BaseId { get; set; }
        public string Sku { get; set; }

        public string Gtip { get; set; }
        public string Barcode { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string ShortDescription { get; set; }

        public string Alias { get; set; }

        public string MetaKeywords { get; set; }

        public string MetaTitle { get; set; }

        public string MetaDescription { get; set; }

        public string SearchEngineTerms { get; set; }
        public decimal? Desi { get; set; }

        public decimal? AbroadDesi { get; set; }

        public decimal? BuyingPrice { get; set; }

        public int? ParentProductId { get; set; }
        public string Supplier { get; set; }
        public string Category { get; set; }
        public string Brand { get; set; }
        public string Currency { get; set; }
        public int? Stock { get; set; }
        public decimal? SellingPrice { get; set; }
        public string StoreCategory { get; set; }
    }
}
