using ProductDb.Mapping.BiggBrandDbModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProductDb.Services.PermissionServices
{
    public interface IUserRolePermissionService
    {
        RolePermissionModel AddUserPermission(RolePermissionModel userPermissionModel);
        RolePermissionModel EditUserPermission(RolePermissionModel userPermissionModel);

        ICollection<RolePermissionModel> AllUserPermissions();
        void SetState(int objectId);
        ICollection<RolePermissionModel> AllUserPermissionsByRoleId(int objectId);
        bool UserPermisionControlByKey(int objectId, string key);
        RolePermissionModel isUserRoleExistsPermission(int RoleId, int PermissionId);
        void AllPermissionToRole(int id);

        int GetUserRoleIdByUserId(int userId);
    }
}
