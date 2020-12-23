using ProductDb.Common.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductDb.Data.BiggBrandsDb
{
    public class Product : EntityBase
    {
        [Required]
        public string Sku { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Barcode { get; set; }

        public string Gtip { get; set; }

        public string Description { get; set; }

        public string Name { get; set; }

        public string Model { get; set; }

        public string ShortDescription { get; set; }

        public string Alias { get; set; }

        public string MetaKeywords { get; set; }

        public string MetaTitle { get; set; }

        public string MetaDescription { get; set; }

        public string SearchEngineTerms { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? Desi { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? AbroadDesi { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? BuyingPrice { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? PsfPrice { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? CorporatePrice { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? DdpPrice { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? FobPrice { get; set; }

        public DateTime? ExpireDate { get; set; }

        public int? ParentProductId { get; set; }
        public int? CurrencyId { get; set; }
        public int? VatRateId { get; set; }
        public int? SupplierUniqueId { get; set; }
        public int? ProductGroupId { get; set; }
        public int? CorporateCurrencyId { get; set; }
        public int? PsfCurrencyId { get; set; }
        public int? DdpCurrencyId { get; set; }
        public int? FobCurrencyId { get; set; }
        public int? SupplierId { get; set; }
        public int? BrandId { get; set; }

        [ForeignKey("BrandId")]
        public virtual Brand Brand { get; set; }

        [ForeignKey("SupplierId")]
        public virtual Supplier Supplier { get; set; }

        [ForeignKey("CurrencyId")]
        public virtual Currency Currency { get; set; }

        [ForeignKey("CorporateCurrencyId")]
        public virtual Currency CorporateCurrency { get; set; }

        [ForeignKey("PsfCurrencyId")]
        public virtual Currency PsfCurrency { get; set; }

        [ForeignKey("DdpCurrencyId")]
        public virtual Currency DdpCurrency { get; set; }

        [ForeignKey("FobCurrencyId")]
        public virtual Currency FobCurrency { get; set; }

        [ForeignKey("VatRateId")]
        public virtual VatRate VatRate { get; set; }

        [ForeignKey("ParentProductId")]
        public virtual ParentProduct ParentProduct { get; set; }

        [ForeignKey("ProductGroupId")]
        public virtual ProductGroup ProductGroup { get; set; }

        public ICollection<Pictures> Pictures { get; set; }
        public ICollection<ProductAttributeMapping> ProductAttributeMappings { get; set; }
    }
}
