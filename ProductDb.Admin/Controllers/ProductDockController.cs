using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductDb.Admin.Helpers.LocalizationHelpers;
using ProductDb.Admin.PageModels.ProductDock;
using ProductDb.Mapping.BiggBrandDbModels;
using ProductDb.Mapping.BiggBrandDbModels.ProductDocksMapping;
using ProductDb.Services.BrandServices;
using ProductDb.Services.CategoryServices;
using ProductDb.Services.LanguageServices;
using ProductDb.Services.PermissionServices;
using ProductDb.Services.ProductDockServices;
using ProductDb.Services.ProductServices;
using ProductDb.Services.SupplierServices;
using ProductDb.Services.TaxServices;
using System.Collections.Generic;
using System.Linq;

namespace ProductDb.Admin.Controllers
{
    [MiddlewareFilter(typeof(LocalizationPipeline))]
    [AllowAnonymous]
    [Route("{lang}/product-dock")]
    public class ProductDockController : BaseController
    {
        private readonly ITaxService taxService;
        private readonly IProductDockService productDockService;
        private readonly IProductService productService;
        private readonly IParentProductService parentProductService;
        private readonly ISupplierService supplierService;
        private readonly IBrandService brandService;
        private readonly ICategoryService categoryService;


        public ProductDockController(ILanguageService languageService,
            IUserRolePermissionService userRolePermissionService,
            IProductDockService productDockService,
            IProductService productService,
            IParentProductService parentProductService,
            ISupplierService supplierService,
            IBrandService brandService,
            ICategoryService categoryService,
            ITaxService taxService)
            : base(languageService, userRolePermissionService)
        {
            this.taxService = taxService;
            this.productDockService = productDockService;
            this.productService = productService;
            this.parentProductService = parentProductService;
            this.brandService = brandService;
            this.categoryService = categoryService;
            this.supplierService = supplierService;
        }

        [Route("home/{page}")]
        public IActionResult List(int? page)
        {
            //if (page.HasValue)
            //    productDockService.NopProducts(page.Value, 100);

            return View();
        }

        [Route("nop-update")]
        public IActionResult NopUpdate()
        {
            productService.UpdateProductImport("", 1);
            return View();
        }

        [Route("list-product-dock/{sku}")]
        [HttpGet]
        public IActionResult ProductDockList(string sku)
        {
            var model = new ProductDockViewModel
            {
                ProductDocks = productDockService.AllProducts(sku, 1, 100).ToList(),
                ParentProducts = parentProductService.AllParentProducts(),
                Categories = categoryService.AllCategoriesWithParentNames(),
                Suppliers = supplierService.AllSuppliers(),
                Brands = brandService.AllBrands()
            };

            return View(model);
        }

        [Route("insert-productdock-with-parent")]
        [HttpPost]
        public JsonResult InsertProductDocksWithParent(int? parentProductDockId, List<int> dockIds)
        {
            if (!parentProductDockId.HasValue)
                return Json("Error");

            try
            {
                foreach (var dockId in dockIds)
                {
                    var productDocks = productDockService.GetProductDockbyId(dockId);
                    productService.AddNewProductByProductDock(productDocks, parentProductDockId.Value);
                }

                return Json(1);
            }
            catch
            {
                return Json("Error");
            }
        }

        [Route("not-insert-product/{id}")]
        public JsonResult NotInsertProduct(int id)
        {
            if(productService.DeleteProductDock(id))
                return Json(1);
            return Json("Error");
        }

        [Route("add-product")]
        public JsonResult AddProduct(int id, int? parentId, int? brand, int? categories, int? supplier)
        {
            if (parentId == null)
            {
                if (brand == null)
                    return Json("Brand Empty Please Fill in the Blanks");
                if (categories == null)
                    return Json("Category Empty Please Fill in the Blanks");
                if (supplier == null)
                    return Json("Supplier Empty Please Fill in the Blanks");

                try
                {
                    var productDocks = productDockService.GetProductDockbyId(id);
                    var parentProduct = parentProductService.AddNewParentProduct(new ParentProductModel()
                    {
                        Title = productDocks.Name,
                        Sku = $"{productDocks.Sku}_PP",
                        Description = productDocks.FullDescription,
                        ShortDescription = productDocks.ShortDescription,
                        CategoryId = categories,
                        SupplierId = supplier,
                        BrandId = brand
                    });

                    if (productService.AddNewProductByProductDock(productDocks, parentProduct.Id))
                        return Json(1);
                    else
                        return Json("Error");
                }
                catch (System.Exception)
                {
                    return Json("Error");
                }
            }
            else
            {
                var productDocks = productDockService.GetProductDockbyId(id);

                if (productService.AddNewProductByProductDock(productDocks, parentId.Value))
                    return Json(1);
                else
                    return Json("Error");
            }
        }

        [Route("product-dock-list-suppliers")]
        public IActionResult ProductDockBySupplierList()
        {
            var data = new ProductDockBySupplierViewModel()
            {
                Suppliers = supplierService.AllActiveSuppliers(),
                ProductDockCategories = productDockService.ProductDockCategoriesBySupplierId(),
                ProductDocks = new List<ProductDockModel>(),
                Categories = categoryService.AllCategoriesWithParentNames()
            };

            return View(data);
        }

        [Route("product-dock-list-suppliers")]
        [HttpPost]
        public IActionResult ProductDockBySupplierList(ProductDockBySupplierViewModel model)
        {
            model.Suppliers = supplierService.AllActiveSuppliers();
            model.ProductDockCategories = productDockService.ProductDockCategoriesBySupplierId(model.SupplierId);

            model.ProductDocks = productDockService.ProductDocksBySupplierAndCategoryId(model.SupplierId,
                     model.ProductDockCategoryId);

            model.Categories = categoryService.AllCategoriesWithParentNames();

            return View(model);
        }

        [HttpGet]
        [Route("ProductDockCategoryBySupplierId/{id}")]
        public IActionResult ProductDockCategoryBySupplierId(int id)
        {
            try
            {
                var data = productDockService.ProductDockCategoriesBySupplierId(id);
                return Json(new { Status = true, data, Message = "OK" });
            }
            catch (System.Exception ex)
            {
                return Json(new { Status = false, Message = ex.Message.ToString() });
            }
        }

        [HttpPost]
        [Route("dock-edit/{id}")]
        public JsonResult Edit(ProductDockModel productDock)
        {
            try
            {
                productDockService.Edit(productDock);
                return Json(new { message = "Güncelleme İşlemi Başarılı", status = true });
            }
            catch (System.Exception ex)
            {
                return Json(new { message = ex.Message, status = false });
            }
        }

        [HttpGet]
        [Route("dock-edit/notvariantedproductdocks/{id}")]
        public IActionResult NotVariantedProductDocks(int id)
        {
            var result = ViewComponent("AddProductDockVariant", new { id = id });
            return result;
        }

        [HttpGet]
        [Route("GetNotVariantedProductDocks/{id}")]
        public JsonResult GetNotVariantedProductDocks(int id)
        {
            try
            {
                return Json(productDockService.NotVariantedProductDocks(id));
            }
            catch (System.Exception ex)
            {
                return Json(ex.Message);
            }
        }

        [HttpPost]
        [Route("SaveProductVariant/{parentId}")]
        public JsonResult SaveProductVariant(int parentId, List<int> selectedIDs)
        {
            try
            {
                var datas = productDockService.ProductDockByIDs(selectedIDs).ToList();
                productDockService.SaveProductVariant(datas, parentId);
                return Json(new { Message = "OK", Status = true });
            }
            catch (System.Exception ex)
            {
                return Json(new { ex.Message, Status = false });
            }
        }


        [Route("ChangeProductParent")]
        public JsonResult ChangeProductParent(int id, int parentId)
        {
            try
            {
                productDockService.ChangeProductParent(id, parentId);
                return Json(new { Status = true, Message = "OK" });
            }
            catch (System.Exception ex)
            {
                return Json(new { Status = false, ex.Message });
            }
        }

        [HttpGet]
        [Route("dock-edit/{id}")]
        public IActionResult Edit(int id)
        {
            var result = ViewComponent("ProductDock", new { id = id });
            return result;
        }

        [HttpGet]
        [Route("get-product-category/{supplierId}/{productDockCategoryId?}")]
        public IActionResult ProductCategories(int supplierId, int? productDockCategoryId)
        {
            var result = ViewComponent("Category", new { supplierId, productDockCategoryId });
            return result;
        }

        [HttpPost]
        [Route("saveallproductdock")]
        public IActionResult SaveAllProductDock(int selecetedCatId, int productDockCategoryId, int supplierId)
        {
            try
            {
                productDockService.SaveAllProductDock(selecetedCatId,productDockCategoryId,supplierId);
                return Json(new { message = "OK", status = true });
            }
            catch (System.Exception ex)
            {
                return Json(new { message = ex.Message, status = false });
            }
        }

    }
}