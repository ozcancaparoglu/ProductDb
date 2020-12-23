using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ProductDb.Common.Enums;
using ProductDb.Common.HttpClient;
using ProductDb.ExportApi.Common;
using ProductDb.ExportApi.Models.CaModels;
using ProductDb.ExportApi.Services.CaFactories;
using ProductDb.Logging;
using ProductDb.Services.ProductServices;

namespace ProductDb.ExportApi.Services.CaServices
{
    public class CaService : ICaService
    {
        private readonly ILoggerManager loggerManager;
        private readonly IApiRepo apiRepo;
        private readonly ICaFactory caFactory;
        private readonly IIntegrationService integrationService;

        public CaService(IIntegrationService integrationService, ICaFactory caFactory, IApiRepo apiRepo,
            ILoggerManager loggerManager)
        {
            this.loggerManager = loggerManager;
            this.apiRepo = apiRepo;
            this.caFactory = caFactory;
            this.integrationService = integrationService;
        }
        public string RefreshTokenCA()
        {
            var result = apiRepo.GetAuthorizeToken("oauth2/token", new CaTokenModel(),
                CaStatics.DeveloperToken, Endpoints.ChannelAdvisor, MediaType.Encoded).Result;

            var token = JsonConvert.DeserializeObject<CaTokenResultModel>(result.JsonContent);
            return token.access_token;
        }
        public async Task<ResultModel> PushAllProduct(CaApiModel caApiModel)
        {
            var processDate = DateTime.Now;

            loggerManager.LogInfo($"Starting Time : {processDate.ToLongDateString()} CA Process For Store : {caApiModel.ProfileId} , Language : {caApiModel.LanguageId}");
            try
            {
                var token = RefreshTokenCA();
                var PreparedCaViewModel = await caFactory.PrepareProductViewModel(caApiModel);
                var vmList = PreparedCaViewModel.ToList();
                var vmCount = vmList.Count;

                for (int i = 0; i < vmCount; i++)
                {
                    var result = PushProduct(vmList[i].channelAdvisorChildModel, token);
                    var imgList = vmList[i].Images;
                    int imgCount = imgList.Count;
                    if (result.ID != 0)
                    {
                        vmList[i].StoreProductId = result.ID;
                        // update store product id
                        integrationService.UpdateStoreProduct(vmList[i].channelAdvisorChildModel.Sku, caApiModel.StoreId, result.ID);
                        // image insert
                        for (int y = 0; y < imgCount; y++)
                        {
                            var res = InsertProductImage(imgList[y], result.ID, token);
                        }
                        // upload quantity
                        UpdateProductQuantity(vmList[i], result.ID, token);
                    }
                }
                loggerManager.LogInfo($"Stopping CA Process Time :" + processDate.ToLongDateString());

                return new ResultModel { message = "Successfull", status = true };
            }
            catch (Exception ex)
            {
                loggerManager.LogError(ex.Message.ToString());
                return new ResultModel { message = ex.Message, status = false };
            }
        }
        public async Task<ResultModel> UpdateAllPrice(CaApiModel caApiModel)
        {
            var processDate = DateTime.Now;

            loggerManager.LogInfo($"Starting Time : {processDate.ToLongDateString()} CA Process For Store : {caApiModel.ProfileId} , Language : {caApiModel.LanguageId}");
            try
            {
                var token = RefreshTokenCA();
                var PreparedCaViewModel = await caFactory.PrepareProductPriceModel(caApiModel);

                var vmList = PreparedCaViewModel.ToList();
                var vmCount = vmList.Count;

                for (int i = 0; i < vmCount; i++)
                {
                    UpdatePrice(vmList[i].ProductId, vmList[i].UpdatePrice.BuyItNowPrice, token);
                }
                loggerManager.LogInfo($"Stopping CA Process Time :" + processDate.ToLongDateString());

                return new ResultModel { message = "Successfull", status = true };
            }
            catch (Exception ex)
            {
                loggerManager.LogError(ex.Message.ToString());
                return new ResultModel { message = ex.Message, status = false };
            }
        }
        public async Task<ResultModel> UpdateAllQuantity(CaApiModel caApiModel)
        {
            var processDate = DateTime.Now;

            loggerManager.LogInfo($"Starting Time : {processDate.ToLongDateString()} CA Process For Store : {caApiModel.ProfileId} , Language : {caApiModel.LanguageId}");
            try
            {
                var token = RefreshTokenCA();
                var PreparedCaViewModel = await caFactory.PrepareProductQuantityModel(caApiModel);

                var vmList = PreparedCaViewModel.ToList();
                var vmCount = vmList.Count;

                for (int i = 0; i < vmCount; i++)
                {
                    UpdateProductQuantity(new CaProductViewModel { DCQuantitys = vmList[i].dCQuantity }, vmList[i].ProductID, token);
                }
                loggerManager.LogInfo($"Stopping CA Process Time :" + processDate.ToLongDateString());

                return new ResultModel { message = "Successfull", status = true };
            }
            catch (Exception ex)
            {
                loggerManager.LogError(ex.Message.ToString());
                return new ResultModel { message = ex.Message, status = false };
            }
        }
        private CaRequestResultModel PushProduct(CaProductModel productModel, string accessToken)
        {
            var request = apiRepo.Push("v1/products?access_token=" + accessToken, productModel, Endpoints.ChannelAdvisor, MediaType.Json).Result;

            return JsonConvert.DeserializeObject<CaRequestResultModel>(request.JsonContent);
        }
        private ApiResult UpdateProductQuantity(CaProductViewModel productModel, int productId, string accessToken)
        {
            return apiRepo.Push("v1/Products(" + productId + ")/UpdateQuantity?access_token=" + accessToken + "", productModel.DCQuantitys, Endpoints.ChannelAdvisor, MediaType.Json).Result;
        }
        private int InsertProductImage(Image image, int productId, string accessToken)
        {
            return apiRepo.PutInfo("v1/Products(" + productId + ")/Images('" + image.PlacementName + "')?access_token=" + accessToken + "", image.UrlPath, Endpoints.ChannelAdvisor, MediaType.Json).Result;
        }
        private int UpdatePrice(int productId, decimal price, string accessToken)
        {
            return apiRepo.PutInfo("v1/Products(" + productId + ")?access_token=" + accessToken + "", price, Endpoints.ChannelAdvisor, MediaType.Json).Result;
        }

        public Task<ResultModel> UpdateQuantity(CaApiModel caApiModel)
        {
            throw new NotImplementedException();
        }

        public Task<ResultModel> UpdatePrice(CaApiModel caApiModel)
        {
            throw new NotImplementedException();
        }
    }
}
