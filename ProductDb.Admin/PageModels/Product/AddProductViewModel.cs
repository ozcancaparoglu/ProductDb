using ProductDb.Mapping.BiggBrandDbModels;
using System.Collections.Generic;

namespace ProductDb.Admin.PageModels.Product
{
    public class AddProductViewModel
    {
        public int StoreId { get; set; }
        public int ProductId { get; set; }
        public List<ProductModel> Products { get; set; }
    }
}
