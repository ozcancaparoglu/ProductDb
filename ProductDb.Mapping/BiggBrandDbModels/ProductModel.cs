using ProductDb.Common.Entities;
using System;
using System.Collections.Generic;

namespace ProductDb.Mapping.BiggBrandDbModels
{
    public class ProductModel : EntityBaseModel
    {
        public string Sku { get; set; }

        public string Gtip { get; set; }

        public string Barcode { get; set; }

        public string Title { get; set; }

        public string Name { get; set; }

        public string Model { get; set; }

        public string Description { get; set; }

        public string ShortDescription { get; set; }

        public string Alias { get; set; }

        public string MetaKeywords { get; set; }

        public string MetaTitle { get; set; }

        public string MetaDescription { get; set; }

        public string SearchEngineTerms { get; set; }

        public bool IsStoreCheck { get; set; }

        public bool IsParentCheck { get; set; }

        public int Stock { get; set; }

        public decimal Price { get; set; }

        public int? SupplierUniqueId { get; set; }

        public decimal? Desi { get; set; }
        public decimal? AbroadDesi { get; set; }
        public decimal? BuyingPrice { get; set; }
        public decimal? PsfPrice { get; set; }
        public decimal? CorporatePrice { get; set; }
        public decimal? DdpPrice { get; set; }
        public decimal? FobPrice { get; set; }

        //[RegularExpression("^-?[0-9]*\\.?[0-9]+$", ErrorMessage = "Buying price should be decimal.")]
        public string BuyingPriceString { get; set; }
        //[RegularExpression("^-?[0-9]*\\.?[0-9]+$", ErrorMessage = "Psf price should be decimal.")]
        public string PsfPriceString { get; set; }
        //[RegularExpression("^-?[0-9]*\\.?[0-9]+$", ErrorMessage = "Corporate price should be decimal.")]
        public string CorporatePriceString { get; set; }
        //[RegularExpression("^-?[0-9]*\\.?[0-9]+$", ErrorMessage = "Corporate price should be decimal.")]
        public string DdpPriceString { get; set; }
        //[RegularExpression("^-?[0-9]*\\.?[0-9]+$", ErrorMessage = "Corporate price should be decimal.")]
        public string FobPriceString { get; set; }
        //[RegularExpression("^-?[0-9]*\\.?[0-9]+$", ErrorMessage = "Abroad desi should be decimal.")]
        public string AbroadDesiString { get; set; }
        //[RegularExpression("^-?[0-9]*\\.?[0-9]+$", ErrorMessage = "Desi should be decimal.")]
        public string DesiString { get; set; }
        public DateTime? ExpireDate { get; set; }

        public int? ParentProductId { get; set; }
        public int? CurrencyId { get; set; }
        public int? CorporateCurrencyId { get; set; }
        public int? PsfCurrencyId { get; set; }
        public int? DdpCurrencyId { get; set; }
        public int? FobCurrencyId { get; set; }
        public int? VatRateId { get; set; }
        public int? ProductGroupId { get; set; }
        public int? SupplierId { get; set; }
        public int? BrandId { get; set; }

        public virtual ParentProductModel ParentProduct { get; set; }
        public virtual CurrencyModel Currency { get; set; }
        public virtual CurrencyModel CorporateCurrency { get; set; }
        public virtual CurrencyModel PsfCurrency { get; set; }
        public virtual CurrencyModel DdpCurrency { get; set; }
        public virtual CurrencyModel FobCurrency { get; set; }
        public virtual VatRateModel VatRate { get; set; }
        public virtual ProductGroupModel ProductGroup { get; set; }
        public virtual SupplierModel Supplier { get; set; }
        public virtual BrandModel Brand { get; set; }

        public ICollection<PicturesModel> Pictures { get; set; }
        public ICollection<ProductAttributeMappingModel> ProductAttributeMappings { get; set; }
        public ICollection<LanguageValuesModel> LanguageValues { get; set; }
        public ICollection<ProductAttributeValueModel> ProductAttributeValues { get; set; }
        public ICollection<ProductModel> ProductVariants { get; set; }

        public string BrndName { get; set; }
        public string Category { get; set; }
        //public string SupplierName { get; set; }
        public string VatRateAmount { get; set; }
        public string CurrName { get; set; }
        public string PsfCurrName { get; set; }
        public string CorCurrName { get; set; }
        public string DdpCurrName { get; set; }
        public string FobCurrName { get; set; }
        public string Store { get; set; }
        public string StoreCategory { get; set; }
        public int StoreProductId { get; set; }
        public int CategoryId { get; set; }
        public string FirstPicturePath { get; set; }
        public string ParentProductName { get; set; }

        public int? ErpPoint { get; set; }
        public decimal? ErpPrice { get; set; }
        public bool? IsFixed { get; set; }
        public bool? IsFixedPoint { get; set; }
        public string CatalogCode { get; set; }
        public int? Point { get; set; }
    }
}
