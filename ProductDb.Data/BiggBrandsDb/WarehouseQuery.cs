using ProductDb.Common.Entities;

namespace ProductDb.Data.BiggBrandsDb
{
    public class WarehouseQuery: EntityBase
    {
        public int WarehouseTypeId { get; set; }
        public string WarehouseName { get; set; }
        public string Query { get; set; }
    }
}
