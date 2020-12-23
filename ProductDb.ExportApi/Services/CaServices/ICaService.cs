using ProductDb.Common.HttpClient;
using ProductDb.ExportApi.Common;
using ProductDb.ExportApi.Models;
using ProductDb.ExportApi.Models.CaModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductDb.ExportApi.Services.CaServices
{
    public interface ICaService
    {
        string RefreshTokenCA();
        Task<ResultModel> UpdateAllPrice(CaApiModel caApiModel);
        Task<ResultModel> PushAllProduct(CaApiModel caApiModel);
        Task<ResultModel> UpdateAllQuantity(CaApiModel caApiModel);
        Task<ResultModel> UpdateQuantity(CaApiModel caApiModel);
        Task<ResultModel> UpdatePrice(CaApiModel caApiModel);
    }
}
