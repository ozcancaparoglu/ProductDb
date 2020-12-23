using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiClient.HttpClient;
using Ganss.XSS;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ProductDb.Common.Cache;
using ProductDb.Common.Enums;
using ProductDb.Common.GlobalEntity;
using ProductDb.Common.Helpers;
using ProductDb.Logging;
using ProductDb.Services.ProductServices;
using ProductDb.Services.ProductVariantServices;
using ProductDb.Services.StoreServices;

namespace ProductDb.Api.Export.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IntegrationController : ControllerBase
    {
        private readonly IApiRepo apiRepo;
        private readonly IProductService productService;
        private readonly IParentProductService parentProductService;
        private readonly IStoreService storeService;
        private readonly ILoggerManager loggerManager;
        private readonly IProductVariantService productVariantService;
        public IntegrationController(IApiRepo apiRepo, IStoreService storeService, IProductService productService, IParentProductService parentProductService, ILoggerManager loggerManager, IProductVariantService productVariantService)
        {
            this.apiRepo = apiRepo;
            this.storeService = storeService;
            this.productService = productService;
            this.parentProductService = parentProductService;
            this.loggerManager = loggerManager;
            this.productVariantService = productVariantService;
        }

        [Route("index")]
        public IActionResult Index()
        {
            return Ok();
        }

        #region CA

        [Route("refresh-token")]
        public string RefreshTokenCA()
        {
            ChannelAdvisorRefreshTokenModel channelAdvisorRefreshTokenModel = new ChannelAdvisorRefreshTokenModel();

            ApiResult apiResult = apiRepo.GetAuthorizeToken("oauth2/token", channelAdvisorRefreshTokenModel, "ZjkzZXY3a3liYWpyZ3BlYzRraXhxMTM0ZHZ1cmkyZmo6ZkRWSE5xNnlXVU9SWXk1YnRSV0RlZw==", Endpoints.ChannelAdvisor, MediaType.Encoded).Result;

            var token = JsonConvert.DeserializeObject<ChannelAdvisorTokenModel>(apiResult.JsonContent);

            return token.access_token;
        }

        [Authorize]
        [HttpPost]
        [Route("push-parent-product")]
        public IActionResult PushParentProduct([FromBody] IntegrationApiModel channelAdvisor)
        {
            string accessToken = RefreshTokenCA();
            int productParentId = 0;
            int loop = 0;
            int take = 5;
            int skip = 0;

            while (loop == 0)
            {
                var ProductList = channelAdvisor.ProductIds.Skip(skip).Take(take).ToList();
                var parentProduct = parentProductService.channelAdvisorParentProductModels(channelAdvisor.StoreId, channelAdvisor.LanguageId, ProductList);

                if (ProductList.Count < 5)
                    loop = 1;

                foreach (var item in parentProduct)
                {
                    item.ProfileID = channelAdvisor.ProfileId.ToString();

                    while (take == 5)
                    {
                        ApiResult apiResult = apiRepo.Push("v1/products?access_token=" + accessToken, item, Endpoints.ChannelAdvisor, MediaType.Json).Result;

                        if (apiResult.JsonContent != null)
                        {
                            productParentId = JsonConvert.DeserializeObject<ChannelAdvisorParentResultModel>(apiResult.JsonContent).ID;
                            break;
                        }

                    }
                    PushProductParent(channelAdvisor.StoreId, channelAdvisor.LanguageId, channelAdvisor.ProfileId, item.Sku, productParentId, accessToken);
                }
                if (loop == 0)
                {
                    skip += 5;
                    System.Threading.Thread.Sleep(3000);
                }
            }
            return Ok();
        }

        public void PushProductParent(int store_id, int language_id, int profileId, string parentSku, int productParentId, string Token)
        {
            string accessToken = Token;

            List<ChannelAdvisorProductViewModel> channelAdvisorProductVMs = new List<ChannelAdvisorProductViewModel>();

            int productId = 0;
            int loop = 0;
            int take = 5;
            int skip = 0;

            while (loop == 0)
            {
                channelAdvisorProductVMs = productService.channelAdvisorProductVMParent(store_id, language_id, parentSku, productParentId, take, skip);

                if (channelAdvisorProductVMs.Count < 5)
                    loop = 1;
                foreach (var item in channelAdvisorProductVMs)
                {
                    int i = 1;

                    ChannelAdvisorChildModel channelAdvisorChildModel = item.channelAdvisorChildModel;

                    channelAdvisorChildModel.ProfileID = profileId;

                    while (take == 5)
                    {
                        ApiResult apiResult = apiRepo.Push("v1/products?access_token=" + accessToken, channelAdvisorChildModel, Endpoints.ChannelAdvisor, MediaType.Json).Result;
                        if (apiResult.JsonContent != null)
                        {
                            productId = JsonConvert.DeserializeObject<ChannelAdvisorResultModel>(apiResult.JsonContent).ID;
                            break;
                        }

                    }

                    storeService.ChangeStoreProduct(item.channelAdvisorProductModel.Sku, store_id, productId, productParentId);

                    foreach (var imageItem in item.Images)
                    {
                        if (item.Images.Count > 5 && (i == 6 || i == 11))
                            System.Threading.Thread.Sleep(5000);
                        while (take == 5)
                        {
                            int responseCode1 = apiRepo.PutInfo("v1/Products(" + productId + ")/Images('" + imageItem.PlacementName + i.ToString() + "')?access_token=" + accessToken + "", imageItem.UrlPath, Endpoints.ChannelAdvisor, MediaType.Json).Result;
                            if (responseCode1 != 0)
                                break;
                        }
                        i++;
                    }

                    while (take == 5)
                    {
                        ApiResult apiResult2 = apiRepo.Push("v1/Products(" + productId + ")/UpdateQuantity?access_token=" + accessToken + "", item.DCQuantitys, Endpoints.ChannelAdvisor, MediaType.Json).Result;
                        if (apiResult2.JsonContent != null)
                            break;
                    }
                }
                if (loop == 0)
                {
                    skip += 5;
                    System.Threading.Thread.Sleep(3000);
                }
            }
        }

        [Authorize]
        [HttpPost]
        [Route("push-product")]
        public IActionResult PushProduct([FromBody] IntegrationApiModel channelAdvisor)
        {

            string accessToken = RefreshTokenCA();

            List<ChannelAdvisorProductViewModel> channelAdvisorProductVMs = new List<ChannelAdvisorProductViewModel>();
            List<int> variantId = new List<int>();
            int productId = 0;
            int take = 5;

            foreach (var item in channelAdvisor.ProductIds.ToList())
            {
                var variant = productVariantService.GetProductVariantByProductId(item);
                if (channelAdvisor.ProductIds.Any(x => x == variant.BaseId))
                    continue;
                else
                {
                    variantId.Add(item);
                    channelAdvisor.ProductIds.Add(variant.BaseId.Value);
                }
            }
            var ProductSku = productService.GetProductSkubyId(channelAdvisor.ProductIds);

            ApiResult productModelResult = apiRepo.Post($"integration/ProductDetailProductSKUsByStoreWithLanguage/{channelAdvisor.StoreId}/{channelAdvisor.LanguageId}", ProductSku, "", Common.Enums.Endpoints.RegisApi, Common.Enums.MediaType.Json).Result;
            var productJsonModels = JsonConvert.DeserializeObject<List<ProductJsonModel>>(productModelResult.JsonContent);

            var productPriceandStockModel = apiRepo.GetList<ProductStockAndPriceJsonModel>($"integration/ProductStockAndPriceByStoreId/{channelAdvisor.StoreId}", "", Common.Enums.Endpoints.RegisApi, Common.Enums.MediaType.Json).Result;

            channelAdvisorProductVMs = productService.JsonchannelAdvisorProductVMs(channelAdvisor.StoreId, productJsonModels, productPriceandStockModel);

            foreach (var item in channelAdvisorProductVMs)
            {
                int i = 1;

                ChannelAdvisorProductModel channelAdvisorProductModel = item.channelAdvisorProductModel;

                channelAdvisorProductModel.ProfileID = channelAdvisor.ProfileId;

                while (take == 5)
                {
                    ApiResult apiResult = apiRepo.Push("v1/products?access_token=" + accessToken, channelAdvisorProductModel, Endpoints.ChannelAdvisor, MediaType.Json).Result;
                    if (apiResult.ResponseCode == 201)
                    {
                        productId = JsonConvert.DeserializeObject<ChannelAdvisorResultModel>(apiResult.JsonContent).ID;
                        storeService.ChangeStoreProduct(item.channelAdvisorProductModel.Sku, channelAdvisor.StoreId, productId, 0);
                        break;
                    }
                    else if (apiResult.ResponseCode == 400)
                        break;
                }
                if (productId == 0)
                    continue;
                foreach (var imageItem in item.Images)
                {
                    int responseCode1 = 0;
                    while (take == 5)
                    {
                        try
                        {
                            responseCode1 = apiRepo.PutInfo("v1/Products(" + productId + ")/Images('" + imageItem.PlacementName + i.ToString() + "')?access_token=" + accessToken + "", imageItem.UrlPath, Endpoints.ChannelAdvisor, MediaType.Json).Result;
                        }
                        catch (Exception) { continue; }
                        if (responseCode1 != 0)
                            break;
                    }
                    i++;
                }
                while (take == 5)
                {
                    ApiResult apiResult2 = apiRepo.Push("v1/Products(" + productId + ")/UpdateQuantity?access_token=" + accessToken + "", item.DCQuantitys, Endpoints.ChannelAdvisor, MediaType.Json).Result;
                    if (apiResult2.ResponseCode == 204)
                        break;
                }
            }

            return Ok();
        }

        [HttpPost]
        [Route("update-product")]
        public IActionResult UpdateProduct([FromBody] IntegrationApiModel channelAdvisor)
        {
            string accessToken = RefreshTokenCA();

            int take = 5;

            List<int> variantId = new List<int>();
            foreach (var item in channelAdvisor.ProductIds.ToList())
            {
                var variant = productVariantService.GetProductVariantByProductId(item);
                if (variant == null)
                    continue;
                if (channelAdvisor.ProductIds.Any(x => x == variant.BaseId))
                    continue;
                else
                {
                    variantId.Add(item);
                    channelAdvisor.ProductIds.Add(variant.BaseId.Value);
                }
            }

            var ProductSku = productService.GetProductSkubyId(channelAdvisor.ProductIds);
            ApiResult productModelResult = apiRepo.Post($"integration/ProductDetailProductSKUsByStoreWithLanguage/{channelAdvisor.StoreId}/{channelAdvisor.LanguageId}", ProductSku, "", Common.Enums.Endpoints.RegisApi, Common.Enums.MediaType.Json).Result;
            var productJsonModels = JsonConvert.DeserializeObject<List<ProductJsonModel>>(productModelResult.JsonContent);

            var productPriceandStockModel = apiRepo.GetList<ProductStockAndPriceJsonModel>($"integration/ProductStockAndPriceByStoreId/{channelAdvisor.StoreId}", "", Common.Enums.Endpoints.RegisApi, Common.Enums.MediaType.Json).Result;

            List<ChannelAdvisorUpdateVM> channelAdvisorUpdates = productService.JsonchannelAdvisorUpdateModels(channelAdvisor.StoreId, productJsonModels, productPriceandStockModel);

            foreach (var item in channelAdvisorUpdates)
            {
                int i = 1;
                while (take == 5)
                {
                    int responseCode1 = 0;
                    try
                    {
                        responseCode1 = apiRepo.PutInfo("v1/Products(" + item.productId + ")?access_token=" + accessToken + "", item.productUpdateModel, Endpoints.ChannelAdvisor, MediaType.Json).Result;
                    }
                    catch (Exception Ex) { continue; }
                    if (responseCode1 != 0)
                        break;
                }
                foreach (var imageItem in item.Images)
                {
                    int responseCode2 = 0;
                    while (take == 5)
                    {
                        try
                        {
                            responseCode2 = apiRepo.PutInfo("v1/Products(" + item.productId + ")/Images('" + imageItem.PlacementName + i.ToString() + "')?access_token=" + accessToken + "", imageItem.UrlPath, Endpoints.ChannelAdvisor, MediaType.Json).Result;
                        }
                        catch (Exception ex) { continue; }
                        if (responseCode2 != 0)
                            break;

                    }
                    i++;
                }
                foreach (var attribute in item.Value)
                {
                    attribute.ProfileID = channelAdvisor.ProfileId;
                    while (take == 5)
                    {
                        ApiResult apiResult = apiRepo.BulkPushProduct("v1/AttributeValues?access_token=" + accessToken + "", attribute, Endpoints.ChannelAdvisor, MediaType.Json).Result;
                        if (apiResult.JsonContent != null)
                            break;
                    }
                }
            }

            return Ok();
        }

        [Route("update-new-store-category/{storeId}")]
        public IActionResult UpdateNewStoreCategory(int storeId)
        {
            string accessToken = RefreshTokenCA();

            var channelAdvisorUpdates = productService.updateStoreCategory(storeId);
            int take = 5;
            foreach (var item in channelAdvisorUpdates)
            {
                foreach (var attribute in item.Value)
                {

                    attribute.ProfileID = 12010091;
                    while (take == 5)
                    {
                        ApiResult apiResult = apiRepo.BulkPushProduct("v1/AttributeValues?access_token=" + accessToken + "", attribute, Endpoints.ChannelAdvisor, MediaType.Json).Result;
                        if (apiResult.JsonContent != null)
                            break;
                    }
                }
            }
            return Ok();
        }

        [HttpPost]
        [Route("update-price/{storeId}")]
        public IActionResult UpdatePrice(int storeId)
        {
            string accessToken = RefreshTokenCA();

            int take = 5;

            List<ChannelAdvisorPriceUpdateModel> channelAdvisorPriceUpdateModels = productService.ChannelAdvisorPriceUpdateModel(storeId);

            foreach (var item in channelAdvisorPriceUpdateModels)
            {
                while (take == 5)
                {
                    int responseCode = apiRepo.PutInfo("v1/Products(" + item.ProductId + ")?access_token=" + accessToken + "", item.UpdatePrice, Endpoints.ChannelAdvisor, MediaType.Json).Result;
                    if (responseCode != 0)
                        break;
                }
            }

            return Ok();
        }

        [HttpPost]
        [Route("update-quantity/{storeId}")]
        public IActionResult UpdateQuantity(int storeId)
        {

            string accessToken = RefreshTokenCA();
            int infinity = 0;

            List<DCQuantityandID> dCQuantitys = productService.UpdateQuantity(storeId);

            foreach (var item in dCQuantitys)
            {
                try
                {
                    while (infinity == 0)
                    {
                        ApiResult apiResult = apiRepo.Push("v1/Products(" + item.ProductID + ")/UpdateQuantity?access_token=" + accessToken + "", item.dCQuantity, Endpoints.ChannelAdvisor, MediaType.Json).Result;
                        if (apiResult.JsonContent != null)
                            break;
                    }
                }
                catch (Exception)
                {

                }
            }


            return Ok();
        }

        [Route("update-quantity-all")]
        public IActionResult UpdateQuantityAllbyStoreId()
        {
            string accessToken = RefreshTokenCA();
            int take = 5;
            List<int> storeId = new List<int>() { 16, 20 };
            foreach (var store in storeId)
            {
                List<DCQuantityandID> dCQuantitys = productService.UpdateQuantityAll(store);

                foreach (var item in dCQuantitys)
                {
                    while (take == 5)
                    {
                        ApiResult apiResult = apiRepo.Push("v1/Products(" + item.ProductID + ")/UpdateQuantity?access_token=" + accessToken + "", item.dCQuantity, Endpoints.ChannelAdvisor, MediaType.Json).Result;
                        if (apiResult.JsonContent != null)
                            break;
                    }
                }
            }
            return Ok();
        }

        [Route("get-channel-advisor-productid/{profileId}/{store_id}")]
        public IActionResult GetChannelAdvisorProductId(int profileId, int store_id)
        {
            int count = 100;
            string accessToken = RefreshTokenCA();

            ChannelAdvisorGetProduct channelAdvisorGetProducts = new ChannelAdvisorGetProduct();

            channelAdvisorGetProducts = apiRepo.GetItem<ChannelAdvisorGetProduct>("v1/Products?access_token=" + accessToken + "&$filter=ProfileID eq " + profileId + "&$select=Sku,ID", "", Endpoints.ChannelAdvisor, MediaType.Json).Result;


            while (count != 0)
            {
                var GetProduct = apiRepo.GetItem<ChannelAdvisorGetProduct>("v1/Products?access_token=" + accessToken + "&$filter=ProfileID eq " + profileId + "&$select=Sku,ID&$skip=" + count + "", "", Endpoints.ChannelAdvisor, MediaType.Json).Result;

                channelAdvisorGetProducts.value.AddRange(GetProduct.value);

                if (GetProduct.value.Count != 100)
                    count = 0;
                else
                    count += 100;
            }

            storeService.AddStoreProduct(channelAdvisorGetProducts.value, store_id, channelAdvisorGetProducts.value.Count);

            return Ok();
        }

        [Route("push-product-all/{store_id}/{language_id}/{profileId}")]
        public IActionResult PushProductAll(int store_id, int language_id, int profileId)
        {

            string accessToken = RefreshTokenCA();
            //var storeProduct = storeService.StoreProducts(store_id).Where(x=>x.StoreProductId==0).ToList();
            List<ChannelAdvisorProductViewModel> channelAdvisorProductVMs = new List<ChannelAdvisorProductViewModel>();

            int productId = 0;
            int take = 5;

            var productJsonModels = apiRepo.GetList<ProductJsonModel>($"integration/ProductDetailByStoreWithLanguage/{store_id}/{language_id}", "", Common.Enums.Endpoints.RegisApi, Common.Enums.MediaType.Json).Result;
            var productPriceandStockModel = apiRepo.GetList<ProductStockAndPriceJsonModel>($"integration/ProductStockAndPriceByStoreId/{store_id}", "", Common.Enums.Endpoints.RegisApi, Common.Enums.MediaType.Json).Result;

            channelAdvisorProductVMs = productService.JsonchannelAdvisorProductVMs(store_id, productJsonModels, productPriceandStockModel);

            foreach (var item in channelAdvisorProductVMs)
            {
                //var product = productService.ProductBySku(item.channelAdvisorProductModel.Sku);
                //if (!storeProduct.Any(x => x.ProductId == product.Id))
                //    continue;
                int i = 1;

                ChannelAdvisorProductModel channelAdvisorProductModel = item.channelAdvisorProductModel;

                channelAdvisorProductModel.ProfileID = profileId;

                while (take == 5)
                {
                    ApiResult apiResult = apiRepo.Push("v1/products?access_token=" + accessToken, channelAdvisorProductModel, Endpoints.ChannelAdvisor, MediaType.Json).Result;
                    if (apiResult.ResponseCode == 201)
                    {
                        productId = JsonConvert.DeserializeObject<ChannelAdvisorResultModel>(apiResult.JsonContent).ID;
                        storeService.ChangeStoreProduct(item.channelAdvisorProductModel.Sku, store_id, productId, 0);
                        break;
                    }
                    else if (apiResult.ResponseCode == 400)
                        break;
                }
                if (productId == 0)
                    continue;
                foreach (var imageItem in item.Images)
                {
                    int responseCode1 = 0;
                    while (take == 5)
                    {
                        try
                        {
                            responseCode1 = apiRepo.PutInfo("v1/Products(" + productId + ")/Images('" + imageItem.PlacementName + i.ToString() + "')?access_token=" + accessToken + "", imageItem.UrlPath, Endpoints.ChannelAdvisor, MediaType.Json).Result;

                        }
                        catch (Exception ex) { }

                        if (responseCode1 != 0)
                            break;
                    }
                    i++;
                }

                while (take == 5)
                {
                    ApiResult apiResult2 = apiRepo.Push("v1/Products(" + productId + ")/UpdateQuantity?access_token=" + accessToken + "", item.DCQuantitys, Endpoints.ChannelAdvisor, MediaType.Json).Result;
                    if (apiResult2.ResponseCode == 204)
                        break;
                }
            }

            return Ok();
        }

        [Route("get-order")]
        public IActionResult ChannelAdvisorGetOrder()
        {
            string Token = RefreshTokenCA();

            List<IntegrationOrderDto> integrationOrders = new List<IntegrationOrderDto>();

            var orderModel = apiRepo.GetItem<ChannelAdvisorOrderModel>("v1/orders?access_token=" + Token + "&exported=false&$skip=479", "", Endpoints.ChannelAdvisor, MediaType.Json).Result;
            int i = 0;
            foreach (var item in orderModel.value.Where(x => !String.IsNullOrEmpty(x.ShippingAddressLine1)))
            {
                IntegrationOrderDto integrationOrder = new IntegrationOrderDto();
                if (item.SiteName == "Amazon Seller Central - AE" || item.SiteName == "Amazon Seller Central - US")
                {
                    ApiResult apiResult2 = apiRepo.Post<ApiResult>("v1/orders(" + item.ID + ")/Export?access_token=" + Token + "", "", Endpoints.ChannelAdvisor, MediaType.Json).Result;
                    continue;
                }
                if (!String.IsNullOrEmpty(item.BillingAddressLine1))
                {
                    integrationOrder = new IntegrationOrderDto()
                    {
                        Id = 0,
                        CreatedBy = 1,
                        CreateDate = DateTime.Now,
                        UpdatedBy = null,
                        UpdateDate = null,
                        OrderNo = CreateOrderID(Convert.ToInt32(item.ProfileID), item.SiteOrderID.ToString(), item.DistributionCenterTypeRollup),//hangi no,
                        ProjectCode = GetLogoInfo(Convert.ToInt32(item.ProfileID), item.DistributionCenterTypeRollup),//enumla project kodları al gelen site name ile bul
                        ShipmentCode = "",//ne
                        BillingAddressName = item.BillingFirstName + " " + item.BillingLastName,
                        BillingAddress = item.BillingAddressLine1 + " " + item.BillingAddressLine2,
                        BillingTown = item.BillingPostalCode.Replace(" ", ""),//boş geliyor sor,
                        BillingCity = item.BillingCity,
                        BillingTelNo1 = item.BillingDaytimePhone,
                        BillingTelNo2 = null,
                        BillingTelNo3 = null,
                        BillingPostalCode = item.BillingPostalCode.Replace(" ", ""),
                        BillingIdentityNumber = null,
                        BillingTaxOffice = null,
                        BillingCountry = GetCountryByCode(item.BillingCountry),
                        BillingEmail = item.BuyerEmailAddress,
                        BillingTaxNumber = null,
                        ShippingAddress = item.ShippingAddressLine1 + " " + item.ShippingAddressLine2,
                        ShippingAddressName = item.ShippingFirstName + " " + item.ShippingLastName,
                        ShippingCity = item.ShippingCity,
                        ShippingCountry = GetCountryByCode(item.ShippingCountry),
                        ShippingEmail = item.BuyerEmailAddress,
                        ShippingPostalCode = item.ShippingPostalCode.Replace(" ", ""),
                        ShippingTelNo1 = item.ShippingDaytimePhone,
                        ShippingTown = item.ShippingPostalCode.Replace(" ", ""),
                        ShippingCost = Convert.ToDecimal(item.TotalShippingPrice),
                        CollectionViaCreditCard = 0,//ne bu acaba
                        CollectionViaTransfer = 0,
                        Explanation1 = item.ID.ToString(),
                        Explanation2 = null,
                        Explanation3 = null,
                        ErrorMessage = null,
                        IsAccountCreated = true,//ne
                        IsAccountShippingCreated = false,
                        isExternal = false,
                        IsTransferred = false,
                        LastTryDate = null,
                        LogoCompanyCode = 918,//"bilmiyorum"
                        OrderDate = item.CreatedDateUtc,
                        ShippingTelNo2 = null,
                        ShippingTelNo3 = null,
                        Total = 0,
                        ThirdPartyTransactionId = item.ID.ToString(),
                        CountryCode = item.ShippingCountry

                    };
                }
                else
                {
                    integrationOrder = new IntegrationOrderDto()
                    {
                        Id = 0,
                        CreatedBy = 1,
                        CreateDate = DateTime.Now,
                        UpdatedBy = null,
                        UpdateDate = null,
                        OrderNo = CreateOrderID(Convert.ToInt32(item.ProfileID), item.SiteOrderID.ToString(), item.DistributionCenterTypeRollup),//hangi no,
                        ProjectCode = GetLogoInfo(Convert.ToInt32(item.ProfileID), item.DistributionCenterTypeRollup),//enumla project kodları al gelen site name ile bul
                        ShipmentCode = "",//ne
                        BillingAddressName = item.ShippingFirstName + " " + item.ShippingLastName,
                        BillingAddress = item.ShippingAddressLine1 + " " + item.ShippingAddressLine2,
                        BillingTown = item.ShippingPostalCode.Replace(" ", ""),//boş geliyor sor,
                        BillingCity = item.ShippingCity,
                        BillingTelNo1 = item.ShippingDaytimePhone,
                        BillingTelNo2 = null,
                        BillingTelNo3 = null,
                        BillingPostalCode = item.ShippingPostalCode.Replace(" ", ""),
                        BillingIdentityNumber = null,
                        BillingTaxOffice = null,
                        BillingCountry = GetCountryByCode(item.BillingCountry),
                        BillingEmail = item.BuyerEmailAddress,
                        BillingTaxNumber = null,
                        ShippingAddress = item.ShippingAddressLine1 + " " + item.ShippingAddressLine2,
                        ShippingAddressName = item.ShippingFirstName + " " + item.ShippingLastName,
                        ShippingCity = item.ShippingCity,
                        ShippingCountry = GetCountryByCode(item.ShippingCountry),
                        ShippingEmail = item.BuyerEmailAddress,
                        ShippingPostalCode = item.ShippingPostalCode.Replace(" ", ""),
                        ShippingTelNo1 = item.ShippingDaytimePhone,
                        ShippingTown = item.ShippingPostalCode.Replace(" ", ""),
                        ShippingCost = Convert.ToDecimal(item.TotalShippingPrice),
                        CollectionViaCreditCard = 0,//ne bu acaba
                        CollectionViaTransfer = 0,
                        Explanation1 = null,
                        Explanation2 = null,
                        Explanation3 = null,
                        ErrorMessage = null,
                        IsAccountCreated = true,//ne
                        IsAccountShippingCreated = false,
                        isExternal = false,
                        IsTransferred = false,
                        LastTryDate = null,
                        LogoCompanyCode = 918,//"bilmiyorum"
                        OrderDate = item.CreatedDateUtc,
                        ShippingTelNo2 = null,
                        ShippingTelNo3 = null,
                        Total = 0,
                        ThirdPartyTransactionId = item.ID.ToString(),
                        CountryCode = item.ShippingCountry

                    };
                }


                List<IntegrationOrderItemDto> orderItemDto = new List<IntegrationOrderItemDto>();

                var orderItemModel = apiRepo.GetItem<ChannelAdvisorOrderItemModel>("v1/orders(" + item.ID + ")/items?access_token=" + Token + "", "", Endpoints.ChannelAdvisor, MediaType.Json).Result;

                foreach (var orderItem in orderItemModel.value)
                {
                    if (item.DistributionCenterTypeRollup == "ExternallyManaged")
                    {
                        orderItemDto.Add(new IntegrationOrderItemDto()
                        {
                            SKU = orderItem.Sku,
                            Desi = 0,
                            OrderId = orderItem.OrderID,
                            ProductName = orderItem.Title,
                            Price = Convert.ToDecimal(orderItem.UnitPrice),
                            VAT = 19,
                            Currency = item.Currency,
                            CargoCurrency = item.Currency,
                            CargoPrice = Convert.ToDecimal(orderItem.ShippingPrice),
                            Quantity = orderItem.Quantity,
                        });
                    }
                    else if(item.ProfileID== 12010136)
                    {
                        orderItemDto.Add(new IntegrationOrderItemDto()
                        {
                            SKU = orderItem.Sku,
                            Desi = 0,
                            OrderId = orderItem.OrderID,
                            ProductName = orderItem.Title,
                            Price = Convert.ToDecimal(orderItem.UnitPrice*0.2205),
                            VAT = 19,
                            Currency = item.Currency,
                            CargoCurrency = item.Currency,
                            CargoPrice = Convert.ToDecimal(orderItem.ShippingPrice),
                            Quantity = orderItem.Quantity,
                        });
                    }
                    else
                    {
                        var product = productService.ProductBySku(orderItem.Sku);
                        orderItemDto.Add(new IntegrationOrderItemDto()
                        {
                            SKU = orderItem.Sku,
                            Desi = Convert.ToInt32(product.Desi.Value),
                            OrderId = orderItem.OrderID,
                            ProductName = orderItem.Title,
                            Price = Convert.ToDecimal(orderItem.UnitPrice),
                            VAT = 21,
                            Currency = item.Currency,
                            CargoCurrency = item.Currency,
                            CargoPrice = Convert.ToDecimal(orderItem.ShippingPrice),
                            Quantity = orderItem.Quantity,
                        });
                    }
                }

                integrationOrder.OrderItems = orderItemDto;

                ApiResult apiResult = apiRepo.Post($"order/save-order/{918}", integrationOrder, "", Endpoints.PMS, MediaType.Json).Result;

                if (apiResult.ResponseCode == 200)
                {
                    ApiResult apiResult2 = apiRepo.Post<ApiResult>("v1/orders(" + item.ID + ")/Export?access_token=" + Token + "", "", Endpoints.ChannelAdvisor, MediaType.Json).Result;
                    i++;
                }

                if (i == 5) break;
            }
            return Ok();
        }

        [Route("get-tracking-nember")]
        public IActionResult ChannelAdvisorUpdateTrackingId()
        {
            string Token = RefreshTokenCA();

            ChannelAdvisorTrackingNumberModel channelAdvisorTrackingNumber = new ChannelAdvisorTrackingNumberModel();

            ApiResult apiTrackingID = apiRepo.Post<OrderTrackingModel>($"order/get-tracking-numbers/{918}", "", Endpoints.PMS, MediaType.Json).Result;

            var trackingModel = JsonConvert.DeserializeObject<OrderTrackingModel>(apiTrackingID.JsonContent);

            foreach (var item in trackingModel.datas.Where(x => x.isActive).Take(5))
            {
                IntegrationOrderDto integrationOrder = new IntegrationOrderDto();
                TrackingNumberData trackingNumberData = new TrackingNumberData()
                {
                    DeliveryStatus = "Complete",
                    DistributionCenterID = 0,
                    SellerFulfillmentID = FullfillmentId(item.CompanyId),
                    ShippedDateUtc = DateTime.Now,
                    ShippingCarrier = item.CargoFirm,
                    ShippingClass = "Ground",
                    TrackingNumber = item.TrackingNumber
                };

                ApiResult apiOrderResult = apiRepo.Post<IntegrationOrderDto>($"order/get-order-by-orderno/{item.OrderNo}", "", Endpoints.PMS, MediaType.Json).Result;
                if (apiOrderResult.ResponseCode == 200)
                    integrationOrder = JsonConvert.DeserializeObject<IntegrationOrderDto>(apiOrderResult.JsonContent);
                else
                    continue;

                ApiResult apiCAResult = apiRepo.Post("v1/orders(" + integrationOrder.ThirdPartyTransactionId + ")/ship?access_token=" + Token + "", trackingNumberData, "", Endpoints.ChannelAdvisor, MediaType.Json).Result;
                if (apiCAResult.ResponseCode == 204)
                {
                    int responseCode = apiRepo.PutInfo($"order/update-tracking-numbers-shipping/{item.Id}", "", Endpoints.PMS, MediaType.Json).Result;
                }
            }
            return Ok();
        }

        public string FullfillmentId(int campaignId)
        {
            switch (campaignId)
            {
                case 918:
                    return "AYHLSLLV2FTKX";
                default:
                    return null;
            }
        }

        public string GetLogoInfo(int number, string distrubutionCenter)
        {
            if (distrubutionCenter == "ExternallyManaged")
                return "AMZ-GFB";
            switch (number)
            {
                case 12009949:
                    return "AMZ-GER";
                case 12010091:
                    return "AMZ-FR";
                case 12010087:
                    return "AMZ-UK";
                case 12010092:
                    return "";
                case 12010136:
                    return "ALL-POL";
                case 12010137:
                    return "";
                case 12010135:
                    return "";
                case 12010093:
                    return "";
                default:
                    return "";
            }
        }

        public string CreateOrderID(int number, string orderId, string distrubutionCenter)
        {
            string prefix;
            string neworderId;
            int startNumber;
            if (distrubutionCenter == "ExternallyManaged")
            {
                prefix = "AGEF-";
                neworderId = orderId.Replace("-", "");
                startNumber = neworderId.Length + prefix.Length - 16;
                neworderId = neworderId.Substring(startNumber, neworderId.Length - startNumber);
            }
            else
            {
                switch (number)
                {
                    case 12009949:
                        prefix = "AGER-";
                        neworderId = orderId.Replace("-", "");
                        startNumber = neworderId.Length + prefix.Length - 16;
                        neworderId = neworderId.Substring(startNumber, neworderId.Length - startNumber);
                        //neworderId = neworderId.Substring(0, 11);
                        break;
                    case 12010091:
                        prefix = "AFR-";
                        neworderId = orderId.Replace("-", "");
                        startNumber = neworderId.Length + prefix.Length - 16;
                        neworderId = neworderId.Substring(startNumber, neworderId.Length - startNumber);
                        //neworderId = neworderId.Substring(0, 12);
                        break;
                    case 12010087:
                        prefix = "AUK-";
                        neworderId = orderId.Replace("-", "");
                        startNumber = neworderId.Length + prefix.Length - 16;
                        neworderId = neworderId.Substring(startNumber, neworderId.Length - startNumber);
                        //neworderId = neworderId.Substring(0, 12);
                        break;
                    case 12010092:
                        return "";
                    case 12010136:
                        prefix = "ALL-";
                        neworderId = orderId.Replace("-", "");
                        startNumber = neworderId.Length + prefix.Length - 16;
                        neworderId = neworderId.Substring(startNumber, neworderId.Length - startNumber);
                        break;
                    case 12010137:
                        return "";
                    case 12010135:
                        return "";
                    case 12010093:
                        return "";
                    default:
                        return "";
                }
            }

            return prefix + neworderId;
        }

        public string GetCountryByCode(string countryCode)
        {
            Dictionary<string, string> CountriesDic = new Dictionary<string, string>();

            #region CountryAdd
            CountriesDic.Add("Afghanistan", "AF");
            CountriesDic.Add("Albania", "AL");
            CountriesDic.Add("Algeria", "DZ");
            CountriesDic.Add("American Samoa", "AS");
            CountriesDic.Add("Andorra", "AD");
            CountriesDic.Add("Angola", "AO");
            CountriesDic.Add("Anguilla", "AI");
            CountriesDic.Add("Antarctica", "AQ");
            CountriesDic.Add("Antigua and Barbuda", "AG");
            CountriesDic.Add("Argentina", "AR");
            CountriesDic.Add("Armenia", "AM");
            CountriesDic.Add("Aruba", "AW");
            CountriesDic.Add("Australia", "AU");
            CountriesDic.Add("Austria", "AT");
            CountriesDic.Add("Azerbaijan", "AZ");
            CountriesDic.Add("Bahamas (the)", "BS");
            CountriesDic.Add("Bahrain", "BH");
            CountriesDic.Add("Bangladesh", "BD");
            CountriesDic.Add("Barbados", "BB");
            CountriesDic.Add("Belarus", "BY");
            CountriesDic.Add("Belgium", "BE");
            CountriesDic.Add("Belize", "BZ");
            CountriesDic.Add("Benin", "BJ");
            CountriesDic.Add("Bermuda", "BM");
            CountriesDic.Add("Bhutan", "BT");
            CountriesDic.Add("Bolivia (Plurinational State of)", "BO");
            CountriesDic.Add("Bonaire, Sint Eustatius and Saba", "BQ");
            CountriesDic.Add("Bosnia and Herzegovina", "BA");
            CountriesDic.Add("Botswana", "BW");
            CountriesDic.Add("Bouvet Island", "BV");
            CountriesDic.Add("Brazil", "BR");
            CountriesDic.Add("British Indian Ocean Territory (the)", "IO");
            CountriesDic.Add("Brunei Darussalam", "BN");
            CountriesDic.Add("Bulgaria", "BG");
            CountriesDic.Add("Burkina Faso", "BF");
            CountriesDic.Add("Burundi", "BI");
            CountriesDic.Add("Cabo Verde", "CV");
            CountriesDic.Add("Cambodia", "KH");
            CountriesDic.Add("Cameroon", "CM");
            CountriesDic.Add("Canada", "CA");
            CountriesDic.Add("Cayman Islands (the)", "KY");
            CountriesDic.Add("Central African Republic (the)", "CF");
            CountriesDic.Add("Chad", "TD");
            CountriesDic.Add("Chile", "CL");
            CountriesDic.Add("China", "CN");
            CountriesDic.Add("Christmas Island", "CX");
            CountriesDic.Add("Cocos (Keeling) Islands (the)", "CC");
            CountriesDic.Add("Colombia", "CO");
            CountriesDic.Add("Comoros (the)", "KM");
            CountriesDic.Add("Congo (the Democratic Republic of the)", "CD");
            CountriesDic.Add("Congo (the)", "CG");
            CountriesDic.Add("Cook Islands (the)", "CK");
            CountriesDic.Add("Costa Rica", "CR");
            CountriesDic.Add("Croatia", "HR");
            CountriesDic.Add("Cuba", "CU");
            CountriesDic.Add("Curaçao", "CW");
            CountriesDic.Add("Cyprus", "CY");
            CountriesDic.Add("Czechia", "CZ");
            CountriesDic.Add("Côte d'Ivoire", "CI");
            CountriesDic.Add("Denmark", "DK");
            CountriesDic.Add("Djibouti", "DJ");
            CountriesDic.Add("Dominica", "DM");
            CountriesDic.Add("Dominican Republic (the)", "DO");
            CountriesDic.Add("Ecuador", "EC");
            CountriesDic.Add("Egypt", "EG");
            CountriesDic.Add("El Salvador", "SV");
            CountriesDic.Add("Equatorial Guinea", "GQ");
            CountriesDic.Add("Eritrea", "ER");
            CountriesDic.Add("Estonia", "EE");
            CountriesDic.Add("Eswatini", "SZ");
            CountriesDic.Add("Ethiopia", "ET");
            CountriesDic.Add("Falkland Islands (the) [Malvinas]", "FK");
            CountriesDic.Add("Faroe Islands (the)", "FO");
            CountriesDic.Add("Fiji", "FJ");
            CountriesDic.Add("Finland", "FI");
            CountriesDic.Add("France", "FR");
            CountriesDic.Add("French Guiana", "GF");
            CountriesDic.Add("French Polynesia", "PF");
            CountriesDic.Add("French Southern Territories (the)", "TF");
            CountriesDic.Add("Gabon", "GA");
            CountriesDic.Add("Gambia (the)", "GM");
            CountriesDic.Add("Georgia", "GE");
            CountriesDic.Add("Germany", "DE");
            CountriesDic.Add("Ghana", "GH");
            CountriesDic.Add("Gibraltar", "GI");
            CountriesDic.Add("Greece", "GR");
            CountriesDic.Add("Greenland", "GL");
            CountriesDic.Add("Grenada", "GD");
            CountriesDic.Add("Guadeloupe", "GP");
            CountriesDic.Add("Guam", "GU");
            CountriesDic.Add("Guatemala", "GT");
            CountriesDic.Add("Guernsey", "GG");
            CountriesDic.Add("Guinea", "GN");
            CountriesDic.Add("Guinea-Bissau", "GW");
            CountriesDic.Add("Guyana", "GY");
            CountriesDic.Add("Haiti", "HT");
            CountriesDic.Add("Heard Island and McDonald Islands", "HM");
            CountriesDic.Add("Holy See (the)", "VA");
            CountriesDic.Add("Honduras", "HN");
            CountriesDic.Add("Hong Kong", "HK");
            CountriesDic.Add("Hungary", "HU");
            CountriesDic.Add("Jamaica", "JM");
            CountriesDic.Add("Japan", "JP");
            CountriesDic.Add("Jersey", "JE");
            CountriesDic.Add("Jordan", "JO");
            CountriesDic.Add("Kazakhstan", "KZ");
            CountriesDic.Add("Kenya", "KE");
            CountriesDic.Add("Kiribati", "KI");
            CountriesDic.Add("Korea (the Democratic People's Republic of)", "KP");
            CountriesDic.Add("Korea (the Republic of)", "KR");
            CountriesDic.Add("Kuwait", "KW");
            CountriesDic.Add("Kyrgyzstan", "KG");
            CountriesDic.Add("Lao People's Democratic Republic (the)", "LA");
            CountriesDic.Add("Latvia", "LV");
            CountriesDic.Add("Lebanon", "LB");
            CountriesDic.Add("Lesotho", "LS");
            CountriesDic.Add("Liberia", "LR");
            CountriesDic.Add("Libya", "LY");
            CountriesDic.Add("Liechtenstein", "LI");
            CountriesDic.Add("Lithuania", "LT");
            CountriesDic.Add("Luxembourg", "LU");
            CountriesDic.Add("Macao", "MO");
            CountriesDic.Add("Madagascar", "MG");
            CountriesDic.Add("Malawi", "MW");
            CountriesDic.Add("Malaysia", "MY");
            CountriesDic.Add("Maldives", "MV");
            CountriesDic.Add("Mali", "ML");
            CountriesDic.Add("Malta", "MT");
            CountriesDic.Add("Marshall Islands (the)", "MH");
            CountriesDic.Add("Martinique", "MQ");
            CountriesDic.Add("Mauritania", "MR");
            CountriesDic.Add("Mauritius", "MU");
            CountriesDic.Add("Mayotte", "YT");
            CountriesDic.Add("Mexico", "MX");
            CountriesDic.Add("Micronesia (Federated States of)", "FM");
            CountriesDic.Add("Moldova (the Republic of)", "MD");
            CountriesDic.Add("Monaco", "MC");
            CountriesDic.Add("Mongolia", "MN");
            CountriesDic.Add("Montenegro", "ME");
            CountriesDic.Add("Montserrat", "MS");
            CountriesDic.Add("Morocco", "MA");
            CountriesDic.Add("Mozambique", "MZ");
            CountriesDic.Add("Myanmar", "MM");
            CountriesDic.Add("Namibia", "NA");
            CountriesDic.Add("Nauru", "NR");
            CountriesDic.Add("Nepal", "NP");
            CountriesDic.Add("Netherlands (the)", "NL");
            CountriesDic.Add("New Caledonia", "NC");
            CountriesDic.Add("New Zealand", "NZ");
            CountriesDic.Add("Nicaragua", "NI");
            CountriesDic.Add("Niger (the)", "NE");
            CountriesDic.Add("Nigeria", "NG");
            CountriesDic.Add("Niue", "NU");
            CountriesDic.Add("Norfolk Island", "NF");
            CountriesDic.Add("Northern Mariana Islands (the)", "MP");
            CountriesDic.Add("Norway", "NO");
            CountriesDic.Add("Oman", "OM");
            CountriesDic.Add("Pakistan", "PK");
            CountriesDic.Add("Palau", "PW");
            CountriesDic.Add("Palestine, State of", "PS");
            CountriesDic.Add("Panama", "PA");
            CountriesDic.Add("Papua New Guinea", "PG");
            CountriesDic.Add("Paraguay", "PY");
            CountriesDic.Add("Peru", "PE");
            CountriesDic.Add("Philippines (the)", "PH");
            CountriesDic.Add("Pitcairn", "PN");
            CountriesDic.Add("Poland", "PL");
            CountriesDic.Add("Portugal", "PT");
            CountriesDic.Add("Puerto Rico", "PR");
            CountriesDic.Add("Qatar", "QA");
            CountriesDic.Add("Republic of North Macedonia", "MK");
            CountriesDic.Add("Romania", "RO");
            CountriesDic.Add("Russian Federation (the)", "RU");
            CountriesDic.Add("Rwanda", "RW");
            CountriesDic.Add("Réunion", "RE");
            CountriesDic.Add("Saint Barthélemy", "BL");
            CountriesDic.Add("Saint Helena, Ascension and Tristan da Cunha", "SH");
            CountriesDic.Add("Saint Kitts and Nevis", "KN");
            CountriesDic.Add("Saint Lucia", "LC");
            CountriesDic.Add("Saint Martin (French part)", "MF");
            CountriesDic.Add("Saint Pierre and Miquelon", "PM");
            CountriesDic.Add("Saint Vincent and the Grenadines", "VC");
            CountriesDic.Add("Samoa", "WS");
            CountriesDic.Add("San Marino", "SM");
            CountriesDic.Add("Sao Tome and Principe", "ST");
            CountriesDic.Add("Saudi Arabia", "SA");
            CountriesDic.Add("Senegal", "SN");
            CountriesDic.Add("Serbia", "RS");
            CountriesDic.Add("Seychelles", "SC");
            CountriesDic.Add("Sierra Leone", "SL");
            CountriesDic.Add("Singapore", "SG");
            CountriesDic.Add("Sint Maarten (Dutch part)", "SX");
            CountriesDic.Add("Slovakia", "SK");
            CountriesDic.Add("Slovenia", "SI");
            CountriesDic.Add("Solomon Islands", "SB");
            CountriesDic.Add("Somalia", "SO");
            CountriesDic.Add("South Africa", "ZA");
            CountriesDic.Add("South Georgia and the South Sandwich Islands", "GS");
            CountriesDic.Add("South Sudan", "SS");
            CountriesDic.Add("Spain", "ES");
            CountriesDic.Add("Sri Lanka", "LK");
            CountriesDic.Add("Sudan (the)", "SD");
            CountriesDic.Add("Suriname", "SR");
            CountriesDic.Add("Svalbard and Jan Mayen", "SJ");
            CountriesDic.Add("Sweden", "SE");
            CountriesDic.Add("Switzerland", "CH");
            CountriesDic.Add("Syrian Arab Republic", "SY");
            CountriesDic.Add("Taiwan (Province of China)", "TW");
            CountriesDic.Add("Tajikistan", "TJ");
            CountriesDic.Add("Tanzania, United Republic of", "TZ");
            CountriesDic.Add("Thailand", "TH");
            CountriesDic.Add("Timor-Leste", "TL");
            CountriesDic.Add("Togo", "TG");
            CountriesDic.Add("Tokelau", "TK");
            CountriesDic.Add("Tonga", "TO");
            CountriesDic.Add("Trinidad and Tobago", "TT");
            CountriesDic.Add("Tunisia", "TN");
            CountriesDic.Add("Turkey", "TR");
            CountriesDic.Add("Turkmenistan", "TM");
            CountriesDic.Add("Turks and Caicos Islands (the)", "TC");
            CountriesDic.Add("Tuvalu", "TV");
            CountriesDic.Add("Uganda", "UG");
            CountriesDic.Add("Ukraine", "UA");
            CountriesDic.Add("United Arab Emirates (the)", "AE");
            CountriesDic.Add("United Kingdom", "GB");
            CountriesDic.Add("United States", "UM");
            CountriesDic.Add("United States America", "US");
            CountriesDic.Add("Uruguay", "UY");
            CountriesDic.Add("Uzbekistan", "UZ");
            CountriesDic.Add("Vanuatu", "VU");
            CountriesDic.Add("Venezuela (Bolivarian Republic of)", "VE");
            CountriesDic.Add("Viet Nam", "VN");
            CountriesDic.Add("Virgin Islands (British)", "VG");
            CountriesDic.Add("Virgin Islands (U.S.)", "VI");
            CountriesDic.Add("Wallis and Futuna", "WF");
            CountriesDic.Add("Western Sahara", "EH");
            CountriesDic.Add("Yemen", "YE");
            CountriesDic.Add("Zambia", "ZM");
            CountriesDic.Add("Zimbabwe", "ZW");
            CountriesDic.Add("Åland Islands", "AX");
            CountriesDic.Add("Iceland", "IS");
            CountriesDic.Add("India", "IN");
            CountriesDic.Add("Indonesia", "ID");
            CountriesDic.Add("Iran (Islamic Republic of)", "IR");
            CountriesDic.Add("Iraq", "IQ");
            CountriesDic.Add("Ireland", "IE");
            CountriesDic.Add("Isle of Man", "IM");
            CountriesDic.Add("Israel", "IL");
            CountriesDic.Add("Italy", "IT");
            #endregion

            var value = "";
            var strReturn = true;

            strReturn = CountriesDic.Select(x => x.Value).Contains(countryCode);
            if (strReturn)
                value = CountriesDic.Where(s => s.Value == countryCode).FirstOrDefault().Key;

            if (string.IsNullOrWhiteSpace(value))
                value = "Not Found";

            return value;
        }
        #endregion

        #region Joom
        [Route("refresh-token-joom")]
        public string JoomRefreshToken()
        {

            JoomRefreshTokenModel joomRefreshTokenModel = new JoomRefreshTokenModel();

            ApiResult apiResult = apiRepo.GetAuthorizeToken("oauth/refresh_token", joomRefreshTokenModel, "", Endpoints.Joomv2, MediaType.Encoded).Result;

            var accessToken = JsonConvert.DeserializeObject<JoomTokenRequestModel>(apiResult.JsonContent).Data.access_token;

            return accessToken;

        }

        [Route("push-product-joom/{store_id}/{language_id}")]
        public IActionResult JoomPushProduct(int store_id, int language_id)
        {
            string accessToken = JoomRefreshToken();
            var joomProduct = productService.joomProductModels(store_id, language_id);

            foreach (var item in joomProduct)
            {
                //int responseCode = apiRepo.GetExternalItem<ApiResult>("products?storeId=5ced38f036b54d03018a11fb&sku=" + item.sku + "", accessToken, Endpoints.Joomv3, MediaType.Json).Result;
                //if (responseCode == 200)
                //    continue;
                item.description = item.description != null ? HTMLParse.StripHTML(item.description) : string.Empty;
                ApiResult apiResult = apiRepo.Post("products/create?storeId=5ced38f036b54d03018a11fb", item, accessToken, Endpoints.Joomv3, MediaType.Json).Result;
                if (apiResult.ResponseCode == 200)
                    continue;
            }

            return Ok();
        }

        [Route("update-product-joom/{store_id}/{language_id}")]
        public IActionResult JoomUpdateProduct(int store_id, int language_id)
        {
            string accessToken = JoomRefreshToken();

            var joomProduct = productService.joomUpdateModels(store_id, language_id);

            foreach (var variant in joomProduct)
            {
                variant.access_token = accessToken;
                ApiResult apiResult = apiRepo.BatchPost("variant/update", variant, accessToken, Endpoints.Joomv2, MediaType.Json).Result;
            }

            return Ok();
        }

        [HttpPost]
        [Route("update-price-and-quantity")]
        public IActionResult UpdatePriceandQuantity()
        {
            string accessToken = JoomRefreshToken();

            var joomProduct = productService.joomUpdateModels(10, 2);

            foreach (var variant in joomProduct)
            {
                variant.access_token = accessToken;
                ApiResult apiResult = apiRepo.BatchPost("variant/update", variant, accessToken, Endpoints.Joomv2, MediaType.Json).Result;
            }

            return Ok();
        }
        #endregion
    }
}