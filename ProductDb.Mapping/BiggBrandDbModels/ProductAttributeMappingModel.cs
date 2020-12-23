using ProductDb.Common.Entities;
using System.Collections;
using System.Collections.Generic;

namespace ProductDb.Mapping.BiggBrandDbModels
{
    public class ProductAttributeMappingModel : EntityBaseModel
    {
        public int? ProductId { get; set; }

        public int? AttributesId { get; set; }
        public AttributesModel Attributes { get; set; }

        public int? AttributeValueId { get; set; }

        public string RequiredAttributeValue { get; set; }

        public bool IsRequired { get; set; }

        public ICollection<LanguageValuesModel> LanguageValues { get; set; }
    }
}
