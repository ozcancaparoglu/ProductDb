using ProductDb.Common.Entities;
using System.ComponentModel.DataAnnotations;

namespace ProductDb.Data.BiggBrandsDb
{
    public class FormulaGroup : EntityBase
    {
        [Required]
        [StringLength(500)]
        public string Name { get; set; }
    }
}
