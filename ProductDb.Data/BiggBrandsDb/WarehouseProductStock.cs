using ProductDb.Common.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductDb.Data.BiggBrandsDb
{
    public class WarehouseProductStock: EntityBase
    {
        public string Sku { get; set; }
        public string Name { get; set; }
        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        public Product Product  { get; set; }
        public int WarehouseTypeId { get; set; }
        [ForeignKey("WarehouseTypeId")]
        public WarehouseType  WarehouseType{ get; set; }
        public double Quantity { get; set; }
    }
}
