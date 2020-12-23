using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProductDb.Data.OnnetDb;
using ProductDb.Repositories.BaseRepositories;
using ProductDb.Repositories.UnitOfWorks;

namespace ProductDb.Services.OnnetServices
{
    public class OnnetService : IOnnetService
    {
        
        private readonly IGenericRepositoryOnnet<OnnetProduct> onnetRepository;
        public OnnetService(IUnitOfWorkOnnet unitOfWorkOnnet)
        {
            this.onnetRepository = unitOfWorkOnnet.Repository<OnnetProduct>();
        }
        public IList<OnnetProduct> GetOnnetProducts(string ProjectCode, List<string> SKUs)
        {
            var query = GetOnnetProductSqlText(ProjectCode,SKUs);
            var list = onnetRepository.ExecuteRawQuery(query).ToList();
            return list;
        }

        public string GetOnnetProductSqlText(string ProjectCode, List<string> SKUs)
        {
            var formattedSkus = string.Format("'{0}'", string.Join("','", SKUs.Select(i => i.Replace("'", "''"))));

            var query = string.Format(OnnetStatics.sqlText, ProjectCode, formattedSkus);
            return query;
        }
    }
}
