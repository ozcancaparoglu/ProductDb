using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PMS.Common.Helpers;
using ProductDb.Admin.Helpers.LocalizationHelpers;
using ProductDb.Admin.PageModels.Filter;
using ProductDb.Admin.PageModels.Language;
using ProductDb.Admin.PageModels.ParentProduct;
using ProductDb.Admin.PageModels.Product;
using ProductDb.Mapping.BiggBrandDbModelFields;
using ProductDb.Mapping.BiggBrandDbModels;
using ProductDb.Services.AttributesServices;
using ProductDb.Services.BrandServices;
using ProductDb.Services.CategoryServices;
using ProductDb.Services.CurrencyServices;
using ProductDb.Services.ErpComanyService;
using ProductDb.Services.LanguageServices;
using ProductDb.Services.PermissionServices;
using ProductDb.Services.PictureServices;
using ProductDb.Services.ProductServices;
using ProductDb.Services.StoreServices;
using ProductDb.Services.SupplierServices;
using ProductDb.Services.TaxServices;
using ProductDb.Services.WarehouseServices;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace ProductDb.Admin.Controllers
{
    [Authorize]
    [Route("{lang}/products")]
    [MiddlewareFilter(typeof(LocalizationPipeline))]
    public class ProductController : BaseController
    {
        private readonly IProductService productService;
        private readonly ITaxService taxService;
        private readonly IWarehouseService warehouseService;
        private readonly ICategoryService categoryService;
        private readonly IParentProductService parentProductService;
        private readonly IAttributesService attributesService;
        private readonly IPictureService pictureService;
        private readonly IBrandService brandService;
        private readonly ICurrencyService currencyService;
        private readonly IStoreService storeService;
        private readonly IErpCompanyService erpCompanyService;
        private readonly ISupplierService supplierService;

        public ProductController(ILanguageService languageService,
            ICategoryService categoryService,
            IProductService productService,
            IParentProductService parentProductService,
            IAttributesService attributesService,
            IWarehouseService warehouseService,
            IPictureService pictureService,
            IUserRolePermissionService userRolePermissionService,
            IBrandService brandService, ICurrencyService currencyService,
            ITaxService taxService, IStoreService storeService, IErpCompanyService erpCompanyService, ISupplierService supplierService) : base(languageService, userRolePermissionService)
        {
            this.taxService = taxService;
            this.warehouseService = warehouseService;
            this.categoryService = categoryService;
            this.productService = productService;
            this.parentProductService = parentProductService;
            this.attributesService = attributesService;
            this.pictureService = pictureService;
            this.brandService = brandService;
            this.currencyService = currencyService;
            this.storeService = storeService;
            this.erpCompanyService = erpCompanyService;
            this.supplierService = supplierService;
        }

        [HttpPost]
        [Route("insertStoreProduct")]
        public JsonResult Insert(List<int> storeIds, List<int> productIds)
        {
            try
            {
                foreach (var storeId in storeIds)
                {
                    storeService.AddNewStoreProductMappingRange(storeId, productIds);
                }
                return Json(new { success = true, url = $"/{CurrentLanguage}/products/list" });
            }
            catch (Exception)
            {
                return Json(new { success = false });
            }
        }

        [HttpPost]
        [Route("deleteStoreProduct")]
        public JsonResult Remove(List<int> storeIds, List<int> productIds)
        {
            try
            {
                foreach (var storeId in storeIds)
                {
                    storeService.RemoveFromStoreProductMappingRange(storeId, productIds);
                }
                return Json(new { success = true, url = $"/{CurrentLanguage}/products/list" });
            }
            catch (Exception)
            {
                return Json(new { success = false });
            }
        }

        [HttpPost]
        [Route("get-all-stores")]
        public JsonResult GetAllStores(KendoFilterModel kendoFilterModel)
        {
            try
            {
                IEnumerable<StoreModel> data = null;
                int total;
                if (kendoFilterModel.filter == null)
                {
                    data = storeService.AllStoresQuerable(out total);
                }
                else
                {
                    var qDatas = storeService.AllStoresQuerable(out total);
                    var datas = KendoFilterHelper.FilterQueryableData(qDatas, kendoFilterModel);
                    data = datas.Skip(kendoFilterModel.skip).Take(kendoFilterModel.take);
                }

                return Json(new { totalStore = total, data });
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, status = false });
            }
        }

        [HttpPost]
        [Route("get-all-products")]
        public JsonResult GetAllProductsByOrder(KendoFilterModel kendoFilterModel)
        {
            try
            {
                IEnumerable<ProductModel> data = null;
                int total;
                if (kendoFilterModel.filter == null)
                {
                    data = productService.AllProductsQueryableAsync(languageService.GetDefaultLanguage().Id, kendoFilterModel.take, kendoFilterModel.skip, out total);
                }
                else
                {
                    var qDatas = productService.AllProductQueryable();
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

        [Route("list")]
        public IActionResult List()
        {
            var model = new ParentProductSearchView()
            {
                ParentProducts = parentProductService.AllActiveParentProducts().ToList()
            };

            return View(model);
        }

        [Route("edit-parent")]
        [HttpPost]
        public JsonResult EditParent(int productId, int parentProductId)
        {
            try
            {
                productService.EditProductParent(productId, parentProductId);

                return Json(new { success = true, url = $"/{CurrentLanguage}/products/list/" });
            }
            catch (Exception)
            {
                return Json(new { success = false });
            }
        }

        [Route("add-parent")]
        [HttpPost]
        public JsonResult AddParent(int parentProductId)
        {
            try
            {
                return Json(new { success = true, url = $"/{CurrentLanguage}/products/create/{parentProductId}" });
            }
            catch (Exception)
            {
                return Json(new { success = false });
            }
        }

        [Route("create/{id}")]
        public IActionResult Create(int id, bool validation = true)
        {
            ViewBag.Validation = validation;
            var parentProduct = parentProductService.ParentProductById(id);

            var model = new ProductViewModel()
            {
                ParentProduct = parentProduct,
                Product = new ProductModel()
                {
                    ProcessedBy = _userId,
                    Description = parentProduct.Description,
                    ShortDescription = parentProduct.ShortDescription,
                    SupplierId = parentProduct.SupplierId,
                    BrandId = parentProduct.BrandId,
                    ExpireDate = null,
                },
                ProductCategoryAttributes = productService.ArrangeCategoryProductAttributes(id, null).ToList(),
                ProductRequiredAttributes = productService.ArrangeRequiredProductAttributes(null).ToList(),
                Currencies = currencyService.AllActiveCurrencies().ToList(),
                CategoryName = categoryService.CategoryWithParentNames(parentProduct.CategoryId.Value),
                VatRates = taxService.AllTaxRate().ToList(),
                ProductGroups = productService.ProductGroups().ToList(),
                Suppliers = supplierService.AllActiveSuppliers().ToList(),
                Brands = brandService.AllActiveBrands().ToList()
            };

            ViewBag.TableName = GetFieldValues.ProductTable;
            ViewBag.FieldNames = GetFieldValues.ProductFields;

            var productAttributeMappingFieldList = (from fieldName in model.ProductRequiredAttributes
                                                    select fieldName.Attributes.Name).ToList();

            ViewBag.RequiredAttributeTableName = GetFieldValues.ProductAttributeMappingTable;
            ViewBag.RequiredAttributeFieldNames = productAttributeMappingFieldList;

            return View(model);
        }

        [Route("create/{id}")]
        [HttpPost]
        public IActionResult Create(ProductViewModel model, LanguageViewModel languageViewModel, LanguageViewAttributeModel languageViewAttributeModel)
        {
            if (!Validation(model))
                return RedirectToAction("create", new { id = model.ParentProduct.Id, validation = false });

            model.Product.BuyingPrice = string.IsNullOrWhiteSpace(model.Product.BuyingPriceString) ? 0 : decimal.Parse(model.Product.BuyingPriceString, NumberStyles.Currency, new CultureInfo("tr-TR"));
            model.Product.AbroadDesi = string.IsNullOrWhiteSpace(model.Product.AbroadDesiString) ? 0 : decimal.Parse(model.Product.AbroadDesiString, NumberStyles.Currency, new CultureInfo("tr-TR"));
            model.Product.Desi = string.IsNullOrWhiteSpace(model.Product.DesiString) ? 0 : decimal.Parse(model.Product.DesiString, NumberStyles.Currency, new CultureInfo("tr-TR"));
            model.Product.PsfPrice = string.IsNullOrWhiteSpace(model.Product.PsfPriceString) ? 0 : decimal.Parse(model.Product.PsfPriceString, NumberStyles.Currency, new CultureInfo("tr-TR"));
            model.Product.CorporatePrice = string.IsNullOrWhiteSpace(model.Product.CorporatePriceString) ? 0 : decimal.Parse(model.Product.CorporatePriceString, NumberStyles.Currency, new CultureInfo("tr-TR"));
            model.Product.DdpPrice = string.IsNullOrWhiteSpace(model.Product.DdpPriceString) ? 0 : decimal.Parse(model.Product.DdpPriceString, NumberStyles.Currency, new CultureInfo("tr-TR"));
            model.Product.FobPrice = string.IsNullOrWhiteSpace(model.Product.FobPriceString) ? 0 : decimal.Parse(model.Product.FobPriceString, NumberStyles.Currency, new CultureInfo("tr-TR"));
            model.Product.ParentProductId = model.ParentProduct.Id;
            model.Product.ProductGroupId = model.Product.ProductGroupId;

            var entity = productService.AddNewProduct(model.Product);

            if (entity == null)
                return RedirectToAction("create", new { id = model.ParentProduct.Id, validation = false });

            languageService.AddNewLanguageValues(languageViewModel.LanguageValues, entity.Id);

            productService.AddRangeProductAttributeMapping(model.ProductCategoryAttributes, entity.Id);

            languageService.AddNewLanguageValues(languageViewAttributeModel.AttributeLanguageValues, entity.Id);

            productService.ArrangeProductAttributeValue(entity.Id);

            //TODO: Refactor - kanıttırır
            foreach (var requiredAttribute in model.ProductRequiredAttributes)
            {
                var productAttributeEntity = productService.AddNewProductAttributeMapping(entity.Id, requiredAttribute.AttributesId.Value, null, requiredAttribute.IsRequired, requiredAttribute.RequiredAttributeValue);

                foreach (KeyValuePair<int, List<LanguageValuesModel>> kvp in languageViewAttributeModel.AttributeLanguageValues)
                {
                    foreach (var item in kvp.Value)
                    {
                        if (item.FieldName == attributesService.AttributeNameById(productAttributeEntity.AttributesId.Value))
                            item.EntityId = productAttributeEntity.Id;
                    }
                }
            }

            warehouseService.AddNewProductStock(new WarehouseProductStockModel()
            {
                ProductId = entity.Id,
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now,
                Name = model.Product.Title,
                Quantity = 0,
                Sku = model.Product.Sku,
                Id = 0
            });

            return RedirectToAction("list", new { lang = CurrentLanguage });
        }

        [Route("edit/{id}")]
        public IActionResult Edit(int id, bool validation = true)
        {
            ViewBag.Validation = validation;

            var entity = productService.ProductById(id);

            entity.BuyingPriceString = entity.BuyingPrice != null ? entity.BuyingPrice.ToString() : 0.ToString();
            entity.AbroadDesiString = entity.AbroadDesi != null ? entity.AbroadDesi.ToString() : 0.ToString();
            entity.DesiString = entity.Desi != null ? entity.Desi.ToString() : 0.ToString();
            entity.PsfPriceString = entity.PsfPrice != null ? entity.PsfPrice.ToString() : 0.ToString();
            entity.CorporatePriceString = entity.CorporatePrice != null ? entity.CorporatePrice.ToString() : 0.ToString();
            entity.DdpPriceString = entity.DdpPrice != null ? entity.DdpPrice.ToString() : 0.ToString();
            entity.FobPriceString = entity.FobPrice != null ? entity.FobPrice.ToString() : 0.ToString();

            var parentProduct = parentProductService.ParentProductById(entity.ParentProductId.Value);

            var model = new ProductViewModel()
            {
                Product = entity,
                ParentProduct = parentProduct,
                ProductCategoryAttributes = productService.ArrangeCategoryProductAttributes(parentProduct.Id, entity.Id).ToList(),
                ProductRequiredAttributes = productService.ArrangeRequiredProductAttributes(entity.Id).ToList(),
                ProductPictures = pictureService.AllPictures(entity.Id).ToList(),
                Currencies = currencyService.AllActiveCurrencies().ToList(),
                CategoryName = categoryService.CategoryWithParentNames(parentProduct.CategoryId.Value),
                VatRates = taxService.AllTaxRate().ToList(),
                ProductGroups = productService.ProductGroups().ToList(),
                ErpCompanies = erpCompanyService.ErpCompanies().ToList(),
                ErpSelectedCompanies = erpCompanyService.ProductCompaniesById(id).ToList(),
                Suppliers = supplierService.AllActiveSuppliers().ToList(),
                Brands = brandService.AllActiveBrands().ToList()
            };

            ViewBag.TableName = GetFieldValues.ProductTable;
            ViewBag.FieldNames = GetFieldValues.ProductFields;

            var productAttributeMappingFieldList = (from fieldName in model.ProductRequiredAttributes
                                                    select fieldName.Attributes.Name).ToList();

            ViewBag.RequiredAttributeTableName = GetFieldValues.ProductAttributeMappingTable;
            ViewBag.RequiredAttributeFieldNames = productAttributeMappingFieldList;

            if (!model.Product.SupplierId.HasValue)
                model.Product.SupplierId = parentProduct.SupplierId;

            if (!model.Product.BrandId.HasValue)
                model.Product.BrandId = parentProduct.BrandId;

            return View(model);
        }

        [Route("edit/{id}")]
        [HttpPost]
        public IActionResult Edit(ProductViewModel model, LanguageViewModel languageViewModel, LanguageViewAttributeModel languageViewAttributeModel)
        {
            if (!Validation(model))
                return RedirectToAction("edit", new { lang = CurrentLanguage, id = model.Product.Id, validation = false });

            languageService.EditLanguageValues(languageViewModel.LanguageValues, model.Product.Id, GetFieldValues.ProductTable);

            productService.EditProductAttributeMapping(model.ProductCategoryAttributes.Union(model.ProductRequiredAttributes).ToList(), model.Product.Id);

            foreach (KeyValuePair<int, List<LanguageValuesModel>> kvp in languageViewAttributeModel.AttributeLanguageValues)
            {
                languageService.EditAttributeValuesLanguage(kvp.Key, kvp.Value, kvp.Value.Select(x => x.EntityId).ToList(), GetFieldValues.ProductAttributeMappingTable);
            }

            productService.ArrangeProductAttributeValue(model.Product.Id);

            model.Product.BuyingPrice = string.IsNullOrWhiteSpace(model.Product.BuyingPriceString) ? 0 : decimal.Parse(model.Product.BuyingPriceString, NumberStyles.Currency, new CultureInfo("tr-TR"));
            model.Product.AbroadDesi = string.IsNullOrWhiteSpace(model.Product.AbroadDesiString) ? 0 : decimal.Parse(model.Product.AbroadDesiString, NumberStyles.Currency, new CultureInfo("tr-TR"));
            model.Product.Desi = string.IsNullOrWhiteSpace(model.Product.DesiString) ? 0 : decimal.Parse(model.Product.DesiString, NumberStyles.Currency, new CultureInfo("tr-TR"));
            model.Product.PsfPrice = string.IsNullOrWhiteSpace(model.Product.PsfPriceString) ? 0 : decimal.Parse(model.Product.PsfPriceString, NumberStyles.Currency, new CultureInfo("tr-TR"));
            model.Product.CorporatePrice = string.IsNullOrWhiteSpace(model.Product.CorporatePriceString) ? 0 : decimal.Parse(model.Product.CorporatePriceString, NumberStyles.Currency, new CultureInfo("tr-TR"));
            model.Product.DdpPrice = string.IsNullOrWhiteSpace(model.Product.DdpPriceString) ? 0 : decimal.Parse(model.Product.DdpPriceString, NumberStyles.Currency, new CultureInfo("tr-TR"));
            model.Product.FobPrice = string.IsNullOrWhiteSpace(model.Product.FobPriceString) ? 0 : decimal.Parse(model.Product.FobPriceString, NumberStyles.Currency, new CultureInfo("tr-TR"));

            productService.EditProduct(model.Product);

            var ProductStockModel = new WarehouseProductStockModel()
            {
                ProductId = model.Product.Id,
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now,
                Name = model.Product.Title,
                Quantity = 0,
                Sku = model.Product.Sku,
                State = 0
            };

            if (warehouseService.IsProductStockDefined(model.Product.Id))
                warehouseService.EditProductStock(ProductStockModel);
            else
            {
                ProductStockModel.Id = 0;
                warehouseService.AddNewProductStock(ProductStockModel);
            }


            return RedirectToAction("edit", new { id = model.Product.Id, lang = CurrentLanguage });
        }

        [Route("delete/{id}")]
        public JsonResult Delete(int id)
        {
            var response = productService.DeleteProduct(id, out string exception);

            if (response)
                return Json(new { success = true, message = $"{exception} deleted successfully!" });

            return Json(new { success = false, message = $"{exception}" });

        }

        [Route("change-state/{id}")]
        public JsonResult ChangeState(int id)
        {
            var state = productService.ProductById(id).State.Value;

            if (state == (int)State.Active)
                state = (int)State.Passive;
            else if (state == (int)State.Passive)
                state = (int)State.Active;

            var response = productService.SetStateProduct(id, state, out string exception);

            if (response)
                return Json(new { success = true, message = $"{exception} updated successfully!" });

            return Json(new { success = false, message = $"{exception}" });
        }

        [Route("removeAttribute/{id}")]
        public IActionResult RemoveAttribute(int id)
        {
            var productAttributeMapping = productService.ProductAttributeMappingById(id);

            var attributeName = productAttributeMapping.Attributes.Name;

            var productId = productService.DeleteProductAttributeMappingWithIdReturnProductId(id);

            productService.DeleteProductAttributeValue(productId, attributeName);

            return RedirectToAction("edit", new { lang = CurrentLanguage, id = productId });
        }

        private bool Validation(ProductViewModel model)
        {
            if (model.ProductCategoryAttributes != null && model.ProductCategoryAttributes.Any(x => x.IsRequired && x.AttributeValueId == null))
                return false;

            if (model.ProductRequiredAttributes != null && model.ProductRequiredAttributes.Any(x => string.IsNullOrWhiteSpace(x.RequiredAttributeValue)))
                return false;

            return true;
        }

        [Route("allproduct-list/{id}")]
        [HttpPost]
        public JsonResult AllProductList(int id, KendoFilterModel kendoFilterModel)
        {
            try
            {
                IEnumerable<ProductModel> data = null;
                int total;
                if (kendoFilterModel.filter == null)
                {
                    data = productService.AllProductsQueryableAsync(id, kendoFilterModel.take, kendoFilterModel.skip, out total);
                }
                else
                {

                    var datas = KendoFilterHelper.FilterQueryableData(productService.AllProductQueryable(), kendoFilterModel).ToList();
                    var qdatas = productService.AllProductsQueryableAsync(id, datas);
                    //var qDatas = productService.AllProductsQueryableAsync(id, kendoFilterModel.take, kendoFilterModel.skip, out total).AsQueryable();
                    //var datas = KendoFilterHelper.FilterQueryableData(qDatas, kendoFilterModel);
                    total = qdatas.Count();
                    data = qdatas;
                }

                return Json(new { total, data });
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, status = false });
            }
        }

        [Route("product-group-list")]
        public IActionResult ProductGroups()
        {
            var datas = productService.ProductGroups();
            return Json(datas);
        }

        [Route("ProductGroup")]
        public IActionResult ProductGroup()
        {
            return View();
        }

        [Route("product-group-insert")]
        [HttpPost]
        public IActionResult InsertProductGroup(ProductGroupModel models)
        {
            try
            {
                productService.AddProductGroup(models);
                return Json(new { status = true, message = "OK" });
            }
            catch (Exception ex)
            {
                return Json(new { status = true, message = ex.Message });
            }

        }

        [Route("product-group-update")]
        public IActionResult UpdateProductGroup(ProductGroupModel product)
        {
            try
            {
                productService.EditProductGroup(product);
                return Json(new { status = true, message = "OK" });
            }
            catch (Exception ex)
            {
                return Json(new { status = true, message = ex.Message });
            }
        }

        [Route("product-group-delete")]
        public IActionResult DeleteProductGroup(ProductGroupModel product)
        {
            try
            {
                productService.DeleteProductGroup(product);
                return Json(new { status = true, message = "OK" });
            }
            catch (Exception ex)
            {
                return Json(new { status = true, message = ex.Message });
            }
        }

        [Route("AllList")]
        public IActionResult AllList()
        {
            var allProductViewModel = new AllProductViewModel
            {
                languages = languageService.AllLanguagesWithDefault().ToList()
            };

            return View(allProductViewModel);
        }

        [Route("purchase-department")]
        public IActionResult PurchaseDepartment()
        {
            return View();
        }
    }

}