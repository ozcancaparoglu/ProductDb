using ProductDb.Common.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductDb.Data.BiggBrandsDb
{
    public class StoreWarehouseMapping : EntityBase
    {
        public int? StoreId { get; set; }
        [ForeignKey("StoreId")]
        public virtual Store Store { get; set; }
        public int? WarehouseTypeId { get; set; }
        [ForeignKey("WarehouseTypeId")]
        public WarehouseType WarehouseType { get; set; }
    }
}
