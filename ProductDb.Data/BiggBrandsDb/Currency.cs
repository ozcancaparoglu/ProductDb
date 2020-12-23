using ProductDb.Common.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductDb.Data.BiggBrandsDb
{
    public class Currency : EntityBase
    {
        [Required]
        [StringLength(250)]
        public string Name { get; set; }

        [StringLength(10)]
        public string Abbrevation { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Value { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal LiveValue { get; set; }
    }
}
