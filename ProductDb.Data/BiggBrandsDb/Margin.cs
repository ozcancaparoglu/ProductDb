using ProductDb.Common.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductDb.Data.BiggBrandsDb
{
    public class Margin : EntityBase
    {
        public int? StoreId { get; set; }
        [ForeignKey("StoreId")]
        public virtual Store Store { get; set; }

        public int? MarginTypeId { get; set; }
        [ForeignKey("MarginTypeId")]
        public virtual MarginType MarginType { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Profit { get; set; }

        [Required]
        public int EntityId { get; set; }

        public int? SecondEntityId { get; set; }
    }
}
