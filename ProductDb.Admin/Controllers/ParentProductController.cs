using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductDb.Admin.Helpers.LocalizationHelpers;
using ProductDb.Admin.PageModels.Filter;
using ProductDb.Admin.PageModels.Language;
using ProductDb.Admin.PageModels.ParentProduct;
using ProductDb.Admin.PageModels.Product;
using ProductDb.Mapping.BiggBrandDbModelFields;
using ProductDb.Mapping.BiggBrandDbModels;
using ProductDb.Services.BrandServices;
using ProductDb.Services.CategoryServices;
using ProductDb.Services.LanguageServices;
using ProductDb.Services.PermissionServices;
using ProductDb.Services.ProductServices;
using ProductDb.Services.SupplierServices;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProductDb.Admin.Controllers
{
    [Authorize]
    [Route("{lang}/parent-products")]
    [MiddlewareFilter(typeof(LocalizationPipeline))]
    public class ParentProductController : BaseController
    {
        private readonly ICategoryService categoryService;
        private readonly IParentProductService parentProductService;
        private readonly ISupplierService supplierService;
        private readonly IBrandService brandService;
        private readonly IProductService productService;

        public ParentProductController(ILanguageService languageService,
            ICategoryService categoryService,
            IParentProductService parentProductService,
            ISupplierService supplierService,
            IBrandService brandService,
            IProductService productService,
            IUserRolePermissionService userRolePermissionService) : base(languageService, userRolePermissionService)
        {
            this.categoryService = categoryService;
            this.parentProductService = parentProductService;
            this.supplierService = supplierService;
            this.brandService = brandService;
            this.productService = productService;
        }

        [HttpPost]
        [Route("insert")]
        public JsonResult Insert(AddProductParentViewModel model)
        {
            if (model.Products == null || model.Products.Count(x => x.IsParentCheck) == 0)
                return Json(new { result = "Redirect" });
            try
            {
                productService.ChangeProductsParent(model.ParentProductId, model.Products.Where(p => p.IsParentCheck).Select(x => x.Id).ToList());

                return Json(new { result = "Redirect" });
            }
            catch (System.Exception)
            {
                return Json(new { result = "Failed", url = $"/{CurrentLanguage}/parent-products/list" });
            }
        }

        [Route("list/{id}")]
        public IActionResult List(int? id)
        {
            ViewBag.CategoryId = 0;

            if (id.HasValue)
                ViewBag.CategoryId = id;
           

            return View();
        }

        [HttpPost]
        [Route("get-all-parent-products/{id}")]
        public JsonResult GetAllParentProductsByOrder(int id, KendoFilterModel kendoFilterModel)
        {
            try
            {
                IEnumerable<ParentProductModel> data = null;
                int total;
                if (kendoFilterModel.filter == null)
                {
                    data = parentProductService.AllParentProductsQueryableAsync(id, kendoFilterModel.take, kendoFilterModel.skip, out total);
                }
                else
                {
                    var qDatas = parentProductService.AllParentProductQueryable(id);
                    var datas = KendoFilterHelper.FilterQueryableData(qDatas, kendoFilterModel);
                    total = datas.Count();
                    data = datas.Skip(kendoFilterModel.skip).Take(kendoFilterModel.take);
                }

                return Json(new { total, data });
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, status = false });
            }
        }

        [Route("create")]
        public IActionResult Create()
        {
            var model = new ParentProductViewModel()
            {
                ParentProduct = new ParentProductModel(),
                Categories = categoryService.AllActiveCategoriesWithParentNames(),
                Suppliers = supplierService.AllActiveSuppliers(),
                Brands = brandService.AllActiveBrands()
            };

            ViewBag.TableName = GetFieldValues.ParentProductTable;
            ViewBag.FieldNames = GetFieldValues.ParentProductFields;

            return View(model);
        }

        [Route("create")]
        [HttpPost]
        public IActionResult Create(ParentProductViewModel model, LanguageViewModel languageViewModel)
        {
            var entity = parentProductService.AddNewParentProduct(model.ParentProduct);

            languageService.AddNewLanguageValues(languageViewModel.LanguageValues, entity.Id);

            return RedirectToAction("list", new { lang = CurrentLanguage, id = 0 });
        }

        [Route("edit/{id}")]
        public IActionResult Edit(int id)
        {
            var model = new ParentProductViewModel
            {
                ParentProduct = parentProductService.ParentProductById(id),
                Categories = categoryService.AllActiveCategoriesWithParentNames(),
                Suppliers = supplierService.AllActiveSuppliers(),
                Brands = brandService.AllActiveBrands()
            };

            ViewBag.TableName = GetFieldValues.ParentProductTable;
            ViewBag.FieldNames = GetFieldValues.ParentProductFields;

            return View(model);
        }

        [Route("edit/{id}")]
        [HttpPost]
        public IActionResult Edit(ParentProductViewModel model, LanguageViewModel languageViewModel)
        {
            var entity = parentProductService.EditParentProduct(model.ParentProduct);

            languageService.EditLanguageValues(languageViewModel.LanguageValues, entity.Id, GetFieldValues.ParentProductTable);

            return RedirectToAction("edit", new { lang = CurrentLanguage, id = entity.Id });
        }

        [Route("delete/{id}")]
        public IActionResult Delete(int id)
        {
            parentProductService.DeleteParentProductById(id);

            return RedirectToAction("list", new { lang = CurrentLanguage, id = 0 });
        }

        [Route("GetParentProductDocks/{id}")]
        public IActionResult GetParentProductDocks(int id)
        {
            var result = ViewComponent("ChangeParentProductDock", new { id });
            return result;
        }

    }
}