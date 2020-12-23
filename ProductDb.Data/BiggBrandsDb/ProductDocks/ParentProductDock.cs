using ProductDb.Common.Entities;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductDb.Data.BiggBrandsDb.ProductDocks
{
    public class ParentProductDock : EntityBase
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public string Sku { get; set; }       

        public int? ProductDockCategoryId { get; set; }
        [ForeignKey("ProductDockCategoryId")]
        public virtual ProductDockCategory Category { get; set; }

        public int? SupplierId { get; set; }
        [ForeignKey("SupplierId")]
        public virtual Supplier Supplier { get; set; }

        public ICollection<ProductDock> Products { get; set; }
    }
}
