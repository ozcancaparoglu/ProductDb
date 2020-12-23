using ProductDb.Mapping.BiggBrandDbModels;
using System.Collections.Generic;
using System.Linq;

namespace ProductDb.Services.WarehouseServices
{
    public interface IWarehouseService
    {
        ICollection<WarehouseTypeModel> AllWarehouseTypes();
        WarehouseTypeModel AddWarehouseType(WarehouseTypeModel model);
        WarehouseTypeModel EditWarehouseType(WarehouseTypeModel model);
        ICollection<WarehouseProductStockModel> WarehouseProductStockModels();
        ICollection<WarehouseProductStockModel> WarehouseProductStockModels(int productId);
        ICollection<WarehouseProductStockModel> WarehouseProductStockModelByWarehouseId(int WarehouseTypeId);
        ICollection<WarehouseTypeModel> WarehouseTypes();
        WarehouseTypeModel WarehouseTypeById(int id);
        IQueryable<WarehouseProductStockModel> WarehouseProductStockModelsQueryable();
        IEnumerable<WarehouseProductStockModel> WarehouseProductStockModelsQueryableFiltered(int skip, int take, out int total);
        bool IsProductStockDefined(int ProductId);
        void AddNewProductStock(WarehouseProductStockModel warehouseProductStockModel);
        void EditProductStock(WarehouseProductStockModel warehouseProductStockModel);
        void SetWarehoseTypeState(int objectId);
    }
}
