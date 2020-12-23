using Microsoft.AspNetCore.Mvc;
using ProductDb.Admin.PageModels.Store;
using ProductDb.Services.CategoryServices;
using ProductDb.Services.StoreServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductDb.Admin.Components
{
    [ViewComponent(Name = "UpdateStoreCategory")]
    public class UpdateStoreCategoryComponent: ViewComponent
    {
        private readonly ICategoryService categoryService;
        private readonly IStoreService storeService;

        public UpdateStoreCategoryComponent(IStoreService storeService, ICategoryService categoryService)
        {
            this.categoryService = categoryService;
            this.storeService = storeService;
        }
        public IViewComponentResult Invoke(int id)
        {
            var datas = storeService.StoreCategoryByStoreId(id);
            var viewModel = new StoreCategoryUpdateViewModel
            {
                StoreCategoryMappings = datas,
                StoreId = id,
                Categories = categoryService.AllActiveCategoriesWithParentNames().ToList()
            };

            return View("_UpdateStoreCategory", viewModel);
        }
    }
}
