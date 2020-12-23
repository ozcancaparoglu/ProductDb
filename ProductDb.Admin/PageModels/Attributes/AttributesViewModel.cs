using ProductDb.Mapping.BiggBrandDbModels;
using System.Collections.Generic;

namespace ProductDb.Admin.PageModels.Attributes
{
    public class AttributesViewModel
    {
        public AttributesModel Attributes { get; set; }
        public ICollection<AttributesModel> AddAttributes { get; set; }
        public int EntityId { get; set; }
        public string AttributeType { get; set; }
    }
}
