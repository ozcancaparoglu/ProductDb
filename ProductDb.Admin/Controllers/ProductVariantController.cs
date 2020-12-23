using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductDb.Admin.Helpers.LocalizationHelpers;
using ProductDb.Admin.PageModels.ProductVariant;
using ProductDb.Mapping.BiggBrandDbModels;
using ProductDb.Services.AttributesServices;
using ProductDb.Services.LanguageServices;
using ProductDb.Services.PermissionServices;
using ProductDb.Services.ProductServices;
using ProductDb.Services.ProductVariantServices;
using ProductDb.Services.StoreServices;
using System.Collections.Generic;
using System.Linq;

namespace ProductDb.Admin.Controllers
{
    [Authorize]
    [MiddlewareFilter(typeof(LocalizationPipeline))]
    [Route("{lang}/ProductVariant")]
    public class ProductVariantController : BaseController
    {
        private readonly IProductVariantService productVariantService;
        private readonly IParentProductService parentProductService;
        private readonly IAttributesService attributesService;
        private readonly IStoreService storeService;

        public ProductVariantController(ILanguageService languageService, IUserRolePermissionService userRolePermissionService, IStoreService storeService, IAttributesService attributesService,
            IParentProductService parentProductService, IProductVariantService productVariantService) : base(languageService, userRolePermissionService)
        {
            this.productVariantService = productVariantService;
            this.parentProductService = parentProductService;
            this.attributesService = attributesService;
            this.storeService = storeService;
        }

        [Route("Variant")]
        public IActionResult Variant()
        {
            var model = new ProductVariantViewModel
            {
                Attributes = attributesService.AllVariantableAttributes().ToList(),
                ParentProducts = parentProductService.AllParentProducts().ToList()
            };

            model.ParentProducts = model.ParentProducts.Where(x => x.Products.Count > 1).ToList();

            List<ParentProductModel> productList = new List<ParentProductModel>();
            foreach (var item in model.ParentProducts)
            {
                productList.Add(new ParentProductModel
                {
                    Sku = $"{item.Sku} / {item.Title}",
                    Id = item.Id
                });
            }

            model.ParentProducts = productList;
            return View(model);
        }
        // new
        [Route("ParentsProducts/{id}")]
        [HttpPost]
        public IActionResult ParentsProducts(int id, List<int> IDs)
        {
            try
            {
                var products = productVariantService.GetParentProductsVariantById(id, IDs);
                return Json(products);
            }
            catch (System.Exception ex)
            {
                return Json(new { message = ex.Message, data = "" });
            }
        }

        [Route("ProductVariants/{id}")]
        [HttpPost]
        public IActionResult ProductVariants(int id, List<int> IDs)
        {
            try
            {
                var products = productVariantService.GetProductsVariant(id, IDs);
                return Json(products);
            }
            catch (System.Exception ex)
            {
                return Json(new { message = ex.Message, data = "" });
            }
        }
        [Route("ClearProductVariants/{id}")]
        public IActionResult ClearProductVariants(int id)
        {
            try
            {
                productVariantService.ClearProductVariants(id);
                return Json(new { status = true, message = "Variants Cleared" });
            }
            catch (System.Exception ex)
            {
                return Json(new { status = false, message = ex.Message });
            }
        }

        [Route("GetVariantedAttributesName/{id}")]
        public IActionResult GetVariantedAttributesName(int id)
        {
            try
            {
                var attributes = productVariantService.GetProductsVariantsAttribute(id);
                return Json(new { status = true, message = attributes });
            }
            catch (System.Exception ex)
            {
                return Json(new { status = false, message = ex.Message });
            }
        }


        [Route("SaveProductVariant")]
        [HttpPost]
        public IActionResult SaveProductVariant(int baseId, int parentProductId, List<int> selectedProdIds, List<int> IDs)
        {
            try
            {
                var list = new List<ProductVariantModel>();
                var preparedIDs = productVariantService.PrepareAttributes(IDs);
                var isBaseAdded = productVariantService.ProductVariantByBaseId(baseId);

                if (isBaseAdded == null)
                {
                    // base prod
                    list.Add(new ProductVariantModel
                    {
                        BaseId = null,
                        ProductId = baseId,
                        ProductAttributes = preparedIDs,
                        ParentProductId = parentProductId
                    });
                }

                // variant list
                foreach (var item in selectedProdIds)
                {
                    list.Add(new ProductVariantModel
                    {
                        IDs = IDs,
                        BaseId = baseId,
                        ParentProductId = parentProductId,
                        ProductId = item,
                        ProductAttributes = preparedIDs
                    });
                }
                productVariantService.Add(list);

                return Json(new { data = "", message = "OK" });
            }
            catch (System.Exception ex)
            {
                return Json(new { data = "", message = ex.Message });
            }
        }

        [Route("GetNotVariantedProducts")]
        [HttpPost]
        public IActionResult GetNotVariantedProducts(int id, int parentProductId, List<int> IDs)
        {
            // Not Varianted Products
            var result = ViewComponent("AddProductVariant", new { id = id, parentProductId = parentProductId, IDs = IDs });
            return result;
        }

        [Route("NotVariantedProductList")]
        [HttpPost]
        public IActionResult NotVariantedProductList(int id, int parentProductId, List<int> IDs)
        {
            try
            {
                var products = productVariantService.GetParentProductsNotVariantById(parentProductId, IDs)
                    .Where(x => x.Id != id);

                return Json(products);
            }
            catch (System.Exception ex)
            {
                return Json(new { message = ex.Message, data = "" });
            }
        }

        [Route("ChangeProductVariant/{id}")]
        public IActionResult ChangeProductVariant(int id)
        {
            // Not Varianted Products
            var result = ViewComponent("ChangeProductVariant", new { id = id });
            return result;
        }

        [Route("SaveChangeProductVariant")]
        [HttpPost]
        public IActionResult SaveChangeProductVariant(int baseId, int productId)
        {
            try
            {
                productVariantService.ChangeProductVariant(baseId, productId);
                return Json(new { status = true, message = "Ürün Çıkartıldı" });
            }
            catch (System.Exception ex)
            {
                return Json(new { status = false, message = ex.Message });
            }
        }

    }
}