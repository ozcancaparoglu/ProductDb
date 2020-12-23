using ProductDb.Mapping.BiggBrandDbModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductDb.Admin.PageModels.Category
{
    public class ProductDockCategoryViewModel
    {
        public int SupplierId { get; set; }
        public int ProductDockCategoryId { get; set; }
        public int CategoryId { get; set; }
        public List<CategoryModel> Categories { get; set; }
    }
}
