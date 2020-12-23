using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductDb.ExportApi.Models;
using ProductDb.ExportApi.Models.CaModels;
using ProductDb.ExportApi.Services.CaServices;

namespace ProductDb.ExportApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CAController : ControllerBase
    {
        private readonly ICaService caService;

        public CAController(ICaService caService)
        {
            this.caService = caService;
        }

        [HttpPost]
        [Route("UpdateAllPrice")]
        public async Task<IActionResult> UpdateAllPrice([FromBody] CaApiModel caApiModel)
        {
            try
            {
                var result = await caService.UpdateAllPrice(caApiModel);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("PushAllProducts")]
        public async Task<IActionResult> PushAllProducts([FromBody] CaApiModel caApiModel)
        {
            try
            {
                var result = await caService.PushAllProduct(caApiModel);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("UpdateAllQuantity")]
        public async Task<IActionResult> UpdateAllQuantity([FromBody] CaApiModel caApiModel)
        {
            try
            {
                var result = await caService.UpdateAllQuantity(caApiModel);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }



    }
}