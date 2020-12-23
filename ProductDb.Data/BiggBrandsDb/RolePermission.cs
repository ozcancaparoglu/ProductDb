using ProductDb.Common.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProductDb.Data.BiggBrandsDb
{
    public class RolePermission: EntityBase
    {
        public int PermissionId { get; set; }
        public Permission Permission { get; set; }
        public int UserRoleId { get; set; }
        public UserRole UserRole { get; set; }
    }
}
