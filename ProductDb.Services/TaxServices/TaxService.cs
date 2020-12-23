using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProductDb.Data.BiggBrandsDb;
using ProductDb.Mapping.AutoMapperConfigurations;
using ProductDb.Mapping.BiggBrandDbModels;
using ProductDb.Repositories.BaseRepositories;
using ProductDb.Repositories.UnitOfWorks;

namespace ProductDb.Services.TaxServices
{
    public class TaxService : ITaxService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IAutoMapperConfiguration autoMapper;
        private readonly IGenericRepository<VatRate> taxRepo;

        public TaxService(IUnitOfWork unitOfWork, IAutoMapperConfiguration autoMapper)
        {
            this.unitOfWork = unitOfWork;
            this.autoMapper = autoMapper;
            taxRepo = unitOfWork.Repository<VatRate>();
        }
        public ICollection<VatRateModel> AllTaxRate()
        {
            return autoMapper.MapCollection<VatRate, VatRateModel>(taxRepo.GetAll()).ToList();
        }
    }
}
