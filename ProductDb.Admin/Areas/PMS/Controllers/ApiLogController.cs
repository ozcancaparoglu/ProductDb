using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PMS.Common;
using ProductDb.Admin.Areas.PMS.Services;
using ProductDb.Admin.Areas.PMS.ViewModels;
using ProductDb.Admin.PageModels.Filter;
using ProductDb.Services.ErpComanyService;

namespace ProductDb.Admin.Areas.PMS.Controllers
{
    [Area("PMS")]
    [Route("PMS/log")]
    public class ApiLogController : Controller
    {
        private readonly IApiRepo apiRepo;
        private readonly IErpCompanyService erpCompanyService;
        private readonly IApiLogService apiLogService;

        public ApiLogController(IApiLogService apiLogService,IErpCompanyService erpCompanyService)
        {
            this.erpCompanyService = erpCompanyService;
            this.apiLogService = apiLogService;
        }

        [Route("LogList")]
        public IActionResult LogList()
        {
            var viewModel = new LogViewModel
            {
                Firms = erpCompanyService.ErpCompanies().ToList()
            };
            return View(viewModel);
        }


        [HttpPost]
        [Route("LogList-CompanyId/{companyId}")]
        public IActionResult GetLogList(int companyId, KendoFilterModel kendoFilterModel)
        {
            try
            {
                var logs = apiLogService.GetApiLogs(companyId, kendoFilterModel,out int total);
                return Json(new { total, data = logs });
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}