using ApiClient.HttpClient;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using ProductDb.Admin.Helpers.LocalizationHelpers;
using ProductDb.Common.Enums;
using ProductDb.Common.GlobalEntity;
using ProductDb.Services.BrandServices;
using ProductDb.Services.CategoryServices;
using ProductDb.Services.LanguageServices;
using ProductDb.Services.PermissionServices;
using ProductDb.Services.ProductServices;
using ProductDb.Services.StoreServices;
using System;
using System.Collections.Generic;

namespace ProductDb.Admin.Controllers
{
    [MiddlewareFilter(typeof(LocalizationPipeline))]
    [Route("{lang}/integration")]
    [Authorize]
    public class IntegrationController : BaseController
    {
        private readonly IApiRepo apiRepo;
        private readonly IStoreService storeService;
        private readonly IBrandService brandService;
        private readonly ICategoryService categoryService;
        private readonly IProductService productService;
        public IntegrationController(ILanguageService languageService, IUserRolePermissionService userRolePermissionService, IApiRepo apiRepo, IStoreService storeService, IBrandService brandService, ICategoryService categoryService, IProductService productService) : base(languageService, userRolePermissionService)
        {
            this.apiRepo = apiRepo;
            this.storeService = storeService;
            this.brandService = brandService;
            this.categoryService = categoryService;
            this.productService = productService;
        }

        [Route("index")]
        public IActionResult Index()
        {
            return View();
        }

        #region Integration

        public string GetBearerToken()
        {
            ApiResult apiResult = apiRepo.Post("Token", new LoginModel() { Username = "burak", Password = "123456" }, "", Endpoints.ExportApi, MediaType.Json).Result;

            string Token = JsonConvert.DeserializeObject<string>(apiResult.JsonContent);

            return Token;
        }

        [Route("list")]
        public IActionResult IntegrationProductList(string error)
        {
            ViewBag.Error = error;
            ViewBag.Stores = GetStore();
            ViewBag.Integration = GetIntegration();
            ViewBag.Brand = GetBrand();
            ViewBag.Category = GetCategory();
            ViewBag.Language = Getlanguage();
            ViewBag.Storesval = 0;
            ViewBag.Integrationval = 0;
            ViewBag.Brandval = 0;
            ViewBag.Categoryval = 0;
            ViewBag.Languageval = 0;
            ViewBag.Sku = 0;
            return View();
        }

        [HttpPost]
        [Route("list")]
        public IActionResult IntegrationProductList(string sku, string brand, string category, string store, Integration integration, string language)
        {
            List<IntegrationList> integrationLists = new List<IntegrationList>();

            switch (integration)
            {
                case Integration.Entegra:
                    if (!String.IsNullOrEmpty(store) && !String.IsNullOrEmpty(language))
                        ViewBag.Url = "http://exportapi.sanalmagaza.com/api/dataexport/get-xml-data/" + store + "/" + language + "/b6732ad4-2d13-40e1-a4eb-c843a5fe28cd";
                    ViewBag.Url = "Store ya da Language boş bırakılamaz";
                    return View();
                case Integration.Allegro:
                    if (!String.IsNullOrEmpty(store) && !String.IsNullOrEmpty(language))
                        ViewBag.Url = "http://exportapi.sanalmagaza.com/api/dataexport/get-xml-data-with-attribute/" + store + "/" + language + "/00c4a02c-2722-4235-9c40-2910dd5c6c5f";
                    ViewBag.Url = "Store ya da Language boş bırakılamaz";
                    return View();
                default:
                    break;
            }

            ViewBag.Stores = GetStore();
            ViewBag.Integration = GetIntegration();
            ViewBag.Brand = GetBrand();
            ViewBag.Category = GetCategory();
            ViewBag.Language = Getlanguage();
            ViewBag.Storesval = store;
            ViewBag.Integrationval = integration;
            ViewBag.Brandval = brand;
            ViewBag.Categoryval = category;
            ViewBag.Languageval = language;
            ViewBag.Sku = sku;

            if (!String.IsNullOrEmpty(sku))
            {
                integrationLists = productService.integrationLists(sku, brand, category, store);
                if (integrationLists.Count == 0)
                {
                    ViewBag.Error = "Sku Bulunamamakta ya da Store'a eklenmemiş";
                    return View();
                }
                else
                {
                    ViewBag.List = 1;
                    return View(integrationLists);
                }
            }
            else if (!String.IsNullOrEmpty(brand))
            {
                integrationLists = productService.integrationLists(sku, brand, category, store);
                if (integrationLists.Count == 0)
                {
                    ViewBag.Error = "Bu kategoride ürün bulunmamakta ya da Store'a eklenmemiş ";
                    return View();
                }
                else
                {
                    ViewBag.List = 1;
                    return View(integrationLists);
                }
            }
            else if (!String.IsNullOrEmpty(category))
            {
                integrationLists = productService.integrationLists(sku, brand, category, store);
                if (integrationLists.Count == 0)
                {
                    ViewBag.Error = "Bu markada ürün bulunmamakta ya da Store'a eklenmemiş";
                    return View();
                }
                else
                {
                    ViewBag.List = 1;
                    return View(integrationLists);
                }
            }
            ViewBag.Error = "Bilgiler Eksik";
            return View();
        }

        [HttpGet]
        [Route("update-all-store/{sku}/{brandval}/{categoryval}/{storeval}/{integrationval}/{languageval}")]
        public IActionResult UpdateAll(string sku, string brandval, string categoryval, string storeval, Integration integrationval, string languageval)
        {
            List<int> integrationLists = new List<int>();
            if (sku!="default")
            {
                integrationLists = productService.integrationListId(sku, brandval, categoryval, storeval);
            }
            else if (!String.IsNullOrEmpty(brandval))
            {
                integrationLists = productService.integrationListId(sku, brandval, categoryval, storeval);
            }
            else if (!String.IsNullOrEmpty(categoryval))
            {
                integrationLists = productService.integrationListId(sku, brandval, categoryval, storeval);
            }
            IntegrationApiModel integrationApiModel = new IntegrationApiModel()
            {
                LanguageId = Convert.ToInt32(languageval),
                ProductIds = integrationLists,
                ProfileId = integrationval.GetHashCode(),
                StoreId = Convert.ToInt32(storeval),
                CAProductId = 0
            };

            ApiResult apiResult = apiRepo.Post("integration/update-product", integrationApiModel, GetBearerToken(), Endpoints.ExportApi, MediaType.Json).Result;
            if (apiResult.ResponseCode == 200)
                return RedirectToAction("IntegrationProductList", new { lang = CurrentLanguage, error = "Başarılı" });
            else
                return RedirectToAction("IntegrationProductList", new { lang = CurrentLanguage, error = "Yükleme Başarısız" });
        }

        [Route("update-single-product")]
        public JsonResult UpdateSingleProduct(int storeId, int storeProductId, int productId, Integration integrationId, string languageId)
        {
            IntegrationApiModel integrationApiModel = new IntegrationApiModel()
            {
                LanguageId = Convert.ToInt32(languageId),
                ProductIds = new List<int>() { productId },
                ProfileId = integrationId.GetHashCode(),
                StoreId = storeId,
                CAProductId = storeProductId
            };
            ApiResult apiResult = apiRepo.Post("integration/update-product", integrationApiModel, GetBearerToken(), Endpoints.ExportApi, MediaType.Json).Result;
            if (apiResult.ResponseCode == 200)
                return Json(1);
            return Json(0);
        }

        [Route("push-all-product/{sku}/{brandval}/{categoryval}/{storeval}/{integrationval}/{languageval}")]
        public IActionResult PushAllProduct(string sku, string brandval, string categoryval, string storeval, Integration integrationval, string languageval)
        {
            List<int> integrationLists = new List<int>();
            if (sku != "default")
            {
                integrationLists = productService.integrationListId(sku, brandval, categoryval, storeval);
            }
            else if (!String.IsNullOrEmpty(brandval))
            {
                integrationLists = productService.integrationListId(sku, brandval, categoryval, storeval);
            }
            else if (!String.IsNullOrEmpty(categoryval))
            {
                integrationLists = productService.integrationListId(sku, brandval, categoryval, storeval);
            }
            IntegrationApiModel integrationApiModel = new IntegrationApiModel()
            {
                LanguageId = Convert.ToInt32(languageval),
                ProductIds = integrationLists,
                ProfileId = integrationval.GetHashCode(),
                StoreId = Convert.ToInt32(storeval),
                CAProductId = 0
            };

            ApiResult apiResult = apiRepo.Post("integration/push-product", integrationApiModel, GetBearerToken(), Endpoints.ExportApi, MediaType.Json).Result;
            if (apiResult.ResponseCode == 200)
                return RedirectToAction("IntegrationProductList", new { lang = CurrentLanguage, error = "Başarılı" });
            else
                return RedirectToAction("IntegrationProductList", new { lang = CurrentLanguage, error = "Yükleme Başarısız" });
        }

        [Route("push-single-product")]
        public JsonResult PushSingleProduct(int storeId, int storeProductId, int productId, Integration integrationId, string languageId)
        {
            IntegrationApiModel integrationApiModel = new IntegrationApiModel()
            {
                LanguageId = Convert.ToInt32(languageId),
                ProductIds = new List<int>() { productId },
                ProfileId = integrationId.GetHashCode(),
                StoreId = storeId,
                CAProductId = storeProductId
            };
            ApiResult apiResult = apiRepo.Post("integration/push-product", integrationApiModel, GetBearerToken(), Endpoints.ExportApi, MediaType.Json).Result;
            if (apiResult.ResponseCode == 200)
                return Json(1);
            return Json(0);
        }

        [Route("push-all-parent-child-product/{sku}/{brandval}/{categoryval}/{storeval}/{integrationval}/{languageval}")]
        public IActionResult PushAllParentChildProduct(string sku, string brandval, string categoryval, string storeval, Integration integrationval, string languageval)
        {
            List<int> integrationLists = new List<int>();
            if (sku != "default")
            {
                integrationLists = productService.integrationListId(sku, brandval, categoryval, storeval);
            }
            else if (!String.IsNullOrEmpty(brandval))
            {
                integrationLists = productService.integrationListId(sku, brandval, categoryval, storeval);
            }
            else if (!String.IsNullOrEmpty(categoryval))
            {
                integrationLists = productService.integrationListId(sku, brandval, categoryval, storeval);
            }
            IntegrationApiModel integrationApiModel = new IntegrationApiModel()
            {
                LanguageId = Convert.ToInt32(languageval),
                ProductIds = integrationLists,
                ProfileId = integrationval.GetHashCode(),
                StoreId = Convert.ToInt32(storeval),
                CAProductId = 0
            };

            ApiResult apiResult = apiRepo.Post("integration/push-parent-product", integrationApiModel, GetBearerToken(), Endpoints.ExportApi, MediaType.Json).Result;
            if (apiResult.ResponseCode == 200)
                return RedirectToAction("IntegrationProductList", new { lang = CurrentLanguage, error = "Başarılı" });
            else
                return RedirectToAction("IntegrationProductList", new { lang = CurrentLanguage, error = "Yükleme Başarısız" });
        }

        [Route("push-single-parent-child-product")]
        public JsonResult PushSingleParentChildProduct(int storeId, int storeProductId, int productId, Integration integrationId, string languageId)
        {
            IntegrationApiModel integrationApiModel = new IntegrationApiModel()
            {
                LanguageId = Convert.ToInt32(languageId),
                ProductIds = new List<int>() { productId },
                ProfileId = integrationId.GetHashCode(),
                StoreId = storeId,
                CAProductId = storeProductId
            };
            ApiResult apiResult = apiRepo.Post("integration/push-parent-product", integrationApiModel, GetBearerToken(), Endpoints.ExportApi, MediaType.Json).Result;
            if (apiResult.ResponseCode == 200)
                return Json(1);
            return Json(0);
        }

        //model
        public List<SelectListItem> GetStore()
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>()
            {
                new SelectListItem()
                {
                    Text = "---- Select ----",
                    Value = "0"
                }
            };
            foreach (var item in storeService.AllActiveStores())
            {
                selectListItems.Add(new SelectListItem()
                {
                    Value = item.Id.ToString(),
                    Text = item.Name
                });
            }
            return selectListItems;
        }

        public List<SelectListItem> Getlanguage()
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>()
            {
                new SelectListItem()
                {
                    Text = "---- Select ----",
                    Value = "0"
                }
            };
            foreach (var item in languageService.AllLanguages())
            {
                selectListItems.Add(new SelectListItem()
                {
                    Value = item.Id.ToString(),
                    Text = item.Name
                });
            }
            return selectListItems;
        }

        public List<SelectListItem> GetBrand()
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>()
            {
                new SelectListItem()
                {
                    Text = "---- Select ----",
                    Value = "0"
                }
            };
            foreach (var item in brandService.AllBrands())
            {
                selectListItems.Add(new SelectListItem()
                {
                    Value = item.Id.ToString(),
                    Text = item.Name
                });
            }
            return selectListItems;
        }

        public List<SelectListItem> GetCategory()
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>()
            {
                new SelectListItem()
                {
                    Text = "---- Select ----",
                    Value = "0"
                }
            };
            foreach (var item in categoryService.AllActiveCategories())
            {
                selectListItems.Add(new SelectListItem()
                {
                    Value = item.Id.ToString(),
                    Text = item.Name
                });
            }
            return selectListItems;
        }

        public List<SelectListItem> GetIntegration()
        {
            Array Actors = Enum.GetValues(typeof(Integration));
            List<SelectListItem> selectListItems = new List<SelectListItem>()
            {
                new SelectListItem()
                {
                    Text = "---- Select ----",
                    Value = "0"
                }
            };
            foreach (var item in Actors)
            {
                selectListItems.Add(new SelectListItem()
                {
                    Text = Enum.GetName(typeof(Integration), item),
                    Value = item.ToString()
                });
            }
            return selectListItems;
        }
        #endregion
    }
}