using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PMS.Common;
using PMS.Common.Dto;
using ProductDb.Admin.Helpers.LocalizationHelpers;
using ProductDb.Mapping.BiggBrandDbModels;
using ProductDb.Services.ErpComanyService;
using ProductDb.Services.PermissionServices;
using ProductDb.Services.ProductServices;
using ProductDb.Services.TaxServices;

namespace ProductDb.Admin.Areas.PMS.Controllers
{
    [Area("PMS")]
    [Route("PMS/item")]
    public class ItemController : BaseController
    {
        private readonly IProductService productService;
        private readonly IErpCompanyService erpCompanyService;

        public ItemController(IErpCompanyService erpCompanyService,
                              IProductService productService,
                              IUserRolePermissionService userRolePermissionService)
            : base(userRolePermissionService)
        {
            this.productService = productService;
            this.erpCompanyService = erpCompanyService;
        }

        [HttpPost]
        [Route("InsertItem/{productId}/{companyId}")]
        public async Task<IActionResult> InsertItem(int productId, int companyId)
        {
            try
            {
                var _product = productService.ProductById(productId);
                var firmCode = erpCompanyService.ErpCompanies().FirstOrDefault(x => x.FirmNo == companyId);
                var result = await erpCompanyService.SaveProductToCompany(firmCode.Id, _product);
                return Json(new { status = true, message = result });
            }
            catch (Exception ex)
            {
                return Json(new { status = false, message = ex.Message });
            }
        }


    }
}