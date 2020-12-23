using ProductDb.Mapping.BiggBrandDbModels;
using System.Collections.Generic;

namespace ProductDb.Admin.PageModels.ParentProduct
{
    public class ParentProductViewModel
    {
        public ParentProductModel ParentProduct { get; set; }
        public ICollection<CategoryModel> Categories { get; set; }
        public ICollection<SupplierModel> Suppliers { get; set; }
        public ICollection<BrandModel> Brands { get; set; }
    }
}
