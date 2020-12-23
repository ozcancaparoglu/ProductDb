using ProductDb.Common.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ProductDb.Data.BiggBrandsDb.ProductDocks
{
    public class ProductDockAttribute : EntityBase
    {
        public int ProductDockId { get; set; }
        [ForeignKey("ProductDockId")]
        public ProductDock ProductDock { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
