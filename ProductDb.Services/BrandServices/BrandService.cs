using ProductDb.Common.Enums;
using ProductDb.Data.BiggBrandsDb;
using ProductDb.Mapping.BiggBrandDbModels;
using ProductDb.Mapping.AutoMapperConfigurations;
using ProductDb.Repositories.BaseRepositories;
using ProductDb.Repositories.UnitOfWorks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ProductDb.Services.BrandServices
{
    public class BrandService : IBrandService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IAutoMapperConfiguration autoMapper;

        private readonly IGenericRepository<Brand> brandRepo;

        private readonly Expression<Func<Brand, bool>> defaultBrandFilter = null;

        public BrandService(IUnitOfWork unitOfWork, IAutoMapperConfiguration autoMapper)
        {
            this.unitOfWork = unitOfWork;
            this.autoMapper = autoMapper;

            brandRepo = this.unitOfWork.Repository<Brand>();

            defaultBrandFilter = entity => entity.State == (int)State.Active;
        }

        public ICollection<BrandModel> AllBrands()
        {
            return autoMapper.MapCollection<Brand, BrandModel>(brandRepo.GetAll()).OrderBy(x => x.Name).ToList();
        }
        public ICollection<BrandModel> AllActiveBrands()
        {
            return autoMapper.MapCollection<Brand, BrandModel>(brandRepo.FindAll(defaultBrandFilter)).OrderBy(x => x.Name).ToList();
        }

        public BrandModel BrandById(int id)
        {
            return autoMapper.MapObject<Brand, BrandModel>(brandRepo.GetById(id));
        }

        public BrandModel AddNewBrand(BrandModel model)
        {
            var entity = autoMapper.MapObject<BrandModel, Brand>(model);

            #region Validations

            if (brandRepo.Filter(x => x.Name.ToLowerInvariant() == entity.Name.ToLowerInvariant()).Count() > 0)
                return null;

            #endregion


            var savedEntity = brandRepo.Add(entity);

            return autoMapper.MapObject<Brand, BrandModel>(savedEntity);
        }

        public BrandModel EditBrand(BrandModel model)
        {
            var entity = autoMapper.MapObject<BrandModel, Brand>(model);

            #region Validations

            if (brandRepo.Filter(x => x.Id != entity.Id && x.Name.ToLowerInvariant() == entity.Name.ToLowerInvariant()).Count() > 0)
                return null;

            #endregion

            var savedEntity = brandRepo.Update(entity);

            return autoMapper.MapObject<Brand, BrandModel>(savedEntity);
        }

        public void SetState(int objectId)
        {
            var entity = brandRepo.GetById(objectId);

            if (entity.State == (int)State.Active)
                entity.State = (int)State.Passive;

            else
                entity.State = (int)State.Active;

            brandRepo.Update(entity);
        }

        public BrandModel IsDefined(string name)
        {
            return autoMapper.MapObject<Brand, BrandModel>(brandRepo.Filter(x => x.Name.ToLowerInvariant() == name.ToLowerInvariant()).FirstOrDefault());
        }
    }
}
