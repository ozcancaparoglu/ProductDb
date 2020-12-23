using ProductDb.Mapping.BiggBrandDbModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductDb.Admin.PageModels.Store
{
    public class StoreMarginViewModel
    {
        public StoreModel StoreModel { get; set; }
        public MarginTypeModel MarginTypeModel { get; set; }
        // models
        public BrandModel BrandModel { get; set; }
        public CategoryModel CategoryModel { get; set; }
        public List<MarginTypeModel> MarginTypes { get; set; }
        public List<BrandModel> Brands { get; set; }
        public List<CategoryModel> Categories { get; set; }
    }
}
