using ProductDb.Mapping.BiggBrandDbModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ProductDb.Services.ErpComanyService
{
    public interface IErpCompanyService
    {
        IEnumerable<ErpCompanyModel> ErpCompanies();
        Task<string> SaveProductToCompany(int companyID, ProductModel product);
        Task SyncFirms();
        ErpCompanyModel ErpCompanyById(int id);
        IEnumerable<ProductErpCompanyMappingModel> ProductCompaniesById(int id);
    }
}
