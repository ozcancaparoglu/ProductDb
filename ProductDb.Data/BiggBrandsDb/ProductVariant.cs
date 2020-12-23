using ProductDb.Common.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ProductDb.Data.BiggBrandsDb
{
    public class ProductVariant : EntityBase
    {
        [ForeignKey("ProductId")]
        public int ProductId { get; set; }
        public Product Product { get; set; }

        [ForeignKey("ParentProductId")] 
        public int ParentProductId { get; set; }
        public ParentProduct ParentProduct { get; set; }

        [ForeignKey("BaseId")]
        public int? BaseId { get; set; }
        public string ProductAttributes { get; set; }
    }
}
