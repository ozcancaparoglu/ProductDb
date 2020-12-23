using ProductDb.Common.Entities;
using System.ComponentModel.DataAnnotations;

namespace ProductDb.Data.BiggBrandsDb
{
    public class UserRole : EntityBase
    {
        [Required]
        [StringLength(750)]
        public string Name { get; set; }
    }
}