using ProductDb.Mapping.BiggBrandDbModels;
using System.Collections.Generic;

namespace ProductDb.Services.CategoryServices
{
    public interface ICategoryService
    {
        ICollection<CategoryModel> AllCategories();
        ICollection<CategoryModel> AllActiveCategories();
        ICollection<CategoryModel> AllCategoriesExceptSelf(int id);
        ICollection<CategoryModel> AllCategoriesWithParentNames();
        ICollection<CategoryModel> AllCategoriesWithParentNamesExceptSelf(int id);
        ICollection<CategoryModel>  AllActiveCategoriesWithParentNames();
        CategoryModel CategoryById(int id);
        CategoryModel AddNewCategory(CategoryModel categoryModel);
        CategoryModel EditCategory(CategoryModel categoryModel);
        int DeleteCategoryById(int id);
        ICollection<CategoryModel> CategoryWithParents(int id);
        string CategoryWithParentNames(int id);
        string CategoryWithParentNamesAndLanguages(int id, int languageId);
        int CategoryCount();
        ICollection<CategoryModel> CategoryWithParentsAndLanguages(int id, int languageId);
        bool IsCategoryDefined(string name);
        #region CategoryAttributes

        ICollection<CategoryAttributeMappingModel> CategoryParentAttributes(int categoryId);
        ICollection<CategoryAttributeMappingModel> CategoryAttributes(int categoryId);
        CategoryAttributeMappingModel AddNewCategoryAttributeMapping(int attributeId, int categoryId, bool isRequired = false);
        CategoryAttributeMappingModel EditCategoryAttributeMapping(CategoryAttributeMappingModel model);
        bool EditCategoryAttributeMappings(List<CategoryAttributeMappingModel> categoryAttributeMappingModels);
        int DeleteCategoryAttributeMapping(CategoryAttributeMappingModel categoryAttributeMappingModel);
        int DeleteCategoryAttributeMappingWithIdReturnCategoryId(int categoryAttributeMappingId);

        #endregion
    }
}