using ProductDb.Common.Entities;

namespace ProductDb.Mapping.BiggBrandDbModels
{
    public class UserModel : EntityBaseModel
    {
        public string Username { get; set; }

        public string Email { get; set; }

        public byte[] Password { get; set; }

        public string PasswordStr { get; set; }

        public int? UserRoleId { get; set; }
        public virtual UserRoleModel UserRole { get; set; }
    }
}
