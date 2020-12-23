using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PMS.API.Models;
using PMS.Service.LoggingService;

namespace PMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogController : ControllerBase
    {
        private readonly IDbLoggingService _dbLoggingService;

        public LogController(IDbLoggingService dbLoggingService)
        {
            _dbLoggingService = dbLoggingService;
        }

        [HttpGet]
        [Route("get-logs-count/{companyId}")]
        public IActionResult GetLogsCount(int companyId)
        {
            var datas = _dbLoggingService.Total(companyId);

            return Ok(datas);
        }


        [HttpPost]
        [Route("get-logs/{companyId}")]
        public IActionResult GetLogs(int companyId, KendoFilterDto kendoFilterDto)
        {
            int total = 0;
            var datas = KendoFilterHelperDto.FilterQueryableData(_dbLoggingService.GetQueryableLogs(companyId), kendoFilterDto);
            total = datas.Count();

            datas = datas.Skip(kendoFilterDto.skip).Take(kendoFilterDto.take);
            return Ok(new { datas = datas, total = total });
        }
    }
}