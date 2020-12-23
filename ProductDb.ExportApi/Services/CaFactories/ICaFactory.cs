using ProductDb.ExportApi.Models.CaModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductDb.ExportApi.Services.CaFactories
{
    public interface ICaFactory
    {
        Task<IEnumerable<CaProductViewModel>> PrepareProductViewModel(CaApiModel caApiModel);
        Task<List<CaPriceUpdateModel>> PrepareProductPriceModel(CaApiModel caApiModel);
        Task<List<DCQuantityandID>> PrepareProductQuantityModel(CaApiModel caApiModel);

    }
}
