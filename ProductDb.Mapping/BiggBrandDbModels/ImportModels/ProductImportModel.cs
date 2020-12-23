using System;
using System.Collections.Generic;
using System.Text;

namespace ProductDb.Mapping.BiggBrandDbModels.ImportModels
{
    public class ProductImportModel
    {
        //[Required(ErrorMessage = "*This field is mandatory")]
        public string Sku { get; set; }

        //[Required(ErrorMessage = "*This field is mandatory")]
        public string Name { get; set; }

        //[Required(ErrorMessage = "*This field is mandatory")]
        public string Barcode { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string ShortDescription { get; set; }

        public string Alias { get; set; }

        public string MetaKeywords { get; set; }

        public string MetaTitle { get; set; }

        public string MetaDescription { get; set; }

        public string SearchEngineTerms { get; set; }
        public decimal BuyingPrice { get; set; }
        public decimal MarketPrice { get; set; }
        public decimal CorporatePrice { get; set; }
        public decimal FobPrice { get; set; }
        public decimal AbroadDesi { get; set; }
        public decimal? Desi { get; set; }
        public int VatRate { get; set; }

        public string BulletPoint1 { get; set; }
        public string BulletPoint2 { get; set; }
        public string BulletPoint3 { get; set; }
        public string BulletPoint4 { get; set; }
        public string BulletPoint5 { get; set; }
        public string ShippingWeight { get; set; }

        public int? CurrencyId { get; set; }
        public int? PsfCurrencyId { get; set; }
        public int? CorporateCurrencyId { get; set; }
        public int? FobCurrencyId { get; set; }

    }
}
