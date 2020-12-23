using ProductDb.Common.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProductDb.Mapping.BiggBrandDbModels
{
    public class ProductErpCompanyMappingModel: EntityBaseModel
    {
        public int ProductId { get; set; }
        public ProductModel Product { get; set; }

        public int ErpCompanyId { get; set; }
        public ErpCompanyModel ErpCompany { get; set; }

        public string Message { get; set; }
        public bool Status { get; set; }
    }
}
