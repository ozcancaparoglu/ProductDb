using PMS.Common.Dto;
using ProductDb.Admin.PageModels.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductDb.Admin.Areas.PMS.Services
{
    public interface IApiLogService
    {
        IEnumerable<ApiLog> GetApiLogs(int companyId, KendoFilterModel kendoFilterModel, out int total);
    }
}
