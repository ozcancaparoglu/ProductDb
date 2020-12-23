using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PMS.Common.Dto;
using PMS.LogoService;
using PMS.LogoService.Helper;
using PMS.LogoService.LogoService;

namespace PMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FirmController : ControllerBase
    {
        private readonly ILogoService logoService;

        public FirmController(ILogoService logoService)
        {
            this.logoService = logoService;
        }

        [Route("FirmList")]
        public async Task<IActionResult> FirmList()
        {
            try
            {
                var firms = await logoService.ExecuteQuery(LogoHelper.GetCapiFirmQuery());

                var resultData = XmlSerializerHelper.ParseXMLToObject<CapiFirm>
                          (StringCompressor.UnzipBase64(firms.resultXML.ToString()),
                               "/RESULTXML/RESULTLINE");

                return Ok(resultData);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}