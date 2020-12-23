using ProductDb.Common.Entities;
using System.ComponentModel.DataAnnotations;

namespace ProductDb.Data.BiggBrandsDb
{
    public class Brand : EntityBase
    {
        [Required]
        [StringLength(250)]
        public string Name { get; set; }

        [StringLength(10)]
        public string Prefix { get; set; }
    }
}