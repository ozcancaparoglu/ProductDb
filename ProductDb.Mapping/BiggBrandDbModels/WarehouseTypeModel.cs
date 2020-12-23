using ProductDb.Common.Entities;

namespace ProductDb.Mapping.BiggBrandDbModels
{
    public class WarehouseTypeModel: EntityBaseModel
    {
        public string Name { get; set; }
        public bool IsSelected { get; set; }
        public int? LogoWarehouseId { get; set; }
    }
}
