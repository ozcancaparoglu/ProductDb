using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductDb.Admin.Helpers.LocalizationHelpers;
using ProductDb.Admin.PageModels.Filter;
using ProductDb.Mapping.BiggBrandDbModels;
using ProductDb.Services.LanguageServices;
using ProductDb.Services.PermissionServices;
using ProductDb.Services.WarehouseServices;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProductDb.Admin.Controllers
{
    [MiddlewareFilter(typeof(LocalizationPipeline))]
    [Authorize]
    [Route("{lang}/warehouse")]
    public class WarehouseController : BaseController
    {

        private readonly IWarehouseService warehouseService;
        private readonly IWarehouseQueryService warehouseQueryService;

        public WarehouseController(ILanguageService languageService,
            IWarehouseService warehouseService,
            IWarehouseQueryService warehouseQueryService,
            IUserRolePermissionService userRolePermissionService) : base(languageService, userRolePermissionService)
        {
            this.warehouseQueryService = warehouseQueryService;
            this.warehouseService = warehouseService;
        }


        [Authorize]
        [Route("warehouseTypeSetState/{id}")]
        public IActionResult WarehouseTypeSetState(int id)
        {
            warehouseService.SetWarehoseTypeState(id);

            return RedirectToAction("warehouseTypelist", new { lang = CurrentLanguage });
        }

        [Authorize]
        [Route("warehouseQuerySetState/{id}")]
        public IActionResult WarehouseQuerySetState(int id)
        {
            warehouseQueryService.SetWarehoseTypeState(id);

            return RedirectToAction("warehouseQuerylist", new { lang = CurrentLanguage });
        }

        [Route("warehouseTypeList")]
        public IActionResult WarehouseTypeList(KendoFilterModel kendoFilterModel)
        {
            var model = warehouseService.WarehouseTypes();
            return View(model);
        }
        [Route("createWarehouseType")]
        public IActionResult CreateWarehousetype()
        {
            var model = new WarehouseTypeModel();
            return View(model);
        }

        [HttpPost]
        [Route("createWarehouseType")]
        public IActionResult CreateWareHousetype(WarehouseTypeModel warehouseTypeModel)
        {
            var data = warehouseService.AddWarehouseType(warehouseTypeModel);
            return RedirectToAction("warehouseTypelist", new { lang = CurrentLanguage });
        }

        [HttpGet]
        [Route("editWarehouseType/{id}")]
        public IActionResult EditWareHouseType(int id)
        {
            var model = warehouseService.WarehouseTypeById(id);
            return View(model);
        }

        [HttpPost]
        [Route("editWarehouseType/{id}")]
        public IActionResult EditWareHouseType(WarehouseTypeModel warehouseTypeModel)
        {
            var data = warehouseService.EditWarehouseType(warehouseTypeModel);
            return RedirectToAction("warehouseTypelist", new { lang = CurrentLanguage });
        }

        [Route("warehouseQueryList")]
        public IActionResult WarehouseQueryList()
        {
            var model = warehouseQueryService.WarehouseQueryModels();
            return View(model);
        }

        [Route("createWarehouseQuery")]
        public IActionResult CreateWarehouseQuery()
        {
            var model = new WarehouseQueryModel
            {
                WarehouseTypeModels = warehouseService.WarehouseTypes()
            };
            return View(model);
        }

        [HttpPost]
        [Route("createWarehouseQuery")]
        public IActionResult CreateWarehouseQuery(WarehouseQueryModel warehouseTypeModel)
        {
            var data = warehouseQueryService.AddLogoQuery(warehouseTypeModel);
            //data.WarehouseTypeModels = warehouseService.GetWarehouseTypes();

            return RedirectToAction("warehouseQuerylist", new { lang = CurrentLanguage });
        }

        [Route("editWarehouseQuery/{id}")]
        public IActionResult EditWarehouseQuery(int id)
        {
            var model = warehouseQueryService.GetQueryById(id);
            model.WarehouseTypeModels = warehouseService.WarehouseTypes();
            return View(model);
        }

        [HttpPost]
        [Route("editWarehouseQuery/{id}")]
        public IActionResult EditWarehouseQuery(WarehouseQueryModel warehouseTypeModel)
        {
            var data = warehouseQueryService.EditLogoQuery(warehouseTypeModel);
            return RedirectToAction("warehouseQuerylist", new { lang = CurrentLanguage });
        }

        [Route("warehouseStockList")]
        public IActionResult WarehouseStockList()
        {
            return View();
        }

        [HttpPost]
        [Route("get-all-warehouseStockList")]
        public JsonResult GetAllWarehouseStockList(KendoFilterModel kendoFilterModel)
        {
            try
            {
                IEnumerable<WarehouseProductStockModel> datas = null;
                int total;

                if (kendoFilterModel.filter == null)
                {
                    datas = warehouseService.WarehouseProductStockModelsQueryableFiltered(kendoFilterModel.skip, kendoFilterModel.take, out total);
                }
                else
                {
                    var qDatas = warehouseService.WarehouseProductStockModelsQueryable();
                    total = qDatas.Count();
                    if (kendoFilterModel.filter != null)
                        datas = KendoFilterHelper.FilterQueryableData(qDatas, kendoFilterModel).Skip(kendoFilterModel.skip).Take(kendoFilterModel.take);
                }

                return Json(new { total, data = datas });
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, status = false });
            }
        }

    }
}