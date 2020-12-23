using ProductDb.Common.Entities;
using System.ComponentModel.DataAnnotations;

namespace ProductDb.Data.BiggBrandsDb
{
    public class Permission: EntityBase
    {
        [Required]
        [StringLength(200)]
        public string name { get; set; }

        [Required]
        [StringLength(200)]
        public string key { get; set; }

        public string Description { get; set; }
    }
}
