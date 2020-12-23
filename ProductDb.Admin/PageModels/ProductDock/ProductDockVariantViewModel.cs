using ProductDb.Mapping.BiggBrandDbModels.ProductDocksMapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductDb.Admin.PageModels.ProductDock
{
    public class ProductDockVariantViewModel
    {
        public int ParentProductDockId { get; set; }
        public ProductDockModel ProductDock { get; set; }
        public List<ProductDockModel> NotVariantedProductDocks { get; set; }
    }
}
