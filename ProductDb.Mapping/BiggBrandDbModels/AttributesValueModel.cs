using ProductDb.Common.Entities;

namespace ProductDb.Mapping.BiggBrandDbModels
{
    public class AttributesValueModel : EntityBaseModel
    {
        public int AttributesId { get; set; }
        public string Value { get; set; }
        public string Unit { get; set; }
    }
}
