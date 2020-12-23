using ProductDb.Common.Entities;
using System.ComponentModel.DataAnnotations;

namespace ProductDb.Mapping.BiggBrandDbModels
{
    public class FormulaModel : EntityBaseModel
    {
        public int? FormulaGroupId { get; set; }
        public virtual FormulaGroupModel FormulaGroup { get; set; }

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

        public string CalculatedResult { get; set; }

    }
}
