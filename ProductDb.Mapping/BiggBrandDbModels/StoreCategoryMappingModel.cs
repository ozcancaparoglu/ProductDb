using ProductDb.Common.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProductDb.Mapping.BiggBrandDbModels
{
    public class StoreCategoryMappingModel: EntityBaseModel
    {
        public int StoreId { get; set; }
        public StoreModel Store { get; set; }
        public int CategoryId { get; set; }
        public CategoryModel Category { get; set; }
        public int ErpCategoryId { get; set; }
    }
}
