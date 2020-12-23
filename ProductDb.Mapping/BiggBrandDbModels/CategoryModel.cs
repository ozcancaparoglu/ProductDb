using ProductDb.Common.Entities;
using System.ComponentModel.DataAnnotations;

namespace ProductDb.Mapping.BiggBrandDbModels
{
    public class CategoryModel : EntityBaseModel
    {
        [Required(ErrorMessage = "*This field is mandatory")]
        public string Name { get; set; }

        public int? ParentCategoryId { get; set; }

        public string CategoryNameWithParents { get; set; }

        public int SkuCount { get; set; }
    }
}