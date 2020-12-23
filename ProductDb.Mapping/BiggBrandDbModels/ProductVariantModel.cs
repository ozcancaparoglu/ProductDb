using ProductDb.Common.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProductDb.Mapping.BiggBrandDbModels
{
    public class ProductVariantModel: EntityBaseModel
    {
        public int ProductId { get; set; }
        public ProductModel Product { get; set; }
        public int ParentProductId { get; set; }
        public ParentProductModel ParentProduct { get; set; }
        public string ProductAttributes { get; set; }
        public int? BaseId { get; set; }
        public ProductModel BaseProduct { get; set; }
        public ProductVariantModel ProductVariants { get; set; }
        // For Navigation
        public string ParentProductSku { get; set; }
        public string BaseProductName { get; set; }
        public string BaseSKU { get; set; }
        public List<int> IDs { get; set; }
    }
}
