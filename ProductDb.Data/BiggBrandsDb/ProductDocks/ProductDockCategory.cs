using ProductDb.Common.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ProductDb.Data.BiggBrandsDb.ProductDocks
{
    public class ProductDockCategory: EntityBase
    {
        [Required]
        [StringLength(250)]
        public string Name { get; set; }
        public int? ParentCategoryId { get; set; }
        public int? SupplierId { get; set; }
        [ForeignKey("SupplierId")]
        public Supplier Supplier { get; set; }
    }
}
