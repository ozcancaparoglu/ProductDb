using Microsoft.AspNetCore.Mvc;
using ProductDb.Admin.PageModels.User;
using ProductDb.Mapping.BiggBrandDbModels;
using ProductDb.Services.PermissionServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductDb.Admin.Components
{
    [ViewComponent(Name = "UserRolePermission")]
    public class UserRolePermissionViewComponent: ViewComponent
    {
        private IPermissionService _permissionService;
        private IUserRolePermissionService _userRolePermissionService;

        public UserRolePermissionViewComponent(IUserRolePermissionService userRolePermissionService,
                                               IPermissionService permissionService)
        {
            _permissionService = permissionService;
            _userRolePermissionService =  userRolePermissionService;
        }
        public IViewComponentResult Invoke(int? id = null)
        {
            UserRolePermissionViewModel ViewModel = new UserRolePermissionViewModel();
            if (id != null)
                ViewModel.RolePermissionModels = _userRolePermissionService.AllUserPermissionsByRoleId(id.Value);
            else
                ViewModel.RolePermissionModels = _userRolePermissionService.AllUserPermissions();

            ViewModel.PermissionModels = _permissionService.AllPermissions();

            ViewModel.RoleId = id.Value;

            return View("_UserRolePermissionView", ViewModel);
        }
    }
}
