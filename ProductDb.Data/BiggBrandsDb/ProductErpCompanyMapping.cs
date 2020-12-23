using ProductDb.Common.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ProductDb.Data.BiggBrandsDb
{
    public class ProductErpCompanyMapping: EntityBase
    {
        public int ProductId { get; set; }

        [ForeignKey("ProductId")]
        public Product Product { get; set; }

        public int ErpCompanyId { get; set; }

        [ForeignKey("ErpCompanyId ")]
        public ErpCompany ErpCompany { get; set; }

        public string Message { get; set; }
        public bool Status { get; set; }
    }
}
