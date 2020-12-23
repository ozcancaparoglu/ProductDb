using ProductDb.Mapping.BiggBrandDbModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductDb.Admin.PageModels.User
{
    public class UserRolePermissionViewModel
    {
        public int RoleId { get; set; }
        public PermissionModel PermissionModel { get; set; }
        public ICollection<RolePermissionModel> RolePermissionModels { get; set; }
        public ICollection<PermissionModel> PermissionModels { get; set; }
    }
}
