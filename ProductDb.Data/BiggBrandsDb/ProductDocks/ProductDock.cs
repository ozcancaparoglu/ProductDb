using ProductDb.Common.Entities;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductDb.Data.BiggBrandsDb.ProductDocks
{
    public class ProductDock : EntityBase
    {
        public string Name { get; set; }
        public string ShortDescription { get; set; }
        public string FullDescription { get; set; }
        public string MetaKeywords { get; set; }
        public string MetaDescription { get; set; }
        public string MetaTitle { get; set; }
        public string Sku { get; set; }
        public string Gtin { get; set; }
        public string Model { get; set; }
        public int? SupplierUniqueId { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? PsfPrice { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? Desi { get; set; }

        public int? ParentProductDockId { get; set; }
        [ForeignKey("ParentProductDockId")]
        public virtual ParentProductDock ParentProductDock { get; set; }

        public string Brand { get; set; }

        public int? SupplierId { get; set; }
        [ForeignKey("SupplierId")]
        public virtual Supplier Supplier{ get; set; }

        public int? ProductDockCategoryId { get; set; }
        [ForeignKey("ProductDockCategoryId")]
        public virtual ProductDockCategory ProductDockCategory { get; set; }

        public int? VatRateId { get; set; }
        [ForeignKey("VatRateId")]
        public virtual VatRate VatRate { get; set; }

        public int? CurrencyId { get; set; }
        [ForeignKey("CurrencyId")]
        public virtual Currency Currency { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Cost { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? CorporatePrice { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Weight { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Length { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Width { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Height { get; set; }

        public string BulletPoint1 { get; set; }
        public string BulletPoint2 { get; set; }
        public string BulletPoint3 { get; set; }
        public string BulletPoint4 { get; set; }
        public string BulletPoint5 { get; set; }
       
        public int Stock { get; set; }

        public ICollection<ProductDockAttribute> Attributes { get; set; }
        public ICollection<ProductDockPictures> Pictures { get; set; }
    }
}
