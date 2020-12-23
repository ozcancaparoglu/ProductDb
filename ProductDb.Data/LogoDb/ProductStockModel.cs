using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ProductDb.Data.LogoDb
{
    public class ProductStockModel
    {
        [Key]
        public long Id { get; set; }
        public Int16 FirmCode { get; set; }
        public string FirmName { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string WarehouseName { get; set; }
        public Int16 WarehouseNumber { get; set; }
        public double? Quantity { get; set; }
    }
}
