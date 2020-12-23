using PMS.Data.Entities.Logs;
using PMS.Data.Repository;
using PMS.Mapping;
using PMS.Mapping.AutoMapperConfiguration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PMS.Service.LoggingService
{
    public class DbLoggingService : IDbLoggingService
    {
        private readonly IAutoMapperService autoMapperService;
        private readonly IRepository<Logs> logRepo;

        public DbLoggingService(IAutoMapperService autoMapperService, IRepository<Logs> logRepo)
        {
            this.autoMapperService = autoMapperService;
            this.logRepo = logRepo;
        }

        public IQueryable<Logs> GetQueryableLogs(int companyId)
        {
            return logRepo.Table().Where(x => x.CompanyId == companyId).OrderByDescending(x => x.CreateDate);
        }

        public void InsertLog(LogModel log)
        {
            logRepo.Add(autoMapperService.Map<LogModel, Logs>(log));
        }

        public int Total(int companyId)
        {
            return logRepo.Table().Where(x => x.CompanyId == companyId).Count();
        }
    }
}
