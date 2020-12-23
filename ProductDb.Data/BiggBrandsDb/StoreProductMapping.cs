using ProductDb.Common.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductDb.Data.BiggBrandsDb
{
    public class StoreProductMapping : EntityBase
    {
        public int? StoreId { get; set; }
        
        [ForeignKey("StoreId")]
        public virtual Store Store { get; set; }

        public int? ProductId { get; set; }

        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }

        public int Stock { get; set; }

        public int StoreProductId { get; set; }

        public int StoreParentProductId { get; set; }

        public int BaseStoreId { get; set; }

        public string StoreCategory { get; set; }

        public bool IsRealStock { get; set; } = true;

        public bool? IsSend { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? Price { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? Point { get; set; }

        public bool? IsFixed { get; set; } = false;
        
        public bool? IsFixedPoint { get; set; } = false;

        [Column(TypeName = "decimal(18,2)")]
        public decimal? ErpPrice { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? ErpPoint { get; set; }

        public string CatalogCode { get; set; }
        
        public string CatalogName { get; set; }

        public int? VatValue { get; set; }
    }
}
