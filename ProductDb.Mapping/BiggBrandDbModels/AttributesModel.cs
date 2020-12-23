using ProductDb.Common.Entities;
using System.Collections.Generic;

namespace ProductDb.Mapping.BiggBrandDbModels
{
    public class AttributesModel : EntityBaseModel
    {
        public string Name { get; set; }
        public bool IsRequired { get; set; }
        public bool IsVariantable { get; set; } = false;
        public ICollection<AttributesValueModel> AttributesValues { get; set; }
    }
}