using ProductDb.Common.Entities;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductDb.Data.BiggBrandsDb
{
    public class ParentProduct : EntityBase
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public string Sku { get; set; }
        
        public string Description { get; set; }

        public string ShortDescription { get; set; }

        public int? CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }

        public int? SupplierId { get; set; }
        [ForeignKey("SupplierId")]
        public virtual Supplier SupplierPP { get; set; }

        public int? BrandId { get; set; }
        [ForeignKey("BrandId")]
        public virtual Brand BrandPP { get; set; }

        public ICollection<Product> Products { get; set; }

    }
}
