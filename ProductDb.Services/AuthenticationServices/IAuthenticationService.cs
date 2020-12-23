using ProductDb.Mapping.BiggBrandDbModels;
using System.Collections.Generic;

namespace ProductDb.Services.AuthenticationServices
{
    public interface IAuthenticationService
    {
        UserModel Authenticate(string emailAccount, string password);
        UserModel CreateUser(string account, string email, string password, int userRoleId);
        ICollection<UserModel> AllActiveUser();
        ICollection<UserModel> AllUser();
        UserModel EditUser(int id, string userName, string email, string password, int userRoleId);
        void SetState(int objectId);
        UserModel GetById(int id);
        int UserCount();
        bool ChangePassword(UserModel model);

        #region UserRole

        ICollection<UserRoleModel> AllActiveUserRole();
        UserRoleModel GetUserRoleById(int userRoleId);
        int CreateUserRoles();

        #endregion
    }
}