using ProductDb.Common.Cache;
using ProductDb.Common.Enums;
using ProductDb.Data.BiggBrandsDb;
using ProductDb.Mapping.AutoMapperConfigurations;
using ProductDb.Mapping.BiggBrandDbModels;
using ProductDb.Repositories.BaseRepositories;
using ProductDb.Repositories.UnitOfWorks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProductDb.Services.PermissionServices
{
    public class UserRolePermissionService : IUserRolePermissionService
    {
        private IUnitOfWork unitOfWork;
        private IAutoMapperConfiguration autoMapper;
        private readonly ICacheManager cacheManager;

        private IGenericRepository<RolePermission> permissionRepo;
        private readonly IGenericRepository<User> userRepo;
        private readonly IPermissionService permissionService;

        public UserRolePermissionService(IUnitOfWork unitOfWork, IAutoMapperConfiguration autoMapper, ICacheManager cacheManager,IPermissionService permissionService)
        {
            this.unitOfWork = unitOfWork;
            this.autoMapper = autoMapper;
            this.cacheManager = cacheManager;
            permissionRepo = this.unitOfWork.Repository<RolePermission>();
            userRepo = this.unitOfWork.Repository<User>();
            this.permissionService = permissionService;
        }
        public RolePermissionModel AddUserPermission(RolePermissionModel userPermissionModel)
        {
            var entity = autoMapper.MapObject<RolePermissionModel, RolePermission>(userPermissionModel);
            return autoMapper.MapObject<RolePermission, RolePermissionModel>(permissionRepo.Add(entity));
        }

        public ICollection<RolePermissionModel> AllUserPermissions()
        {
            return autoMapper.MapCollection<RolePermission, RolePermissionModel>(permissionRepo.GetAll()).ToList();
        }

        public ICollection<RolePermissionModel> AllUserPermissionsByRoleId(int objectId)
        {
            var roles = new List<RolePermission>();

            //if (!cacheManager.TryGetValue(CacheStatics.RolesCache, out roles))
            //{
            //    roles = permissionRepo.Filter(a => a.UserRoleId == objectId, null, "Permission,UserRole").ToList();

            //    cacheManager.Set(CacheStatics.RolesCache, roles, CacheStatics.RoleCacheTime);
            //}

            roles = permissionRepo.Filter(a => a.UserRoleId == objectId, null, "Permission,UserRole").ToList();

            cacheManager.Set(CacheStatics.RolesCache, roles, CacheStatics.RoleCacheTime);

            return autoMapper.MapCollection<RolePermission, RolePermissionModel>(roles).ToList();
        }

        public RolePermissionModel EditUserPermission(RolePermissionModel userPermissionModel)
        {
            var entity = autoMapper.MapObject<RolePermissionModel, RolePermission>(userPermissionModel);

            return autoMapper.MapObject<RolePermission, RolePermissionModel>(permissionRepo.Update(entity));
        }

        public RolePermissionModel isUserRoleExistsPermission(int RoleId, int PermissionId)
        {
            var permissions = permissionRepo.Filter(a => a.UserRoleId == RoleId && a.PermissionId == PermissionId).FirstOrDefault();

            return autoMapper.MapObject<RolePermission, RolePermissionModel>(permissions);
        }

        public void SetState(int objectId)
        {
            var entity = permissionRepo.GetById(objectId);

            if (entity.State == (int)State.Active)
                entity.State = (int)State.Passive;

            else
                entity.State = (int)State.Active;

            permissionRepo.Update(entity);
        }

        public bool UserPermisionControlByKey(int objectId, string key)
        {
            var Permissions = AllUserPermissionsByRoleId(objectId);

            var data = Permissions.Where(a => a.Permission.key == key && a.State == (int)State.Active).
                        FirstOrDefault() == null ? false : true;

            return data;
        }

        public void AllPermissionToRole(int id)
        {
            var roles = AllUserPermissionsByRoleId(id);
            var now = DateTime.Now;

            var permissions = permissionService.AllPermissions().ToList();
            var rCount = permissions.Count;
            for (int i = 0; i < rCount; i++)
            {
                var prole = roles.Where(x => x.PermissionId == permissions[i].Id).FirstOrDefault();

                if (prole == null)
                {
                    permissionRepo.Add(new RolePermission()
                    {
                        PermissionId = permissions[i].Id,
                        UserRoleId = id,
                        CreatedDate = now,
                        State = (int)State.Active
                    });
                }
            }
        }
        public int GetUserRoleIdByUserId(int userId)
        {
            return userRepo.GetById(userId).UserRoleId.Value;
        }
    }
}
