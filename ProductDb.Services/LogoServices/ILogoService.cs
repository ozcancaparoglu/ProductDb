using PMS.Common.Dto;
using ProductDb.Common.Enums;
using ProductDb.Data.LogoDb;
using ProductDb.Mapping.BiggBrandDbModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProductDb.Services.LogoServices
{
    public interface ILogoService
    {
        //IList<T> GetStockListByWarehouseId<T>(int WarehouseTypeId, List<string> SKUs) where T : class;
        void UpdateProductStockFromLogo();
        void SyncProductWarehouseStock();
        IEnumerable<LogoProduct> GetOnnetStockQuantityFromLogoByWarehouse(List<string> SKUs);
        Task PushAllProductToRedisAsync();
        Task UpdateProductStockFromRedis();
        Task<LogoResponseDto> AddProductToLogo(ProductModel product, int companyId);
        ItemCard PrepareItemCard(ProductModel product);
        Task<IEnumerable<CapiFirm>> SyncFirms();
    }
}