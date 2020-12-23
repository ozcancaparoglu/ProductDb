using ProductDb.Common.Entities;
using System.ComponentModel.DataAnnotations;

namespace ProductDb.Data.BiggBrandsDb
{
    public class Category : EntityBase
    {
        [Required]
        [StringLength(250)]
        public string Name { get; set; }
        public int? ParentCategoryId { get; set; }
    }
}