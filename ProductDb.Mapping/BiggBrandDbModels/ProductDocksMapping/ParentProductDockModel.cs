using ProductDb.Common.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProductDb.Mapping.BiggBrandDbModels.ProductDocksMapping
{
    public class ParentProductDockModel: EntityBaseModel
    {
        public string Title { get; set; }

        public string Sku { get; set; }

        public int? ProductDockCategoryId { get; set; }
        public virtual ProductDockCategoryModel Category { get; set; }

        public int? SupplierId { get; set; }
        public virtual SupplierModel Supplier { get; set; }

        public int? BrandId { get; set; }
        public virtual BrandModel Brand { get; set; }

        public ICollection<ProductDockModel> Products { get; set; }
    }
}
