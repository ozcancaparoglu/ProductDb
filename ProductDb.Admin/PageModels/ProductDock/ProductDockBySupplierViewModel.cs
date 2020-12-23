using ProductDb.Mapping.BiggBrandDbModels;
using ProductDb.Mapping.BiggBrandDbModels.ProductDocksMapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductDb.Admin.PageModels.ProductDock
{
    public class ProductDockBySupplierViewModel
    {
        public int SupplierId { get; set; }
        public int? ProductDockCategoryId { get; set; }
        public IEnumerable<SupplierModel> Suppliers { get; set; }
        public IEnumerable<ProductDockCategoryModel> ProductDockCategories { get; set; }
        public IEnumerable<ProductDockModel> ProductDocks { get; set; }
        public IEnumerable<CategoryModel> Categories { get; set; }
    }
}
