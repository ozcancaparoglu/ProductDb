using ProductDb.Common.Entities;

namespace ProductDb.Mapping.BiggBrandDbModels
{
    public class WarehouseProductStockModel: EntityBaseModel
    {
        public string Sku { get; set; }
        public string Name { get; set; }
        public int ProductId { get; set; }
        public ProductModel Product { get; set; }
        public int WarehouseTypeId { get; set; }
        public WarehouseTypeModel  WarehouseType{ get; set; }
        public double Quantity { get; set; }
        public string ProductName { get; set; }
    }
}
