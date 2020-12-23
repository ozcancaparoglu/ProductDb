using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PMS.Common;
using PMS.Common.Dto;
using ProductDb.Admin.Helpers.LocalizationHelpers;
using ProductDb.Services.ErpComanyService;
using ProductDb.Services.PermissionServices;

namespace ProductDb.Admin.Areas.PMS.Controllers
{
    [Area("PMS")]
    [Route("PMS/company")]
    public class CompanyController : BaseController
    {
        private readonly IErpCompanyService erpCompanyService;
        public CompanyController(IApiRepo apiRepo,IErpCompanyService erpCompanyService,IUserRolePermissionService userRolePermissionService)
            : base(userRolePermissionService)
        {
            this.erpCompanyService = erpCompanyService;
        }

        [Route("SyncFirms")]
        public async Task<IActionResult> FirmList()
        {
            try
            {
                await erpCompanyService.SyncFirms();
                return Json(new { status = true, message = "Firm Syncronization Completed" });
            }
            catch (Exception ex)
            {
                return Json(new { status = false, message = ex.Message });
            }
        }
    }
}