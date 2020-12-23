using ProductDb.Common.Entities;
using System.ComponentModel.DataAnnotations;

namespace ProductDb.Data.BiggBrandsDb
{
    public class CargoType : EntityBase
    {
        [Required]
        [StringLength(250)]
        public string Name { get; set; }
    }
}
