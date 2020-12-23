using ProductDb.Common.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductDb.Data.BiggBrandsDb
{
    public class Cargo : EntityBase
    {
        public int? StoreId { get; set; }
        [ForeignKey("StoreId")]
        public virtual Store Store { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal MinDesi { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal MaxDesi { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Value { get; set; }

        public bool IsLastDesi { get; set; }

    }
}
