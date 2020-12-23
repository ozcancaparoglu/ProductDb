using ProductDb.Common.Entities;
using System.Collections.Generic;

namespace ProductDb.Mapping.BiggBrandDbModels
{
    public class ParentProductModel : EntityBaseModel
    {

        public string Title { get; set; }

        public string Sku { get; set; }
        
        public string Description { get; set; }

        public string ShortDescription { get; set; }

        public int? CategoryId { get; set; }
        public virtual CategoryModel Category { get; set; }

        public int? SupplierId { get; set; }
        public virtual SupplierModel SupplierPP { get; set; }

        public int? BrandId { get; set; }
        public virtual BrandModel BrandPP { get; set; }

        public ICollection<ProductModel> Products { get; set; }

        public string SkuList { get; set; }

    }
}
