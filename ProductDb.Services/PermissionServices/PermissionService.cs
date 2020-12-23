using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using ProductDb.Data.BiggBrandsDb;
using ProductDb.Mapping.AutoMapperConfigurations;
using ProductDb.Mapping.BiggBrandDbModels;
using ProductDb.Repositories.BaseRepositories;
using ProductDb.Repositories.UnitOfWorks;

namespace ProductDb.Services.PermissionServices
{
    public class PermissionService : IPermissionService
    {
        private IUnitOfWork unitOfWork;
        private IAutoMapperConfiguration autoMapper;
        private IActionDescriptorCollectionProvider _actionDescriptorCollectionProvider;
        private IGenericRepository<Permission> permissionRepo;

        public PermissionService(IUnitOfWork unitOfWork,
                                 IAutoMapperConfiguration autoMapper,
                                 IActionDescriptorCollectionProvider actionDescriptorCollectionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.autoMapper = autoMapper;
            _actionDescriptorCollectionProvider = actionDescriptorCollectionProvider;
            permissionRepo = this.unitOfWork.Repository<Permission>();
        }
        public PermissionModel AddPermission(PermissionModel permissionModel)
        {
            var entity = autoMapper.MapObject<PermissionModel, Permission>(permissionModel);
            return autoMapper.MapObject<Permission, PermissionModel>(permissionRepo.Add(entity));
        }

        public ICollection<PermissionModel> AllPermissions()
        {
            return autoMapper.MapCollection<Permission, PermissionModel>(permissionRepo.GetAll()).ToList();
        }

        public PermissionModel AllPermissionsByPermissionKey(string Key)
        {
            var key = permissionRepo.Filter(a => a.key == Key).SingleOrDefault();
            return autoMapper.MapObject<Permission, PermissionModel>(key);
        }

        public PermissionModel EditPermission(PermissionModel permissionModel)
        {
            var entity = autoMapper.MapObject<PermissionModel, Permission>(permissionModel);
            return autoMapper.MapObject<Permission, PermissionModel>(permissionRepo.Update(entity));
        }

        public PermissionModel GetPermissionById(int id)
        {
            var data = permissionRepo.GetById(id);
            return autoMapper.MapObject<Permission, PermissionModel>(data);
        }

        public PermissionModel GetPermissionByKey(string Key)
        {
            var data = permissionRepo.Filter(a => a.key == Key).SingleOrDefault();
            return autoMapper.MapObject<Permission, PermissionModel>(data);
        }

        public void synchronization()
        {
            var routes = _actionDescriptorCollectionProvider.ActionDescriptors.Items.Select(x => new
            {
                Action = $"{x.RouteValues["Controller"].ToLowerInvariant()}.{x.RouteValues["Action"].ToLowerInvariant()}",
                Name = $"{x.RouteValues["Controller"]}/{x.RouteValues["Action"]}",
            }).GroupBy(a => new { a.Action, a.Name }).ToList();

            var Permissions = AllPermissions();
            foreach (var item in routes)
            {
                var data = Permissions.Where(a => a.key == item.Key.Action).FirstOrDefault();
                if (data == null)
                {
                    AddPermission(new PermissionModel()
                    {
                        key = item.Key.Action,
                        name = item.Key.Name,
                        CreatedDate = DateTime.Now,
                        ProcessedBy = 1,
                        State = 1,
                        UpdatedDate = DateTime.Now
                    });
                }

            }

        }
    }
}
