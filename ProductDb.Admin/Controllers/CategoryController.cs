using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductDb.Admin.PageModels.Category;
using ProductDb.Admin.PageModels.Language;
using ProductDb.Mapping.BiggBrandDbModels;
using ProductDb.Mapping.BiggBrandDbModelFields;
using ProductDb.Services.CategoryServices;
using ProductDb.Services.LanguageServices;
using System.Linq;
using ProductDb.Services.PermissionServices;
using ProductDb.Admin.Helpers.LocalizationHelpers;

namespace ProductDb.Admin.Controllers
{
    [MiddlewareFilter(typeof(LocalizationPipeline))]
    [Route("{lang}/category")]
    [Authorize]
    public class CategoryController : BaseController
    {
        private readonly ICategoryService categoryService;

        public CategoryController(ILanguageService languageService, ICategoryService categoryService, IUserRolePermissionService userRolePermissionService) : base(languageService, userRolePermissionService)
        {
            this.categoryService = categoryService;
        }

        [Route("list")]
        public IActionResult List()
        {
            var model = categoryService.AllCategoriesWithParentNames();
            return View(model);
        }

        [Route("create")]
        public IActionResult Create()
        {
            var model = new CategoryViewModel
            {
                Category = new CategoryModel(),
                ParentCategories = categoryService.AllCategoriesWithParentNames()
            };

            ViewBag.TableName = GetFieldValues.CategoryTable;
            ViewBag.FieldNames = GetFieldValues.CategoryFields;

            return View(model);
        }

        [Route("create")]
        [HttpPost]
        public IActionResult Create(CategoryViewModel model, LanguageViewModel languageViewModel)
        {
            var entity = categoryService.AddNewCategory(model.Category);

            languageService.AddNewLanguageValues(languageViewModel.LanguageValues, entity.Id);

            return RedirectToAction("list", new { lang = CurrentLanguage });
        }

        [Route("edit/{id}")]
        public IActionResult Edit(int id)
        {
            var model = new CategoryViewModel
            {
                Category = categoryService.CategoryById(id),
                ParentCategories = categoryService.AllCategoriesWithParentNamesExceptSelf(id),
                ParentCategoryTree = categoryService.CategoryWithParents(id),
                CategoryParentAttributes = categoryService.CategoryParentAttributes(id),
                CategoryAttributes = categoryService.CategoryAttributes(id).ToList()
            };

            ViewBag.TableName = GetFieldValues.CategoryTable;
            ViewBag.FieldNames = GetFieldValues.CategoryFields;

            return View(model);
        }

        [Route("edit/{id}")]
        [HttpPost]
        public IActionResult Edit(CategoryViewModel model, LanguageViewModel languageViewModel)
        {
            var entity = categoryService.EditCategory(model.Category);

            languageService.EditLanguageValues(languageViewModel.LanguageValues, entity.Id, GetFieldValues.CategoryTable);

            categoryService.EditCategoryAttributeMappings(model.CategoryAttributes);

            return RedirectToAction("edit", new { lang = CurrentLanguage, id = entity.Id });
        }

        [Route("delete/{id}")]
        public IActionResult Delete(int id)
        {
            var res = categoryService.DeleteCategoryById(id);

            return RedirectToAction("list", new { lang = CurrentLanguage }); 
        }

        [Route("delete-attribute/{id}")]
        public IActionResult DeleteCategoryAttribute(int id)
        {
            var categoryId = categoryService.DeleteCategoryAttributeMappingWithIdReturnCategoryId(id);

            return RedirectToAction("edit", new { lang = CurrentLanguage, id = categoryId });

        }
    }
}