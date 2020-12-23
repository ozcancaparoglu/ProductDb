using ApiClient.HttpClient;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductDb.Admin.Helpers;
using ProductDb.Admin.Helpers.LocalizationHelpers;
using ProductDb.Admin.PageModels.Product;
using ProductDb.Admin.PageModels.Store;
using ProductDb.Admin.PageModels.Transportation;
using ProductDb.Common.Enums;
using ProductDb.Mapping.BiggBrandDbModels;
using ProductDb.Services.BrandServices;
using ProductDb.Services.CalculationServices;
using ProductDb.Services.CategoryServices;
using ProductDb.Services.CurrencyServices;
using ProductDb.Services.ErpComanyService;
using ProductDb.Services.LanguageServices;
using ProductDb.Services.PermissionServices;
using ProductDb.Services.StoreServices;
using ProductDb.Services.WarehouseServices;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace ProductDb.Admin.Controllers
{
    [Authorize]
    [MiddlewareFilter(typeof(LocalizationPipeline))]
    [Route("{lang}/store")]
    public class StoreController : BaseController
    {
        private readonly IErpCompanyService erpCompanyService;
        private readonly IBrandService brandService;
        private readonly ICategoryService categoryService;
        private readonly ICalculationService calculationService;
        private readonly IStoreService storeService;
        private readonly IWarehouseService warehouseService;
        private readonly ICurrencyService currencyService;
        private readonly IApiRepo apiRepo;

        public StoreController(ILanguageService languageService, IStoreService storeService, IWarehouseService warehouseService,
            IUserRolePermissionService userRolePermission, ICurrencyService currencyService, IApiRepo apiRepo, ICalculationService calculationService,ICategoryService categoryService,IBrandService brandService,
            IErpCompanyService erpCompanyService) : base(languageService, userRolePermission)
        {
            this.erpCompanyService = erpCompanyService;
            this.brandService = brandService;
            this.categoryService = categoryService;
            this.calculationService = calculationService;
            this.storeService = storeService;
            this.warehouseService = warehouseService;
            this.currencyService = currencyService;
            this.apiRepo = apiRepo;
        }

        [HttpPost]
        [Route("insertStoreProduct")]
        public JsonResult Insert(AddProductViewModel model)
        {
            if (model.Products == null || model.Products.Count(x => x.IsStoreCheck) == 0)
                return Json(new { result = "Redirect" });
            try
            {
                storeService.AddNewStoreProductMappingRange(model.StoreId, model.Products.Where(p => p.IsStoreCheck).Select(x => x.Id).ToList());

                return Json(new { result = "Redirect" });
            }
            catch (System.Exception)
            {

                return Json(new { result = "Failed", url = $"/{CurrentLanguage}/store/product" });
            }
        }

        [Route("list")]
        public IActionResult List()
        {
            var model = storeService.AllStores();
            return View(model);
        }

        [Route("create")]
        public IActionResult Create()
        {
            var model = new StoreViewModel()
            {
                Store = new StoreModel(),
                Warehouses = warehouseService.AllWarehouseTypes().ToList(),
                Currencies = currencyService.AllCurrencies().ToList(),
                FormulaGroups = calculationService.AllFormulaGroups().ToList(),
                CargoTypes = calculationService.AllCargoTypes().ToList(),
                ErpCompanies = erpCompanyService.ErpCompanies().ToList(),
                StoreTypes = storeService.AllStoreTypes().ToList()
            };

            return View(model);
        }

        [Route("create")]
        [HttpPost]
        public IActionResult Create(StoreViewModel model)
        {
            model.Store.MaxPrice = decimal.Parse(model.Store.MaxPriceString ?? "0", NumberStyles.Currency, new CultureInfo("tr-TR"));
            model.Store.MinPrice = decimal.Parse(model.Store.MinPriceString ?? "0", NumberStyles.Currency, new CultureInfo("tr-TR"));
            model.Store.Rate = decimal.Parse(model.Store.RateString ?? "0", NumberStyles.Currency, new CultureInfo("tr-TR"));
            model.Store.DefaultMarj = decimal.Parse(model.Store.DefaultMarjString ?? "0", NumberStyles.Currency, new CultureInfo("tr-TR"));
            model.Store.Sarf = decimal.Parse(model.Store.SarfString ?? "0", NumberStyles.Currency, new CultureInfo("tr-TR"));

            var entity = storeService.AddNewStore(model.Store);

            model.StoreWarehouseMappings = new List<StoreWarehouseMappingModel>();

            foreach (var warehouse in model.Warehouses)
            {
                if (!warehouse.IsSelected)
                    continue;

                model.StoreWarehouseMappings.Add(new StoreWarehouseMappingModel
                {
                    StoreId = entity.Id,
                    WarehouseTypeId = warehouse.Id
                });
            }

            storeService.AddNewStoreWarehouses(model.StoreWarehouseMappings);

            return RedirectToAction("list", new { lang = CurrentLanguage });
        }

        [Route("edit/{id}")]
        public IActionResult Edit(int id)
        {
            var model = new StoreViewModel()
            {
                Store = storeService.StoreById(id),
                Warehouses = warehouseService.AllWarehouseTypes().ToList(),
                StoreWarehouseMappings = storeService.StoreWarehouses(id).ToList(),
                Currencies = currencyService.AllCurrencies().ToList(),
                FormulaGroups = calculationService.AllFormulaGroups().ToList(),
                CargoTypes = calculationService.AllCargoTypes().ToList(),
                ErpCompanies = erpCompanyService.ErpCompanies().ToList(),
                StoreTypes = storeService.AllStoreTypes().ToList()
            };

            foreach (var item in model.StoreWarehouseMappings)
                model.Warehouses.FirstOrDefault(x => x.Id == item.WarehouseTypeId).IsSelected = true;

            model.Store.MaxPriceString = model.Store.MaxPrice != null ? model.Store.MaxPrice.ToString() : 0.ToString();
            model.Store.MinPriceString = model.Store.MinPrice != null ? model.Store.MinPrice.ToString() : 0.ToString();

            model.Store.MaxPointString = model.Store.MaxPoint != null ? model.Store.MaxPoint.ToString() : 0.ToString();
            model.Store.MinPointString = model.Store.MinPoint != null ? model.Store.MinPoint.ToString() : 0.ToString();
            model.Store.RateString = model.Store.Rate != null ? model.Store.Rate.ToString() : 0.ToString();

            model.Store.DefaultMarjString = model.Store.DefaultMarj != null ? model.Store.DefaultMarj.ToString() : 0.ToString();
            model.Store.SarfString = model.Store.Sarf != null ? model.Store.Sarf.ToString() : 0.ToString();

            return View(model);
        }

        [Route("edit/{id}")]
        [HttpPost]
        public IActionResult Edit(StoreViewModel model)
        {
            model.Store.MaxPrice = decimal.Parse(model.Store.MaxPriceString ?? "0", NumberStyles.Currency, new CultureInfo("tr-TR"));
            model.Store.MinPrice = decimal.Parse(model.Store.MinPriceString ?? "0", NumberStyles.Currency, new CultureInfo("tr-TR"));

            model.Store.MaxPoint = decimal.Parse(model.Store.MaxPointString ?? "0", NumberStyles.Currency, new CultureInfo("tr-TR"));
            model.Store.MinPoint = decimal.Parse(model.Store.MinPointString ?? "0", NumberStyles.Currency, new CultureInfo("tr-TR"));
            model.Store.Rate = decimal.Parse(model.Store.RateString ?? "0", NumberStyles.Currency, new CultureInfo("tr-TR"));

            model.Store.DefaultMarj = decimal.Parse(model.Store.DefaultMarjString ?? "0", NumberStyles.Currency, new CultureInfo("tr-TR"));
            model.Store.Sarf = decimal.Parse(model.Store.SarfString ?? "0", NumberStyles.Currency, new CultureInfo("tr-TR"));

            var entity = storeService.EditStore(model.Store);

            model.StoreWarehouseMappings = new List<StoreWarehouseMappingModel>();

            foreach (var warehouse in model.Warehouses)
            {
                if (!warehouse.IsSelected)
                    continue;

                model.StoreWarehouseMappings.Add(new StoreWarehouseMappingModel
                {
                    StoreId = entity.Id,
                    WarehouseTypeId = warehouse.Id
                });
            }

            storeService.EditStoreWarehouses(model.StoreWarehouseMappings, entity.Id);

            return RedirectToAction("edit", new { lang = CurrentLanguage, id = model.Store.Id });
        }

        // new 

        [Route("StoreProdocuts/{id}")]
        public IActionResult StoreProdocuts(int id)
        {
            var products = storeService.StoreProducts(id).ToList();

            return Json(products);
        }

        [Route("StoreProduct-Update")]
        [HttpPost]
        public IActionResult StoreProductUpdate(StoreProductMappingModel productMapping)
        {
            try
            {
                var data = storeService.UpdateStoreProductMapping(productMapping);
                return Json(data.Id);
            }
            catch (Exception ex)
            {
                return Json(new { status = true, message = ex.Message });
            }
        }

        [Route("StoreProduct-Delete")]
        [HttpPost]
        public IActionResult StoreProductDelete(StoreProductMappingModel productMapping)
        {
            try
            {
                storeService.DeleteStoreProduct(productMapping.Id);
                return Json(productMapping.Id);
            }
            catch (Exception ex)
            {
                return Json(new { status = true, message = ex.Message });
            }
        }

        [HttpGet]
        [Route("StoreCategoryById/{id}")]
        public IActionResult StoreCategoryById(int id)
        {
            try
            {
                var datas = storeService.StoreCategoryByStoreId(id);
                return Json(datas);
            }
            catch (Exception ex)
            {
                return Json(ex.Message.ToString());
            }
        }

        [HttpGet]
        [Route("StoreCategory/{id}")]
        public IActionResult StoreCategory(int id)
        {
            var result = ViewComponent("UpdateStoreCategory", new { id = id });
            return result;
        }

        [HttpPost]
        [Route("AddStoreCategory")]
        public IActionResult AddStoreCategory(StoreCategoryMappingModel storeCategory)
        {
            try
            {
                storeService.AddStoreCategory(storeCategory);
                return Json(new { message = "OK", status = true });
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, status = false });
            }
        }

        [HttpPost]
        [Route("UpdateStoreCategory")]
        public IActionResult UpdateStoreCategory(StoreCategoryMappingModel storeCategory)
        {
            try
            {
                storeService.EditStoreCategory(storeCategory);
                return Json(new { message = "OK", status = true });
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, status = false });
            }
        }

        [HttpPost]
        [Route("DeleteStoreCategory")]
        public IActionResult DeleteStoreCategory(StoreCategoryMappingModel storeCategory)
        {
            try
            {
                storeService.DeleteStoreCategory(storeCategory);
                return Json(new { message = "OK", status = true });
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, status = false });
            }
        }


        // end new
        [HttpPost]
        [Route("sync-categories/{storeId}")]
        public JsonResult SyncCategories(int storeId)
        {
            if(storeService.SyncProductCategories(storeId, out string exception))
                return Json(new { success = true, result = "success", id = storeId });

            return Json(new { success = false, result = "error", message = exception });
        }

        [Route("product")]
        public IActionResult Product()
        {
            var model = new StoreProductViewModel()
            {
                StoreId = 0,
                Stores = storeService.AllActiveStores().ToList(),
                Store = new StoreModel(),
                StoreProducts = new List<StoreProductMappingModel>()
            };

            return View(model);
        }

        [Route("product")]
        [HttpPost]
        public IActionResult Product(StoreProductViewModel model)
        {
            if (model.StoreId != 0)
            {
                model.Store = storeService.StoreById(model.StoreId);
                model.StoreProducts = storeService.StoreProducts(model.StoreId).ToList();
                model.Stores = storeService.AllActiveStores().ToList();
            }
            else
            {
                model = new StoreProductViewModel()
                {
                    StoreId = 0,
                    Stores = storeService.AllActiveStores().ToList()
                };
                ViewBag.NoResultFound = "No Result Found";
            }

            return View(model);
        }


        [Route("productByStoreId/{storeId}")]
        [HttpGet]
        public JsonResult ProductByStoreId(int storeId)
        {
            try
            {
                var storeProducts = storeService.StoreProducts(storeId);
                return Json(storeProducts);
            }
            catch (System.Exception ex)
            {
                return Json(new { status = false, message = ex.Message });
            }
        }

        [HttpGet]
        [Route("storeReadyCopyProducts/{storeId}")]
        public IActionResult StoreReadyCopyProducts(int storeId)
        {
            var result = ViewComponent("StoreCopy", new { id = storeId });
            return result;
        }

        [HttpGet]
        [Route("storeReadyCopySettings/{storeId}/{typeId}/{typeName}")]
        public IActionResult StoreReadyCopySettings(int storeId, int typeId, string typeName)
        {
            var result = ViewComponent("StoreSettingsCopy", new { id = storeId, typeId, typeName });
            return result;
        }

        [HttpPost]
        [Route("storeProductInsert")]
        public JsonResult Insert(int storeId, List<int> products)
        {
            try
            {
                storeService.AddNewStoreProductMappingRangeCopy(storeId, products, _userId);
                return Json(new { status = true, message = "OK" });
            }
            catch (System.Exception ex)
            {
                return Json(new { status = false, message = ex.Message });
            }
        }

        [HttpPost]
        [Route("storeSettingsInsert/{storeId}/{targetStoreId}/{typeId}")]
        public JsonResult Insert(int storeId, int targetStoreId, int typeId)
        {
            try
            {
                switch (typeId)
                {
                    case (int)StoreCopyTypeEnum.Category:
                        storeService.AddNewStoreCategoryRangeCopy(storeId, targetStoreId, typeId);
                        break;
                    case (int)StoreCopyTypeEnum.Margin:
                        storeService.AddNewStoreMarginRangeCopy(storeId, targetStoreId, typeId);
                        break;
                    case (int)StoreCopyTypeEnum.Cargo:
                        storeService.AddNewStoreCargoRangeCopy(storeId, targetStoreId, typeId);
                        break;
                    case (int)StoreCopyTypeEnum.Transportation:
                        storeService.AddNewStoreTransportationRangeCopy(storeId, targetStoreId, typeId);
                        break;
                    default:
                        break;
                }
                return Json(new { status = true, message = "OK" });
            }
            catch (System.Exception ex)
            {
                return Json(new { status = false, message = ex.Message });
            }
        }

        [Route("delete-product/{id}")]
        public IActionResult DeleteProduct(int id)
        {
            storeService.DeleteStoreProduct(id);
            return RedirectToAction("Product", new { lang = CurrentLanguage });
        }

        [Route("add-product-component/{storeId}")]
        public IActionResult AddProducts(int storeId)
        {
            var result = ViewComponent("AddProduct", new { storeId });
            return result;
        }

        // Product Fixing
        [Route("ProductFixing")]
        public IActionResult ProductFixing()
        {
            var model = new StoreProductViewModel()
            {
                StoreId = 0,
                Stores = storeService.AllActiveStores().ToList(),
                Store = new StoreModel(),
                isUploaded = false
            };

            return View(model);
        }


        // for margins
        [Route("MarginBrands/{storeId}/{marginTypeId}")]
        public IActionResult MarginBrands(int storeId, int marginTypeId)
        {
            try
            {
                var datas = calculationService.StoreBrands(storeId, marginTypeId);
                // store margins   
                return Json(datas);
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        [Route("MarginCategories/{storeId}/{marginTypeId}")]
        public IActionResult MarginCategories(int storeId, int marginTypeId)
        {
            try
            {
                var datas = calculationService.StoreCategory(storeId, marginTypeId);
                return Json(datas);
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        [Route("MarginProducts/{id}/{marginTypeId}")]
        public IActionResult MarginProducts(int id, int marginTypeId)
        {
            try
            {
                var datas = calculationService.StoreProducts(id, marginTypeId);
                // store margins   
                return Json(datas);
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        [Route("MarginCategoryBrand/{id}/{marginTypeId}")]
        public IActionResult MarginCategoryBrand(int id, int marginTypeId)
        {
            try
            {
                var datas = calculationService.MarginCategoryBrand(id, marginTypeId);
                // store margins   
                return Json(datas);
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }


        [Route("MarginUpdate")]
        public IActionResult MarginUpdate(MarginModel margin)
        {
            try
            {
                calculationService.MarginUpdate(margin);
                return Json(margin.Id);
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        [Route("MarginInsert")]
        [HttpPost]
        public IActionResult MarginInsert(MarginModel margin)
        {
            try
            {
                calculationService.MarginInsert(margin);
                return Json(new { status = true, message = "OK" });
            }
            catch (Exception ex)
            {
                return Json(new { status = false, message = ex.Message });
            }
        }


        [Route("MarginDelete")]
        public IActionResult MarginDelete(MarginModel margin)
        {
            try
            {
                calculationService.MarginDelete(margin);
                return Json(margin.Id);
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }


        [Route("StoreMargin/{id}")]
        public IActionResult StoreMargin(int id)
        {
            var storeModel = storeService.StoreById(id);
            // get stores
            var viewModel = new StoreMarginViewModel
            {
                MarginTypeModel = new MarginTypeModel(),
                BrandModel = new BrandModel(),
                CategoryModel = new CategoryModel(),
                Categories = categoryService.AllCategoriesWithParentNames().ToList(),
                Brands = brandService.AllActiveBrands().ToList(),
                MarginTypes = calculationService.MarginTypes().ToList(),
                StoreModel = storeModel
            };

            return View(viewModel);
        }

        [Route("StoreMarginsByStoreId/{storeId}/{marginTypeId}")]
        public IActionResult StoreMarginsByStoreId(int storeId,int marginTypeId)
        {
            try
            {
                var datas = calculationService.StoreMarginsByStoreId(storeId, marginTypeId);
                return Json(datas);
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }
        // for cargo
        [Route("StoreCargo/{id}")]
        public IActionResult StoreCargo(int id)
        {
            ViewBag.StoreId = id;
            return View();
        }
        // read
        [Route("StoreCargoByStoreId/{id}")]
        public IActionResult StoreCargoByStoreId(int id)
        {
            var model = calculationService.CargoByStoreId(id);
            return Json(model);
        }
        // create
        [Route("CreateCargo")]
        public IActionResult CreateCargo(CargoModel cargo)
        {
            var model = calculationService.InsertCargo(cargo);
            return Json(model);
        }

        [Route("UpdateCargo")]
        public IActionResult UpdateCargo(CargoModel cargo)
        {
            var model = calculationService.UpdateCargo(cargo);
            return Json(model);
        }

        [Route("DeleteCargo")]
        public IActionResult DeleteCargo(CargoModel cargo)
        {
            calculationService.DeleteCargo(cargo.Id);
            return Json(new { status = true });
        }
        // for tranportation

        [Route("StoreTransportation/{id}")]
        public IActionResult StoreTransportation(int id)
        {
            var storeModel = storeService.StoreById(id);

            var viewModel = new TransportationViewModel
            {
                StoreModel = storeModel,

                TransportationTypes = calculationService.AllTransportationTypes().ToList()
            };
            return View(viewModel);
        }

        [Route("StoreTransportationByStoreId/{id}")]
        public IActionResult StoreTransportationByStoreId(int id)
        {
            var model = calculationService.AllActiveTransportations(id);
            return Json(model);
        }
        // create
        [Route("CreateTransportation")]
        public IActionResult CreateTransportation(TransportationModel transportation)
        {
            var model = calculationService.AddNewTransportation(transportation);
            return Json(model.Id);
        }
        // update
        [Route("UpdateTransportation")]
        public IActionResult UpdateTransportation(TransportationModel transportation)
        {
            transportation.TransportationTypeId = transportation.TransportationType.Id;
            transportation.CurrencyId = transportation.Currency.Id;
            transportation.Currency = null;
            transportation.TransportationType = null;

            var model = calculationService.EditTransportation(transportation);
            return Json(model.Id);
        }

        [Route("DeleteTransportation")]
        public IActionResult DeleteTransportation(TransportationModel transportation)
        {
            transportation.Currency = null;
            transportation.TransportationType = null;

            calculationService.DeleteTransportation(transportation.Id);
            return Json(new { status = true });
        }

        // store brands 
        [Route("Transportationbrands/{id}")]
        public IActionResult Transportationbrands(int id)
        {
            try
            {
                var models = calculationService.StoreTransportationBrands(id);
                return Json(models);
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }
        // store products
        [Route("Transportationproducts/{id}")]
        public IActionResult Transportationproducts(int id)
        {
            try
            {
                var models = calculationService.StoreTransportationProducts(id);
                return Json(models);
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        [Route("TransportationTypeEnum")]
        [HttpPost]
        public IActionResult TransportationTypeEnum(string IDs)
        {
            try
            {
                var arrIDs = IDs.Split(',').Select(int.Parse).ToList();
                var models = calculationService.StoreTransportationTypeEnum(arrIDs);

                return Json(models);
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        #region StoreType

        [Route("store-type-list")]
        public IActionResult StoreTypeList()
        {
            var model = storeService.AllStoreTypes();
            return View(model);
        }

        [Route("store-type-create")]
        public IActionResult StoreTypeCreate()
        {
            return View(new StoreTypeModel());
        }

        [Route("store-type-create")]
        [HttpPost]
        public IActionResult StoreTypeCreate(StoreTypeModel model)
        {
            var entity = storeService.AddNewStoreType(model);

            return RedirectToAction("StoreTypeList", new { lang = CurrentLanguage });
        }

        [Route("store-type-edit/{id}")]
        public IActionResult StoreTypeEdit(int id)
        {
            var model = storeService.StoreTypeById(id);

            return View(model);
        }

        [Route("store-type-edit/{id}")]
        [HttpPost]
        public IActionResult StoreTypeEdit(StoreTypeModel model)
        {
            var entity = storeService.EditStoreType(model);

            return RedirectToAction("StoreTypeList", new { lang = CurrentLanguage});
        }

        [Route("store-type-delete/{id}")]
        public IActionResult StoreTypeDelete(int id)
        {
            var state = storeService.StoreTypeById(id).State;

            if (state == (int)State.Active)
                state = (int)State.Passive;
            else if (state == (int)State.Passive)
                state = (int)State.Active;

            storeService.SetStoreTypeState(id, state.Value);

            return RedirectToAction("StoreTypeList", new { lang = CurrentLanguage });
        }


        #endregion

    }
}