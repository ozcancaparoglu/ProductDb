using ProductDb.Mapping.BiggBrandDbModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductDb.Admin.PageModels.Store
{
    public class StoreCategoryUpdateViewModel
    {
        public int StoreId { get; set; }
        public List<CategoryModel> Categories { get; set; }
        public List<StoreCategoryMappingModel> StoreCategoryMappings{ get; set; }
    }
}
