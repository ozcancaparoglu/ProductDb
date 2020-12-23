using ProductDb.Common.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductDb.Data.BiggBrandsDb
{
    public class Formula : EntityBase
    {
        public int? FormulaGroupId { get; set; }
        [ForeignKey("FormulaGroupId")]
        public virtual FormulaGroup FormulaGroup { get; set; }

        [Required]
        [StringLength(500)]
        public string Name { get; set; }
        
        [Required]
        [StringLength(1000)]
        public string FormulaStr { get; set; }
        
        [Required]
        public int Order { get; set; }

        [Required]
        [StringLength(10)]
        public string Result { get; set; }


    }
}
