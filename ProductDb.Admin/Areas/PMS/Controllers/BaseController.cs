using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ProductDb.Services.LanguageServices;
using ProductDb.Services.PermissionServices;

namespace ProductDb.Admin.Areas.PMS.Controllers
{
    public class BaseController : Controller
    {
        private readonly IUserRolePermissionService userRolePermissionService;
        public static int _userId = 0;
        public static string _userRole = "";

        public BaseController(IUserRolePermissionService userRolePermissionService)
        {
            this.userRolePermissionService = userRolePermissionService;
        }
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var ActionName = ((Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor)filterContext.ActionDescriptor).ActionName;
            var ControllerName = ((Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor)filterContext.ActionDescriptor).ControllerName;

            _userId = int.Parse(User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Sid).Value);
            _userRole = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role).Value;

            string key = $"{ControllerName.ToLowerInvariant()}.{ActionName.ToLowerInvariant()}";

            if (_userId != 0)
            {
                var roleId = userRolePermissionService.GetUserRoleIdByUserId(_userId);

                var _permissionControl = userRolePermissionService.UserPermisionControlByKey(roleId, key);
                if (!_permissionControl)
                    filterContext.Result = new RedirectToActionResult("AccessDenied", "Auth", null);
            }

            filterContext.RouteData.Values["Controller"] = ControllerName;
            filterContext.RouteData.Values["Action"] = ActionName;

            base.OnActionExecuting(filterContext);
        }
    }
}