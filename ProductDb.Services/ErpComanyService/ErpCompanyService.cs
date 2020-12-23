using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PMS.Common.Dto;
using ProductDb.Common.Enums;
using ProductDb.Data.BiggBrandsDb;
using ProductDb.Mapping.AutoMapperConfigurations;
using ProductDb.Mapping.BiggBrandDbModels;
using ProductDb.Repositories.BaseRepositories;
using ProductDb.Repositories.UnitOfWorks;
using ProductDb.Services.LogoServices;

namespace ProductDb.Services.ErpComanyService
{
    public class ErpCompanyService : IErpCompanyService
    {
        private readonly ILogoService logoService;
        private readonly IUnitOfWork unitOfWork;
        private readonly IGenericRepository<ErpCompany> erpCompanyRepo;
        private readonly IGenericRepository<Product> productRepo;
        private readonly IGenericRepository<ProductErpCompanyMapping> productErpCompanyMappingRepo;
        private readonly IAutoMapperConfiguration autoMapper;

        public ErpCompanyService(IUnitOfWork unitOfWork, IAutoMapperConfiguration autoMapper, ILogoService logoService)
        {
            this.logoService = logoService;
            this.unitOfWork = unitOfWork;
            this.erpCompanyRepo = this.unitOfWork.Repository<ErpCompany>();
            this.productRepo = this.unitOfWork.Repository<Product>();
            this.productErpCompanyMappingRepo = this.unitOfWork.Repository<ProductErpCompanyMapping>();
            this.autoMapper = autoMapper;
        }
        public IEnumerable<ErpCompanyModel> ErpCompanies()
        {
            return autoMapper.MapCollection<ErpCompany, ErpCompanyModel>(erpCompanyRepo.Table().Where(x => x.State == (int)State.Active).OrderBy(x => x.Title));
        }

        public ErpCompanyModel ErpCompanyById(int id)
        {
            return autoMapper.MapObject<ErpCompany, ErpCompanyModel>(erpCompanyRepo.Table().FirstOrDefault(x => x.Id == id));
        }

        public IEnumerable<ProductErpCompanyMappingModel> ProductCompaniesById(int id)
        {
            return autoMapper.MapCollection<ProductErpCompanyMapping, ProductErpCompanyMappingModel>(
                productErpCompanyMappingRepo.Table().Include(x => x.ErpCompany).Where(x => x.ProductId == id)).ToList();
        }

        public async Task<string> SaveProductToCompany(int companyId, ProductModel product)
        {
            ProductErpCompanyMapping productErpCompanyMapping = null;

            var prod = autoMapper.MapObject<Product, ProductModel>(productRepo.Table().Include(x => x.VatRate).Include(x => x.Brand).FirstOrDefault());
            if (prod.BrandId == null)
            {
                product = autoMapper.MapObject<Product, ProductModel>(productRepo.Table().Include(x => x.VatRate).Include(x => x.ParentProduct)
                    .ThenInclude(x => x.BrandPP).FirstOrDefault(x => x.Id == product.Id));
            }
            else
                product = prod;

            string message = string.Empty;
            var now = DateTime.Now;

            var erpCompany = ErpCompanyById(companyId);

            var data = productErpCompanyMappingRepo.Table().FirstOrDefault(x => x.ProductId == product.Id && x.ErpCompanyId == erpCompany.Id && x.Status == false);
            if (data == null)
            {
                productErpCompanyMapping = productErpCompanyMappingRepo.Add(new ProductErpCompanyMapping
                {
                    ErpCompanyId = erpCompany.Id,
                    ProductId = product.Id,
                    CreatedDate = now
                });
            }
            else
            {
                productErpCompanyMapping = data;
            }
            // api control for logo response
            var result = await logoService.AddProductToLogo(product, erpCompany.FirmNo);

            if (!result.Status)
            {
                LogoErrorEnumDto errorMessage = isContainError(result.Message);
                // api error message error code contains
                switch (errorMessage)
                {
                    case LogoErrorEnumDto.NoError:
                        productErpCompanyMapping.Status = true;
                        break;
                    case LogoErrorEnumDto.Duplicate:
                        productErpCompanyMapping.Status = true;
                        break;
                    default:
                        productErpCompanyMapping.Status = false;
                        break;
                }
            }
            else
                productErpCompanyMapping.Status = true;

            message = result.Message;
            productErpCompanyMapping.Message = result.Message;
            productErpCompanyMappingRepo.Update(productErpCompanyMapping);

            return message;

        }

        public async Task SyncFirms()
        {
            var firms = await logoService.SyncFirms();
            var erpComanies = ErpCompanies();
            var now = DateTime.Now;

            foreach (var capiFirm in firms)
            {
                var data = erpComanies.FirstOrDefault(x => x.FirmNo == capiFirm.NR);
                if (data == null)
                {
                    erpCompanyRepo.Add(new ErpCompany
                    {
                        ErpRef = capiFirm.LOGICALREF,
                        FirmName = capiFirm.NAME,
                        FirmNo = capiFirm.NR,
                        Title = capiFirm.TITLE,
                        CreatedDate = now
                    });
                }
                else
                {
                    data.FirmName = capiFirm.NAME;
                    data.Title = capiFirm.TITLE;

                    erpCompanyRepo.Update(autoMapper.MapObject<ErpCompanyModel, ErpCompany>(data));
                }
            }
        }

        private LogoErrorEnumDto isContainError(string message)
        {
            LogoErrorEnumDto returnedEnum = LogoErrorEnumDto.NoError;

            var errorMessages = Enum.GetValues(typeof(LogoErrorEnumDto)).Cast<int>().ToArray();
            var errorCount = errorMessages.Length;
            for (int i = 0; i < errorCount; i++)
            {
                if (message.Contains(errorMessages[i].ToString()))
                {
                    returnedEnum = (LogoErrorEnumDto)errorMessages[i];
                    break;
                }
            }
            return returnedEnum;
        }
    }
}
