using ProductDb.Common.Enums;
using ProductDb.Mapping.BiggBrandDbModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProductDb.Services.WarehouseServices
{
    public interface IWarehouseQueryService
    {
        IList<WarehouseQueryModel> WarehouseQueryModels();
        WarehouseQueryModel AddLogoQuery(WarehouseQueryModel logoWarehouseQueryModel);
        WarehouseQueryModel EditLogoQuery(WarehouseQueryModel logoWarehouseQueryModel);
        int DeleteLogoQuery(WarehouseQueryModel logoWarehouseQueryModel);
        WarehouseQueryModel GetQueryById(int id);
        WarehouseQueryModel GetLogoWarehouseQueryModelByWarehouse(int WarehouseTypeId,List<string> SKUs, int? AMBAR_ID = null);
        WarehouseQueryModel GetLogoWarehouseQueryModelByWarehouse(List<string> SKUs, int? AMBAR_ID = null);
        void SetWarehoseTypeState(int objectId);
        WarehouseQueryModel GetWarehouseQueriesByWarehouseType(int id);
    }
}
