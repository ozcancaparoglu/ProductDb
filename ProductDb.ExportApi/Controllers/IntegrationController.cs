using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductDb.ExportApi.DTOs;
using ProductDb.ExportApi.Services;
using ProductDb.Mapping.BiggBrandDbModels;

namespace ProductDb.ExportApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IntegrationController : ControllerBase
    {
        private readonly IIntegrationService integrationService;

        public IntegrationController(IIntegrationService integrationService)
        {
            this.integrationService = integrationService;
        }

        [Route("ProductDetailById/{productId}/{languageId}")]
        public IActionResult ProductDetailById(int productId, int languageId)
        {
            try
            {
                var productDetail = integrationService.ProductDetailById(productId, languageId);
                return Ok(productDetail);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message.ToString());
            }
        }

        [Route("ProductDetailByStoreWithLanguage/{storeId}/{languageId}/{startIndex?}/{endIndex?}")]
        public async Task<IActionResult> ProductDetailByStoreWithLanguage(int storeId, int languageId, int? startIndex, int? endIndex)
        {
            try
            {
                var productDetails = await integrationService.ProductDetailByStoreWithLanguage(storeId, languageId, startIndex, endIndex);

                return Ok(productDetails);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message.ToString());
            }
        }

        [Route("ProductDetailProductSKUsByStoreWithLanguage/{storeId}/{languageId}")]
        [HttpPost]
        public async Task<IActionResult> ProductDetailProductSKUsByStoreWithLanguage([FromBody] List<string> SKUs, int storeId, int languageId)
        {
            try
            {
                var productDetails = await integrationService.ProductDetailByStoreWithLanguage(storeId, languageId, SKUs);

                return Ok(productDetails);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message.ToString());
            }
        }

        [Route("AllActiveStores")]
        public async Task<IActionResult> AllActiveStores()
        {
            try
            {
                var result = await integrationService.StoreModels();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("ProductStockAndPriceByStoreId/{storeId}")]
        public IActionResult ProductStockAndPriceByStoreId(int storeId)
        {
            try
            {
                var result = integrationService.ProductStockAndPriceByStoreId(storeId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        // Clear Cache  By Store And Lannguage Id
        [Route("ClearCache/{storeId}/{languageId}")]
        public IActionResult ClearCache(int storeId,int languageId)
        {
            try
            {
                integrationService.ClearCache(storeId, languageId);
                return Ok("Cache Cleared");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        #region MockForTesing
        [Route("ProductDetailByStoreWithLanguageMock/{storeId}/{languageId}/{startIndex?}/{endIndex?}")]
        public async Task<IActionResult> ProductDetailByStoreWithLanguageMock(int storeId, int languageId, int? startIndex, int? endIndex)
        {
            try
            {
                var productDetails = await integrationService.ProductDetailByStoreWithLanguageMock(storeId, languageId, startIndex, endIndex);

                return Ok(productDetails);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message.ToString());
            }
        }
        #endregion

        [Route("GetStoreLanguage/{langId}")]
        public IActionResult GetStoreLanguage(int langId)
        {
            try
            {
                var result = integrationService.GetStoreLanguage(langId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}