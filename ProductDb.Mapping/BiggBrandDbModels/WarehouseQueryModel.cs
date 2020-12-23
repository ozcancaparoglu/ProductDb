using ProductDb.Common.Entities;
using System.Collections.Generic;

namespace ProductDb.Mapping.BiggBrandDbModels
{
    public class WarehouseQueryModel: EntityBaseModel
    {
        public int WarehouseTypeId { get; set; }
        public string WarehouseName { get; set; }
        public string Query { get; set; }

        public ICollection<WarehouseTypeModel> WarehouseTypeModels { get; set; }
    }
}
