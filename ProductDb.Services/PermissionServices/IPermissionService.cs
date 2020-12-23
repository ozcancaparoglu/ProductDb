using ProductDb.Mapping.BiggBrandDbModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProductDb.Services.PermissionServices
{
    public interface IPermissionService
    {
        PermissionModel AddPermission(PermissionModel permissionModel);
        PermissionModel EditPermission(PermissionModel permissionModel);
        ICollection<PermissionModel> AllPermissions();
        PermissionModel AllPermissionsByPermissionKey(string Key);
        PermissionModel GetPermissionById(int id);
        PermissionModel GetPermissionByKey(string Key);
        void synchronization();
    }
}
