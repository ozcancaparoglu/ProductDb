using ProductDb.Mapping.BiggBrandDbModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductDb.ExportApi.DTOs
{
    public class ProductDTO
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
        public string SupplierName { get; set; }
        public string Category { get; set; }
        public string Brand { get; set; }
        public string Currency { get; set; }
        public string ManufacturerPartNumber { get; set; }
        public int? Stock { get; set; }
        public decimal? SellingPrice { get; set; }
        public decimal? PSFPrice { get; set; }
        public int? ErpPoint { get; set; }
        public decimal? ErpPrice { get; set; }
        public bool? IsFixed { get; set; }
        public bool? IsFixedPoint { get; set; }
        public string CatalogCode { get; set; }
      
        public int? Point { get; set; }
        public int? LanguageId { get; set; }
        public string Store { get; set; }
        public string StoreCategory { get; set; }
        public int StoreProductId { get; set; }
        public string LanguageAbbrevation { get; set; }
        //public ICollection<PicturesModel> Pictures { get; set; }
        public ICollection<PictureDTO> Pictures { get; set; }
        public ICollection<ProductAttributeDTO> Attributes { get; set; }
        public List<ProductDTO> ProductVariants { get; set; }
        public int BaseId { get; set; }
        public int TaxRate { get; set; }
        public string[] ProductVariantAttr { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
