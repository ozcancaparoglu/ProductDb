using System;
using System.Collections.Generic;
using System.Text;

namespace ProductDb.Common.GlobalEntity
{
    [Serializable]
    public class ProductJsonModel
    {
        public object ShallowCopy()
        {
            return this.MemberwiseClone();
        }
        public int Id { get; set; }
        public string Sku { get; set; }
        public string Gtip { get; set; }
        public string Barcode { get; set; }
        public string Title { get; set; }
        public string Name { get; set; }
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
        public string ManufacturerPartNumber { get; set; }
        public int? Stock { get; set; }
        public decimal? SellingPrice { get; set; }
        public decimal? PSFPrice { get; set; }
        public int? LanguageId { get; set; }
        public string Store { get; set; }
        public string StoreCategory { get; set; }
        public int StoreProductId { get; set; }
        //public ICollection<PicturesModel> Pictures { get; set; }
        public ICollection<PictureJsonModel> Pictures { get; set; }
        public ICollection<ProductAttributeJsonModel> Attributes { get; set; }
        public List<ProductJsonModel> ProductVariants { get; set; }
        public int BaseId { get; set; }
        public int TaxRate { get; set; }
        public string[] ProductVariantAttr { get; set; }
    }

    public class PictureJsonModel
    {
        //public int? ProductId { get; set; }
        //public string LocalPath { get; set; }
        public string CdnPath { get; set; }
        //public string Alt { get; set; }
        public string Title { get; set; }
        public int Order { get; set; }
        //public string Sku { get; set; }
    }

    public class ProductAttributeJsonModel
    {
        //public int EntityId { get; set; }
        //public string TableName { get; set; }
        public string FieldName { get; set; }
        //public int? LanguageId { get; set; }
        public string Value { get; set; }
        public string Unit { get; set; }
    }
}
