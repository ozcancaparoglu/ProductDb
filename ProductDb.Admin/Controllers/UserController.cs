using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductDb.Admin.Helpers.LocalizationHelpers;
using ProductDb.Admin.PageModels.RolePermission;
using ProductDb.Admin.PageModels.User;
using ProductDb.Mapping.BiggBrandDbModels;
using ProductDb.Services.AuthenticationServices;
using ProductDb.Services.LanguageServices;
using ProductDb.Services.PermissionServices;
using System;
using System.Linq;

namespace ProductDb.Admin.Controllers
{
    [MiddlewareFilter(typeof(LocalizationPipeline))]
    [Authorize]
    [Route("{lang}/user")]
    public class UserController : BaseController
    {
        private readonly IUserRolePermissionService userRolePermissionService;
        private readonly IAuthenticationService authenticationService;

        public UserController(ILanguageService languageService,
                              IAuthenticationService authenticationService,
                              IUserRolePermissionService userRolePermissionService) : base(languageService, userRolePermissionService)
        {
            this.userRolePermissionService = userRolePermissionService;
            this.authenticationService = authenticationService;
        }

        [Route("list")]
        public IActionResult List()
        {
            var model = authenticationService.AllUser();

            return View(model);
        }

        [Route("create")]
        public IActionResult Create(bool validation = true)
        {
            ViewBag.Validation = validation;
            var model = new UserViewModel
            {
                User = new UserModel(),
                UserRoles = authenticationService.AllActiveUserRole()
            };

            return View(model);
        }

        [Route("create")]
        [HttpPost]
        public IActionResult Create(UserViewModel model)
        {
            authenticationService.CreateUser(model.User.Username, model.User.Email, model.Password, (int)model.User.UserRoleId);

            return RedirectToAction("list", new { lang = CurrentLanguage });
        }

        [Route("edit/{id}")]
        public IActionResult Edit(int id, bool validation = true)
        {
            ViewBag.Validation = validation;
            var model = authenticationService.GetById(id);

            UserViewModel userVM = new UserViewModel
            {
                User = model,
                UserRoles = authenticationService.AllActiveUserRole()
            };
            return View(userVM);
        }

        [Authorize]
        [Route("edit/{id}")]
        [HttpPost]
        public IActionResult Edit(UserViewModel model)
        {
            authenticationService.EditUser(model.User.Id, model.User.Username, model.User.Email, model.Password, (int)model.User.UserRoleId);

            return RedirectToAction("list", new { lang = CurrentLanguage });
        }

        [Authorize]
        [Route("setstate/{id}")]
        public IActionResult State(int id)
        {
            authenticationService.SetState(id);

            return RedirectToAction("list", new { lang = CurrentLanguage });
        }

        [HttpGet]
        [Route("GetUserRoles/{id}")]
        public IActionResult GetUserRoles(int id)
        {
            ViewBag.Id = id;
            return PartialView();
        }

        [HttpPost]
        [Route("AddPermissionRole")]
        public JsonResult AddPermissionRole(RolePermissionModel RoleModel)
        {
            try
            {

                RoleModel.ProcessedBy = _userId;

                var data = userRolePermissionService.isUserRoleExistsPermission(RoleModel.UserRoleId, RoleModel.PermissionId);
                if (data != null)
                {
                    RoleModel.UpdatedDate = DateTime.Now;
                    RoleModel.Id = data.Id;
                    RoleModel.State = 1;
                    userRolePermissionService.EditUserPermission(RoleModel);
                }
                else
                {
                    RoleModel.State = 1;
                    RoleModel.CreatedDate = DateTime.Now;
                    RoleModel.UpdatedDate = DateTime.Now;
                    userRolePermissionService.AddUserPermission(RoleModel);
                }

                return Json(new { result = "Sucess", ErrorMessage = "", status = true });
            }
            catch (Exception ex)
            {
                return Json(new { result = "Failed", ErrorMessage = ex.Message.ToString(), status = false });
            }
        }

        [Route("StateRolePermission/{id}")]
        [HttpGet]
        public IActionResult StateRolePermission(int id)
        {
            try
            {
                userRolePermissionService.SetState(id);
                return Json(new { result = "Sucess", ErrorMessage = "", status = true });
            }
            catch (Exception ex)
            {
                return Json(new { result = "Failed", ErrorMessage = ex.Message.ToString(), status = false });
            }
        }

        [HttpPost]
        [Route("change-password")]
        public JsonResult Password(UserModel model)
        {
            if (authenticationService.ChangePassword(model))
                return Json(new { result = "Success", ErrorMessage = "", status = true });
            else
                return Json(new { result = "Failed", ErrorMessage = "Password is not changed", status = false });
        }

        // new 
        [Route("RolePermissions")]
        public IActionResult RolePermissions()
        {
            var viewModel = new RolePermissionViewModel
            {
                UserRoleModels = authenticationService.AllActiveUserRole().ToList()
            };
            return View(viewModel);
        }
    }
}