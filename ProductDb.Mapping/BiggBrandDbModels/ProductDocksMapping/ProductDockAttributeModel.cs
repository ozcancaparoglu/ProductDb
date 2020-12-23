using ProductDb.Common.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProductDb.Mapping.BiggBrandDbModels.ProductDocksMapping
{
    public class ProductDockAttributeModel: EntityBaseModel
    {
        public int ProductDockId { get; set; }
        public ProductDockModel ProductDock { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
