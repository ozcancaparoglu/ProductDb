using ProductDb.Common.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProductDb.Mapping.BiggBrandDbModels
{
    public class RolePermissionModel: EntityBaseModel
    {
        public int PermissionId { get; set; }
        public PermissionModel Permission { get; set; }
        public int UserRoleId { get; set; }
        public UserRoleModel UserRole { get; set; }
        public string Key { get; set; }
    }
}
