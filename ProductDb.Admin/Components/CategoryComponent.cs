using Microsoft.AspNetCore.Mvc;
using ProductDb.Admin.PageModels.Category;
using ProductDb.Services.CategoryServices;
using ProductDb.Services.ProductDockServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductDb.Admin.Components
{
    [ViewComponent(Name = "Category")]
    public class CategoryComponent : ViewComponent
    {
        private ICategoryService categoryService;

        public CategoryComponent(ICategoryService categoryService)
        {
            this.categoryService = categoryService;
        }
        public IViewComponentResult Invoke(int supplierId,int? productDockCategoryId)
        {
            var catViewModel = new ProductDockCategoryViewModel
            {
                Categories = categoryService.AllActiveCategoriesWithParentNames().ToList(),
                ProductDockCategoryId = productDockCategoryId ?? 0,
                SupplierId = supplierId,
                CategoryId = 0
            };
            return View("_Category", catViewModel);
        }
    }
}
