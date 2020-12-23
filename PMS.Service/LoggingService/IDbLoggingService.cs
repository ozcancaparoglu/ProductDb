
using PMS.Data.Entities.Logs;
using PMS.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PMS.Service.LoggingService
{
    public interface IDbLoggingService
    {
        void InsertLog(LogModel log);
        IQueryable<Logs> GetQueryableLogs(int companyId);
        int Total(int companyId);
    }
}
