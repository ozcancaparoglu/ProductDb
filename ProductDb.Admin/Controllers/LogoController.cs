using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductDb.Data.LogoDb;
using ProductDb.Logging;
using ProductDb.Services.CalculationServices;
using ProductDb.Services.LogoServices;
using ProductDb.Services.StoreServices;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProductDb.Admin.Controllers
{
    [AllowAnonymous]
    [Route("logo")]
    public class LogoController : Controller
    {
        private readonly ILoggerManager _logger;
        private readonly ILogoService logoService;
        private readonly IStoreService storeService;
        private readonly ICalculationService calculationService;

        public LogoController(ILogoService logoService, IStoreService storeService, ICalculationService calculationService, ILoggerManager logger)
        {
            _logger = logger;
            this.logoService = logoService;
            this.storeService = storeService;
            this.calculationService = calculationService;
        }

        [HttpPost]
        [Route("update-productstockswarehousetype")]
        public ContentResult UpdateProductStockWarehoyseTypes()
        {
            try
            {
                logoService.SyncProductWarehouseStock();
                return Content("synchronization is completed.");
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }

        [HttpPost]
        [Route("update-warehouses")]
        public ContentResult UpdateStockFromLogo()
        {
            try
            {
                logoService.UpdateProductStockFromLogo();
                return Content("Logo Stok Up To Date");
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }

        [HttpPost]
        [Route("LogoStockRedisCache")]
        public async Task<ContentResult> LogoStockRedisCache()
        {
            try
            {
                await logoService.PushAllProductToRedisAsync();
                return Content("Redis Logo Stock Updated");
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }

        [HttpPost]
        [Route("StockUpdateFromRedis")]
        public async Task<ContentResult> StockUpdateFromRedis()
        {
            try
            {
                await logoService.UpdateProductStockFromRedis();
                return Content("Redis Stock Updated");
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }

        [HttpPost]
        [Route("update-stocks")]
        public ContentResult UpdateStoreStocks()
        {
            try
            {
                storeService.UpdateStoreProducts();
                return Content("Store Stoks Up To Date");
            }
            catch (System.Exception ex)
            {
                return Content(ex.Message);
            }

        }

        [HttpPost]
        [Route("get-onnet-logo-stocks")]
        public IEnumerable<LogoProduct> GetOnnetLogoStocks([FromBody] List<LogoProduct> SKUs)
        {
            try
            {
                List<string> SKUs_ = new List<string>();

                foreach (var item in SKUs)
                    SKUs_.Add(item.SKU);
                
                var data = logoService.GetOnnetStockQuantityFromLogoByWarehouse(SKUs_);
                return data;
            }
            catch
            {
                return null;
            }
        }

        [HttpPost]
        [Route("calculate-prices")]
        public ContentResult CalculatePrices()
        {
            try
            {
                calculationService.CalculateAllPrices();
                return Content("All Prices Updated");
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }
    }
}