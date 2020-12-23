using ProductDb.Common.Entities;
using System.ComponentModel.DataAnnotations;

namespace ProductDb.Data.BiggBrandsDb
{
    public class Language : EntityBase
    {
        [Required]
        [StringLength(250)]
        public string Name { get; set; }
        [Required]
        [StringLength(10)]
        public string Abbrevation { get; set; }
        [Required]
        public string LogoPath { get; set; }
        public bool IsDefault { get; set; }
    }
}
