using ProductDb.Mapping.BiggBrandDbModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductDb.Admin.PageModels.User
{
    public class UserViewModel
    {
        public UserModel User { get; set; }

        public ICollection<UserRoleModel> UserRoles{ get; set; }
       
        public string Password { get; set; }
    }
}
