using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProductDb.Common.Cache;
using ProductDb.ExportApi.Models.CaModels;
using Attribute = ProductDb.ExportApi.Models.CaModels.Attribute;

namespace ProductDb.ExportApi.Services.CaFactories
{
    public class CaFactory : ICaFactory
    {
        private readonly IIntegrationService integrationService;
        public CaFactory(IIntegrationService integrationService)
        {
            this.integrationService = integrationService;
        }
        public async Task<IEnumerable<CaProductViewModel>> PrepareProductViewModel(CaApiModel caApiModel)
        {
            var viewModelRedis = await integrationService.ProductDetailByStoreWithLanguage(caApiModel.StoreId, caApiModel.LanguageId, null, null);
            var viewModel = viewModelRedis.Where(x => x.StoreProductId == 0).Skip(0).Take(5);

            var subStores = integrationService.IntegrationSubNodesByStoreId(caApiModel.StoreId);

            var viewModelDatas = viewModel.ToList();
            int productCount = viewModelDatas.Count();
            int storeId = caApiModel.StoreId;
            List<CaProductViewModel> viewModels = new List<CaProductViewModel>();

            for (int i = 0; i < productCount; i++)
            {
                var caProductModel = new CaProductViewModel();
                var categoryName = viewModelDatas[i].Category.Split(">>").LastOrDefault();
                // product model
                caProductModel.channelAdvisorChildModel = new CaProductModel()
                {
                    Brand = viewModelDatas[i].Brand,
                    BuyItNowPrice = viewModelDatas[i].SellingPrice ?? 0,
                    Description = viewModelDatas[i].Description,
                    SupplierName = viewModelDatas[i].SupplierName,
                    ShortDescription = viewModelDatas[i].ShortDescription,
                    EAN = viewModelDatas[i].Barcode,
                    Manufacturer = viewModelDatas[i].SupplierName,
                    Sku = viewModelDatas[i].Sku,
                    Title = viewModelDatas[i].Title,
                    Classification = categoryName,
                    ProfileID = caApiModel.ProfileId
                };

                // product Attributes
                var attCount = viewModelDatas[i].Attributes.Count;
                var attList = viewModelDatas[i].Attributes.ToList();

                List<Attribute> viewAttList = new List<Models.CaModels.Attribute>();

                for (int y = 0; y < attCount; y++)
                {
                    viewAttList.Add(new Attribute()
                    {
                        Name = $"{attList[y].FieldName.ToUpperInvariant()}NEW",
                        Value = attList[y].Value
                    });
                }
                // current store category ID-Category just For Channel Advisor
                viewAttList.Add(new Attribute
                {
                    Name = storeId + "-Category",
                    Value = viewModelDatas[i].StoreCategory
                });

                // sub store category just For Channel Advisor
                var category = subStores.Where(x => x.ProductId == viewModelDatas[i].Id).Select(x => new Attribute()
                {
                    Name = x.Store.Name + "-Category",
                    Value = x.StoreCategory
                }).ToList();

                caProductModel.channelAdvisorChildModel.Attributes = new List<Attribute>();
                caProductModel.channelAdvisorChildModel.Attributes = viewAttList;
                // product label
                caProductModel.labels = new Label() { Name = viewModelDatas[i].Store };

                // product images
                var imgList = viewModelDatas[i].Pictures.ToList();
                var imgCount = imgList.Count;

                caProductModel.Images = new List<Image>();

                for (int k = 0; k < imgCount; k++)
                {
                    caProductModel.Images.Add(new Image()
                    {
                        PlacementName = $"{CaStatics.IMAGENAME}" + (k + 1),
                        UrlPath = new UrlPath() { Url = imgList[k].CdnPath }
                    });
                }
                // product quantity
                var quantity = viewModelDatas[i].Stock;

                caProductModel.DCQuantitys = new DCQuantity
                {
                    Value = new Value()
                    {
                        UpdateType = "InStock",
                        Updates = new List<Update>
                        {
                            new Update()
                            {
                                Quantity = quantity == null ? 0 : quantity.Value
                            }
                        }
                    }
                };

                viewModels.Add(caProductModel);
            }

            return viewModels;
        }
        public async Task<List<CaPriceUpdateModel>> PrepareProductPriceModel(CaApiModel caApiModel)
        {
            // Redis Cache
            var datas = await integrationService.ProductDetailByStoreWithLanguage(caApiModel.StoreId, caApiModel.LanguageId, null, null);
            datas = datas.Where(x => caApiModel.ProductIds.Any(k => k == x.Id));
            return (from d in datas
                    select new CaPriceUpdateModel
                    {
                        ProductId = d.Id,
                        UpdatePrice = new CaPrice
                        {
                            BuyItNowPrice = d.SellingPrice == null ? 0 : d.SellingPrice.Value
                        }
                    }).ToList();
        }
        public async Task<List<DCQuantityandID>> PrepareProductQuantityModel(CaApiModel caApiModel)
        {
            // Redis Cache
            var datas = await integrationService.ProductDetailByStoreWithLanguage(caApiModel.StoreId, caApiModel.LanguageId, null, null);
            datas = datas.Where(x => caApiModel.ProductIds.Any(k => k == x.Id));
            return (from d in datas
                    select new DCQuantityandID
                    {
                        ProductID = d.Id,
                        dCQuantity = new DCQuantity
                        {
                            Value = new Value
                            {
                                UpdateType = "InStock",
                                Updates = new List<Update>
                                {
                                    new Update
                                    {
                                        Quantity = d.Stock ?? 0
                                    }
                                }
                            }
                        }
                    }).ToList();
        }
    }
}
