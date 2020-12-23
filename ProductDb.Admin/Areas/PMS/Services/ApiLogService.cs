using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PMS.Common;
using PMS.Common.Dto;
using ProductDb.Admin.PageModels.Filter;

namespace ProductDb.Admin.Areas.PMS.Services
{
    public class ApiLogService : IApiLogService
    {
        private readonly IApiRepo apiRepo;

        public ApiLogService(IApiRepo apiRepo)
        {
            this.apiRepo = apiRepo;
        }
        public IEnumerable<ApiLog> GetApiLogs(int companyId, KendoFilterModel kendoFilterModel, out int total)
        {
            var logs = apiRepo.Post($"log/get-logs/{companyId}", kendoFilterModel,
               "", Endpoints.PMS).Result;
            var list2 = JsonConvert.DeserializeObject<ApiLogModel>(logs.JsonContent); ;
            total = list2.total;
            return list2.datas;
        }
    }
}
