using System.Linq;
using ProductDb.Data.BiggBrandsDb.ProductDocks;
using ProductDb.Mapping.AutoMapperConfigurations;
using ProductDb.Mapping.BiggBrandDbModels.ProductDocksMapping;
using ProductDb.Repositories.BaseRepositories;
using ProductDb.Repositories.UnitOfWorks;

namespace ProductDb.Services.ProductDockServices
{
    public class ProductDockPictureService : IProductDockPictureService
    {
        private readonly IAutoMapperConfiguration autoMapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly IGenericRepository<ProductDockPictures> produckDockPictureRepo;

        public ProductDockPictureService(IUnitOfWork unitOfWork,IAutoMapperConfiguration autoMapper)
        {
            this.autoMapper = autoMapper;
            this.unitOfWork = unitOfWork;
            this.produckDockPictureRepo = this.unitOfWork.Repository<ProductDockPictures>();
        }
        public ProductDockPicturesModel ProductDockPictureByProductDockId(int id)
        {
            return autoMapper.MapObject<ProductDockPictures, 
                ProductDockPicturesModel>(produckDockPictureRepo.Filter(x => x.ProductDockId == id).FirstOrDefault());
        }
    }
}
