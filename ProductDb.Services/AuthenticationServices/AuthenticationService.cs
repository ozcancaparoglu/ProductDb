using ProductDb.Common.Enums;
using ProductDb.Common.Extensions;
using ProductDb.Data.BiggBrandsDb;
using ProductDb.Mapping.BiggBrandDbModels;
using ProductDb.Mapping.AutoMapperConfigurations;
using ProductDb.Repositories.BaseRepositories;
using ProductDb.Repositories.UnitOfWorks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ProductDb.Services.AuthenticationServices
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IAutoMapperConfiguration autoMapper;

        private readonly IGenericRepository<User> userRepo;
        private readonly IGenericRepository<UserRole> userRoleRepo;

        private readonly Expression<Func<User, bool>> defaultUserFilter = null;
        private readonly Expression<Func<UserRole, bool>> defaultUserRoleFilter = null;

        public AuthenticationService(IUnitOfWork unitOfWork, IAutoMapperConfiguration autoMapper)
        {
            this.unitOfWork = unitOfWork;
            this.autoMapper = autoMapper;

            userRepo = this.unitOfWork.Repository<User>();
            userRoleRepo = this.unitOfWork.Repository<UserRole>();

            defaultUserFilter = entity => entity.State == (int)State.Active;
            defaultUserRoleFilter = entity => entity.State == (int)State.Active;
        }

        public UserModel Authenticate(string emailAccount, string password)
        {
            var user = userRepo.Find(x => x.Email == emailAccount && x.State == (int)State.Active);

            if (user != null && user.Password.Length > 0)
            {
                byte[] hashBytes = user.Password;

                var hash = new PasswordHash(hashBytes);

                if (!hash.Verify(password))
                    return null;

                user = userRepo.Filter(x => x.Id == user.Id && x.State == (int)State.Active, null, "UserRole").FirstOrDefault();

                return autoMapper.MapObject<User, UserModel>(user);
            }

            return null;
        }

        public UserModel CreateUser(string account, string email, string password, int userRoleId)
        {
            if (userRepo.Exist(x => x.Email.ToLowerInvariant() == email.ToLowerInvariant().Trim()))
                return null;

            var pass = new PasswordHash(password);

            var newUser = new User
            {
                Username = account,
                Email = email,
                Password = pass.ToArray(),
                UserRoleId = userRoleId
            };

            return autoMapper.MapObject<User, UserModel>(userRepo.Add(newUser));
        }

        public ICollection<UserModel> AllActiveUser()
        {
            return autoMapper.MapCollection<User, UserModel>(userRepo.FindAll(defaultUserFilter)).ToList();
        }

        public ICollection<UserModel> AllUser()
        {
            return autoMapper.MapCollection<User, UserModel>(userRepo.Filter(null, null, "UserRole")).ToList();
        }

        public UserModel EditUser(int id, string userName, string email, string password, int userRoleId)
        {
            #region Validations
            //Böyle bir email var ise
            if (userRepo.Exist(x => x.Id != id && x.Email.ToLowerInvariant() == email.ToLowerInvariant().Trim()))
                return null;
            //Böyle bir userName var ise
            if (userRepo.Exist(x => x.Id != id && x.Username.ToLowerInvariant() == userName.ToLowerInvariant().Trim()))
                return null;

            #endregion

            var user = userRepo.FindBy(x => x.Id == id).FirstOrDefault();

            if (!string.IsNullOrWhiteSpace(password))
            {
                var pass = new PasswordHash(password);
                user.Password = pass.ToArray();
            }

            user.Username = userName.Trim();
            user.Email = email.Trim();
            user.UserRoleId = userRoleId;

            return autoMapper.MapObject<User, UserModel>(userRepo.Update(user));
        }

        public void SetState(int objectId)
        {
            var entity = userRepo.GetById(objectId);

            if (entity.State == (int)State.Active)
                entity.State = (int)State.Passive;

            else
                entity.State = (int)State.Active;

            userRepo.Update(entity);
        }

        public UserModel GetById(int id)
        {
            return autoMapper.MapObject<User, UserModel>(userRepo.GetById(id));
        }

        public int UserCount()
        {
            return userRepo.Count();
        }

        public bool ChangePassword(UserModel model)
        {
            try
            {
                var entity = userRepo.GetById(model.Id);

                var newPass = new PasswordHash(model.PasswordStr);

                entity.Password = newPass.ToArray();

                userRepo.Update(entity);

                return true;
            }
            catch
            {
                return false;
            }
        }

        #region UserRoles
        public ICollection<UserRoleModel> AllActiveUserRole()
        {
            return autoMapper.MapCollection<UserRole, UserRoleModel>(userRoleRepo.FindAll(defaultUserRoleFilter)).ToList();
        }

        public UserRoleModel GetUserRoleById(int userRoleId)
        {
            return autoMapper.MapObject<UserRole, UserRoleModel>(userRoleRepo.GetById(userRoleId));
        }

        public int CreateUserRoles()
        {
            var userRoles = new List<UserRole>
            {
                new UserRole { Name = UserRolesEnum.SuperAdmin, State = (int)State.Active },
                new UserRole { Name = UserRolesEnum.ContentAdmin, State = (int)State.Active },
                new UserRole { Name = UserRolesEnum.PriceAdmin, State = (int)State.Active },
                new UserRole { Name = UserRolesEnum.ContentUser, State = (int)State.Active }
            };

            return userRoleRepo.AddRange(userRoles);
        }

    

        #endregion

    }
}
