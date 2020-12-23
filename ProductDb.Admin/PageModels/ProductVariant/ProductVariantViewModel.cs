using ProductDb.Mapping.BiggBrandDbModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductDb.Admin.PageModels.ProductVariant
{
    public class ProductVariantViewModel
    {
        public ProductVariantViewModel()
        {
            ParentProducts = new List<ParentProductModel>();
        }
        public List<ParentProductModel> ParentProducts { get; set; }
        public List<AttributesModel> Attributes { get; set; }
        public int ParentProductId { get; set; }
        public int AttributeId { get; set; }
    }
}
