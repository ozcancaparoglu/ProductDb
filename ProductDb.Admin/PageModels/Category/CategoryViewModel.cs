using ProductDb.Mapping.BiggBrandDbModels;
using System.Collections.Generic;

namespace ProductDb.Admin.PageModels.Category
{
    public class CategoryViewModel
    {
        public CategoryModel Category { get; set; }
        public ICollection<CategoryModel> ParentCategories { get; set; }
        public ICollection<CategoryModel> ParentCategoryTree { get; set; }
        public ICollection<CategoryAttributeMappingModel> CategoryParentAttributes { get; set; }
        public List<CategoryAttributeMappingModel> CategoryAttributes { get; set; }
    }
}
