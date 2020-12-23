using Microsoft.AspNetCore.Mvc;
using ProductDb.Admin.Helpers.LocalizationHelpers;
using ProductDb.Mapping.BiggBrandDbModels;
using ProductDb.Services.LanguageServices;
using ProductDb.Services.PermissionServices;
using System;

namespace ProductDb.Admin.Controllers
{
    [MiddlewareFilter(typeof(LocalizationPipeline))]
    [Route("{lang}/Permission")]
    public class PermissionController : BaseController
    {
        private readonly IPermissionService _permissionService;
        private readonly IUserRolePermissionService _userRolePermissionService;

        public PermissionController(ILanguageService languageService, IPermissionService permissionService, IUserRolePermissionService userRolePermissionService) : base(languageService, userRolePermissionService)
        {
            _permissionService = permissionService;
            _userRolePermissionService = userRolePermissionService;
        }

        [Route("list")]
        public IActionResult List()
        {  
            return View(_permissionService.AllPermissions());
        }

        [Route("Synchronization")]
        [HttpPost]
        public JsonResult Synchronization()
        {
            try
            {
                _permissionService.synchronization();
                return Json(new { status = true, Message = "" });
            }
            catch (Exception ex)
            {
                return Json(new { status = false, Message = ex.Message.ToString() });
            }
        }

        [Route("create")]
        public IActionResult Create(bool validation = true)
        {
            ViewBag.Validation = validation;
            var model = new PermissionModel();

            return View(model);
        }

        [Route("create")]
        [HttpPost]
        public IActionResult Create(PermissionModel model)
        {
            _permissionService.AddPermission(model);
            return RedirectToAction("list", new { lang = CurrentLanguage });
        }

        [Route("edit/{id}")]
        public IActionResult Edit(int id)
        {
            var model = _permissionService.GetPermissionById(id);
            return View(model);
        }

        [Route("edit/{id}")]
        [HttpPost]
        public IActionResult Edit(PermissionModel model)
        {
            _permissionService.EditPermission(model);
            return RedirectToAction("List", new { lang = CurrentLanguage });
        }

        [Route("AllpermissionToRole/{id}")]
        public IActionResult AllpermissionToRole(int id)
        {
            try
            {
                _userRolePermissionService.AllPermissionToRole(id);
                return Json(new { status = true, message = "OK" });
            }
            catch (Exception ex)
            {
                return Json(new { status = true, message = ex.Message });
            }
        }
    }
}