using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ProductDb.Mapping.BiggBrandDbModels;
using ProductDb.Services.LanguageServices;
using ProductDb.Services.PermissionServices;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Localization;

namespace ProductDb.Admin.Controllers
{
    public class BaseController : Controller
    {
        private readonly IUserRolePermissionService userRolePermissionService;
        private string _currentLanguage;
        public readonly ILanguageService languageService;

        public static int _userId = 0;
        public static string _userRole = "";
        public static List<LanguageModel> languages;

        public BaseController(ILanguageService languageService, IUserRolePermissionService userRolePermissionService)
        {
            this.userRolePermissionService = userRolePermissionService;
            this.languageService = languageService;
        }
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            languages = languageService.AllLanguages().ToList();

            var ActionName = ((Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor)filterContext.ActionDescriptor).ActionName;
            var ControllerName = ((Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor)filterContext.ActionDescriptor).ControllerName;

            _userId = int.Parse(User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Sid).Value);
            _userRole = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role).Value;

            string key = $"{ControllerName.ToLowerInvariant()}.{ActionName.ToLowerInvariant()}";

            //if (_userId != 0)
            //{
            //    var roleId = userRolePermissionService.GetUserRoleIdByUserId(_userId);

            //    var _permissionControl = userRolePermissionService.UserPermisionControlByKey(roleId, key);
            //    if (!_permissionControl)
            //        filterContext.Result = new RedirectToActionResult("AccessDenied", "Auth", new { lang = CurrentLanguage });
            //}

            filterContext.RouteData.Values["Controller"] = ControllerName;
            filterContext.RouteData.Values["Action"] = ActionName;
            filterContext.RouteData.Values["lang"] = CurrentLanguage;

            base.OnActionExecuting(filterContext);
        }

        public ActionResult RedirectToDefaultLanguage()
        {
            return RedirectToAction("Index", new { lang = CurrentLanguage });
        }

        public string CurrentLanguage
        {
            get
            {
                if (string.IsNullOrEmpty(_currentLanguage))
                {
                    var feature = HttpContext.Features.Get<IRequestCultureFeature>();
                    _currentLanguage = feature.RequestCulture.Culture.TwoLetterISOLanguageName.ToLower();
                }

                return _currentLanguage;
            }
        }

    }
}