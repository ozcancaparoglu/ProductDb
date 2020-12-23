using ProductDb.Common.Entities;

namespace ProductDb.Data.BiggBrandsDb
{
    public class WarehouseType: EntityBase
    {
        public string Name { get; set; }
        public int? LogoWarehouseId { get; set; }
    }
}
