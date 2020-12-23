using Newtonsoft.Json;
using PMS.Common;
using PMS.Common.Dto;
using ProductDb.Common.Cache;
using ProductDb.Common.Enums;
using ProductDb.Data.BiggBrandsDb;
using ProductDb.Data.LogoDb;
using ProductDb.Mapping.BiggBrandDbModels;
using ProductDb.Repositories.BaseRepositories;
using ProductDb.Repositories.UnitOfWorks;
using ProductDb.Services.ProductServices;
using ProductDb.Services.WarehouseServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace ProductDb.Services.LogoServices
{
    public class LogoService : ILogoService
    {
        private readonly IRedisCache redisCache;
        private readonly IUnitOfWork unitOfWork;
        private readonly IProductService productService;
        private readonly IWarehouseService warehouseService;
        private readonly IWarehouseQueryService logoWarehouseQueryService;
        private readonly IUnitOfWorkLogo unitOfWorkLogo;
        private readonly IApiRepo apiRepo;
        private readonly IGenericRepositoryLogo<URUN_WEB_STOK> urunWebStockRepo;
        private readonly IGenericRepositoryLogo<ProductStockModel> productStockModelRepo;
        private readonly IGenericRepository<WarehouseProductStock> WarehouseProductStockRepo;

        public LogoService(IUnitOfWorkLogo unitOfWorkLogo,
                          IWarehouseQueryService logoWarehouseQueryService,
                          IWarehouseService warehouseService,
                          IProductService productService,
                          IUnitOfWork unitOfWork,
                          IRedisCache redisCache, IApiRepo apiRepo)
        {
            this.redisCache = redisCache;
            this.unitOfWork = unitOfWork;
            this.productService = productService;
            this.warehouseService = warehouseService;
            this.logoWarehouseQueryService = logoWarehouseQueryService;
            this.unitOfWorkLogo = unitOfWorkLogo;
            this.apiRepo = apiRepo;
            WarehouseProductStockRepo = this.unitOfWork.Repository<WarehouseProductStock>();
            urunWebStockRepo = this.unitOfWorkLogo.Repository<URUN_WEB_STOK>();
            productStockModelRepo = this.unitOfWorkLogo.Repository<ProductStockModel>();
        }

        private IList<URUN_WEB_STOK> ExecuteQuery(string query)
        {
            var DataList = urunWebStockRepo.ExecuteRawQuery(query).ToList();

            return DataList;
        }

        private IList<ProductStockModel> ExecuteNewQuery(string query)
        {
            var DataList = productStockModelRepo.ExecuteRawQuery(query).ToList();
            return DataList;
        }

        private string GetStockQueryByWarehouseId(int WarehouseTypeId, List<string> SKUs, int? AMBAR_ID = null)
        {
            string SQlquery = string.Empty;
            var query = logoWarehouseQueryService.GetLogoWarehouseQueryModelByWarehouse(WarehouseTypeId, SKUs, AMBAR_ID);
            if (query != null)
                SQlquery = query.Query;

            return SQlquery;
        }

        public void UpdateProductStockFromLogo()
        {
            var WarehouseTypes = warehouseService.WarehouseTypes();
            string query = string.Empty;
            DateTime now = DateTime.Now;
            List<WarehouseProductStock> warehouseProductStocks = new List<WarehouseProductStock>();
            List<URUN_WEB_STOK> list = new List<URUN_WEB_STOK>();

            foreach (var itemW in WarehouseTypes)
            {
                var queryString = string.Empty;
                var WarehouseProductStocks = WarehouseProductStockRepo.FindAll(a => a.State == (int)State.Active && a.WarehouseTypeId == itemW.Id).ToList();
                var productWarehouseStock = WarehouseProductStockRepo.Filter(a => a.WarehouseTypeId == itemW.Id).ToList();
                int skipTakeCount = WarehouseProductStocks.Count() / 1000;

                for (int i = 0; i <= skipTakeCount; i++)
                {
                    var skip = i * 1000;

                    var productStock = WarehouseProductStocks.Skip(skip).Take(1000);

                    queryString = GetStockQueryByWarehouseId(itemW.Id, productStock.Select(a => a.Sku).ToList(), itemW.LogoWarehouseId);

                    if (!string.IsNullOrWhiteSpace(queryString))
                    {
                        query = string.Format(queryString, itemW.Id.ToString());
                        
                        var webStockList = ExecuteQuery(query);

                        foreach (var item in webStockList)
                        {
                            var listEntity = productWarehouseStock.Where(x => x.Sku == item.Sku).FirstOrDefault();

                            if (listEntity != null)
                            {
                                listEntity.UpdatedDate = now;
                                listEntity.Quantity = item.Stock.Value;
                                warehouseProductStocks.Add(listEntity);
                            }
                        }

                        list.AddRange(webStockList);
                    }
                }

                var expectedSKUList = WarehouseProductStocks.Except(
                                        WarehouseProductStocks.Where(a => list.Any(x => x.Sku == a.Sku))).ToList();

                expectedSKUList.ForEach(a => a.Quantity = 0);
                expectedSKUList.ForEach(a => a.UpdatedDate = now);
                WarehouseProductStockRepo.BulkUpdate(expectedSKUList);
            }

            warehouseProductStocks.ForEach(a => { a.UpdatedDate = now; a.WarehouseType = null; });
            WarehouseProductStockRepo.BulkUpdate(warehouseProductStocks.GroupBy(g => g.Id).Select(s => s.First()).ToList());
        }

        public void SyncProductWarehouseStock()
        {
            var types = warehouseService.WarehouseTypes().Select(a => a.Id);
            var SKUs = WarehouseProductStockRepo.Table();
            var groupedSKUs = SKUs.GroupBy(a => new { a.Sku, a.ProductId, a.Name }).ToList();
            var Date = DateTime.Now;
            foreach (var item in groupedSKUs)
            {
                var typeList = SKUs.Where(a => a.Sku == item.Key.Sku).Select(a => a.WarehouseTypeId);
                var expceptedList = types.Except(typeList.Where(a => types.Any(b => b == a)).ToList());
                if (expceptedList.Count() > 0)
                {
                    foreach (var itemW in expceptedList)
                    {
                        WarehouseProductStockRepo.Add(new WarehouseProductStock()
                        {
                            ProductId = item.Key.ProductId,
                            Sku = item.Key.Sku,
                            State = (int)State.Active,
                            Name = item.Key.Name,
                            Quantity = 0,
                            WarehouseTypeId = itemW,
                            CreatedDate = Date,
                            UpdatedDate = Date,
                            ProcessedBy = 0
                        });
                    }
                }
            }
            //var stockWarehouseTypeList = WarehouseProductStockRepo.Table().GroupBy(a => a.WarehouseTypeId).Select(a => a.Key).ToList();
            //var typeListIDs = types.Except(stockWarehouseTypeList.Where(a => types.Any(b => b == a)).ToList());
            //if (typeListIDs.Count() > 0)
            //{
            //    var Date = DateTime.Now;
            //    var SkuList = (from s in WarehouseProductStockRepo.Table()
            //                   where s.State == (int)State.Active
            //                   group s by new { s.Sku, s.ProductId, s.Name } into grp
            //                   select grp).ToList();
            //    foreach (var itemW in typeListIDs)
            //    {
            //        foreach (var item in SkuList)
            //        {
            //            WarehouseProductStockRepo.Add(new WarehouseProductStock()
            //            {
            //                ProductId = item.Key.ProductId,
            //                Sku = item.Key.Sku,
            //                State = (int)State.Active,
            //                Name = item.Key.Name,
            //                Quantity = 0,
            //                WarehouseTypeId = itemW,
            //                CreatedDate = Date,
            //                UpdatedDate = Date,
            //                ProcessedBy = 0
            //            });
            //        }
            //    }
            //}
        }

        public IEnumerable<LogoProduct> GetOnnetStockQuantityFromLogoByWarehouse(List<string> SKUs)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///  Push Redis Cache All Products
        /// </summary>
        /// <returns></returns>
        public async Task PushAllProductToRedisAsync()
        {
            if (!redisCache.IsCached(CacheStatics.LogoProductStockCache))
            {
                var query = LogoQueryStatics.GetLogoStockWarehouseQuery();
                var dataList = ExecuteNewQuery(query);
                await redisCache.SetAsync(CacheStatics.LogoProductStockCache, dataList, CacheStatics.LogoProductStockCacheTime);
            }
        }

        /// <summary>
        ///  Update Products From Redis Cache
        /// </summary>
        /// <returns></returns>
        public async Task UpdateProductStockFromRedis()
        {
            if (!redisCache.IsCached(CacheStatics.LogoProductStockCache))
                await PushAllProductToRedisAsync();

            var stockListFromRedis = await redisCache.GetAsync<List<ProductStockModel>>(CacheStatics.LogoProductStockCache);

            var WarehouseTypes = warehouseService.WarehouseTypes().Where(x => x.LogoWarehouseId != null);
            string query = string.Empty;
            DateTime now = DateTime.Now;

            List<WarehouseProductStock> warehouseProductStocks = new List<WarehouseProductStock>();

            foreach (var itemW in WarehouseTypes)
            {
                var productWarehouseStock = WarehouseProductStockRepo.Filter(a => a.WarehouseTypeId == itemW.Id).ToList();
                // Stock List From Redis
                var stockList = stockListFromRedis.Where(x => x.WarehouseNumber == itemW.LogoWarehouseId);
                // Firm Code by Warehousetype
                var queryCodes = logoWarehouseQueryService.GetWarehouseQueriesByWarehouseType(itemW.Id);
                // Firm Code Filter
                if (queryCodes != null)
                {
                    stockList = stockList.Where(x => queryCodes.Query.Split(",").Any(k => x.FirmCode.ToString() == k));
                    foreach (var itemR in stockList)
                    {
                        var entity = productWarehouseStock.FirstOrDefault(x => x.Sku == itemR.Code);
                        if (entity != null)
                        {
                            entity.Quantity = itemR.Quantity.Value;
                            warehouseProductStocks.Add(entity);
                        }
                    }

                    var expectedSKUList = productWarehouseStock.Except(
                                          productWarehouseStock.Where(a => stockList.Any(x => x.Code == a.Sku))).ToList();

                    expectedSKUList.ForEach(a => a.Quantity = 0);
                    expectedSKUList.ForEach(a => a.UpdatedDate = now);
                    WarehouseProductStockRepo.BulkUpdate(expectedSKUList);
                }
            }

            warehouseProductStocks.ForEach(a => a.UpdatedDate = now);
            WarehouseProductStockRepo.BulkUpdate(warehouseProductStocks);
        }

        public async Task<LogoResponseDto> AddProductToLogo(ProductModel product, int companyId)
        {
            var _itemCard = PrepareItemCard(product);
            var response = await apiRepo.Post($"item/insertitem/{companyId}", _itemCard, "", PMS.Common.Endpoints.PMS);
            var message = JsonConvert.DeserializeObject<LogoResponseDto>(response.JsonContent);

            return message;
        }

        public ItemCard PrepareItemCard(ProductModel product)
        {
            return new ItemCard
            {
                AbroadDesi = product.AbroadDesi,
                Alias = product.Alias,
                Barcode = product.Barcode,
                //Brand = product.ParentProduct.BrandPP == null ? string.Empty : product.ParentProduct.BrandPP.Name,
                Brand = product.BrandId == null ? product.ParentProduct.BrandPP.Name : product.Brand.Name,
                BuyingPrice = product.BuyingPrice,
                Description = product.Description,
                Desi = product.Desi,
                CorporatePrice = product.CorporatePrice,
                MetaDescription = product.MetaDescription,
                MetaKeywords = product.MetaKeywords,
                Model = product.Model,
                Name = product.Name,
                Sku = product.Sku,
                Title = product.Title,
                ShortDescription = product.ShortDescription,
                VatRate = product.VatRate == null ? 0 : product.VatRate.Value
            };
        }

        public async Task<IEnumerable<CapiFirm>> SyncFirms()
        {
            var firmList = await apiRepo.GetItem<IEnumerable<CapiFirm>>($"firm/firmList", "", PMS.Common.Endpoints.PMS);
            return firmList;
        }
    }
}
