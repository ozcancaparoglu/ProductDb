using ProductDb.Mapping.BiggBrandDbModels;
using ProductDb.Mapping.BiggBrandDbModels.ProductDocksMapping;
using System.Collections.Generic;

namespace ProductDb.Admin.PageModels.ProductDock
{
    public class ProductDockViewModel
    {
        public List<ProductDockModel> ProductDocks { get; set; }
        public ICollection<ParentProductModel> ParentProducts { get; set; }
        public ICollection<CategoryModel> Categories { get; set; }
        public ICollection<BrandModel> Brands { get; set; }
        public ICollection<SupplierModel> Suppliers { get; set; }
    }
}
