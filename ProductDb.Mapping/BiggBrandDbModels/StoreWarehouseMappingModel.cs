using ProductDb.Common.Entities;

namespace ProductDb.Mapping.BiggBrandDbModels
{
    public class StoreWarehouseMappingModel : EntityBaseModel
    {
        public int? StoreId { get; set; }
        public StoreModel Store { get; set; }
        public int? WarehouseTypeId { get; set; }
        public WarehouseTypeModel WarehouseType { get; set; }
    }
}
