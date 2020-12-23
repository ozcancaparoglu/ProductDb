using ProductDb.Common.Entities;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProductDb.Mapping.BiggBrandDbModels.ProductDocksMapping
{
    public class ProductDockModel : EntityBaseModel
    {
        public string Name { get; set; }
        public string ShortDescription { get; set; }
        public string FullDescription { get; set; }
        public string MetaKeywords { get; set; }
        public string MetaDescription { get; set; }
        public string MetaTitle { get; set; }
        public string Sku { get; set; }
        public string Gtin { get; set; }
        public decimal Cost { get; set; }
        public decimal? Weight { get; set; }
        public decimal? Length { get; set; }
        public decimal? Width { get; set; }
        public decimal? Height { get; set; }
        public string BulletPoint1 { get; set; }
        public string BulletPoint2 { get; set; }
        public string BulletPoint3 { get; set; }
        public string BulletPoint4 { get; set; }
        public string BulletPoint5 { get; set; }
        public string Model { get; set; }
        public decimal? PsfPrice { get; set; }
        public decimal? CorporatePrice { get; set; }
        public decimal? Desi { get; set; }
        public int? VatRateId { get; set; }
        public int? SupplierUniqueId { get; set; }
        public int? SupplierId { get; set; }
        public int? ParentProductDockId { get; set; }
        public bool IsParentProductDockChecked { get; set; }
        public int? ProductDockCategoryId { get; set; }
        public int Stock { get; set; }

        public string Brand { get; set; }

        public int? CurrencyId { get; set; }
        public virtual CurrencyModel Currency { get; set; }

        //[RegularExpression("^-?[0-9]*\\.?[0-9]+$", ErrorMessage = "PSF price should be decimal.")]
        public string PsfPriceString { get; set; }

        //[RegularExpression("^-?[0-9]*\\.?[0-9]+$", ErrorMessage = "Desi price should be decimal.")]
        public string DesiString { get; set; }

        //[RegularExpression("^-?[0-9]*\\.?[0-9]+$", ErrorMessage = "Cost should be decimal.")]
        //[Required(ErrorMessage = "Cost is Required")]
        public string CostString { get; set; }
        public virtual ParentProductDockModel ParentProductDock { get; set; }
        public virtual SupplierModel Supplier { get; set; }
        public virtual VatRateModel VatRate { get; set; }

        public ICollection<ProductDockAttributeModel> Attributes { get; set; }
        public ICollection<ProductDockCategoryModel> Categories { get; set; }
        public ICollection<ProductDockPicturesModel> Pictures { get; set; }
        public ICollection<VatRateModel> VatRates { get; set; }

        public bool IsChecked { get; set; }

    }
}
