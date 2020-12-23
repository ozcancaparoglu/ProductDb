using ProductDb.Mapping.BiggBrandDbModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductDb.Admin.PageModels.ProductVariant
{
    public class ChangeProductVariantViewModel
    {
        public int ProductId { get; set; }
        public int BaseId { get; set; }
        public IList<ProductModel> Products { get; set; }
    }
}
