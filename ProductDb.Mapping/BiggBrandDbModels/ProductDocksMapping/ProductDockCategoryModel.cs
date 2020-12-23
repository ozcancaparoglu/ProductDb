using ProductDb.Common.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProductDb.Mapping.BiggBrandDbModels.ProductDocksMapping
{
    public class ProductDockCategoryModel: EntityBaseModel
    {
        public string Name { get; set; }
        public int? ParentCategoryId { get; set; }
    }
}
