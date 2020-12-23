using ProductDb.Data.BiggBrandsDb.ProductDocks;
using ProductDb.Mapping.AutoMapperConfigurations;
using ProductDb.Mapping.BiggBrandDbModels.ProductDocksMapping;
using ProductDb.Repositories.BaseRepositories;
using ProductDb.Repositories.UnitOfWorks;
using System.Collections.Generic;
using System.Linq;

namespace ProductDb.Services.ProductDockServices
{
    public class ParentProductDockService : IParentProductDockService 
    {
        private readonly IAutoMapperConfiguration autoMapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly IGenericRepository<ParentProductDock> parentProductRepo;

        public ParentProductDockService(IUnitOfWork unitOfWork,IAutoMapperConfiguration autoMapper)
        {
            this.autoMapper = autoMapper;
            this.unitOfWork = unitOfWork;
            this.parentProductRepo = this.unitOfWork.Repository<ParentProductDock>();
        }
        public ICollection<ParentProductDockModel> AllParentProductDocks()
        {
            return autoMapper.MapCollection<ParentProductDock, ParentProductDockModel>(parentProductRepo.GetAll()).ToList();
        }

        public ParentProductDockModel AddNewParentProductDock(ParentProductDockModel model)
        {
            var entity = autoMapper.MapObject<ParentProductDockModel, ParentProductDock>(model);

            var savedEntity = parentProductRepo.Add(entity);

            return autoMapper.MapObject<ParentProductDock, ParentProductDockModel>(savedEntity);
        }

        public ParentProductDockModel ParentProductDockById(int id)
        {
            var parentProductDTO = parentProductRepo.Filter(x => x.Id == id).FirstOrDefault();
            return autoMapper.MapObject<ParentProductDock, ParentProductDockModel>(parentProductDTO);
        }

        public ParentProductDockModel ParentProductDockBySku(string sku)
        {
            var parentProductDTO = parentProductRepo.Filter(x => x.Sku == sku).FirstOrDefault();
            return autoMapper.MapObject<ParentProductDock, ParentProductDockModel>(parentProductDTO);
        }
    }
}
