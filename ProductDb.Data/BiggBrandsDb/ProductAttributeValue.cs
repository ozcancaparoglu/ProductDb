using ProductDb.Common.Entities;

namespace ProductDb.Data.BiggBrandsDb
{
    public class ProductAttributeValue : EntityBase
    {
        public int ProductId { get; set; }
        public string Attribute { get; set; }
        public int? AttributeValueId { get; set; }
        public string AttributeValue { get; set; }
        public string Unit { get; set; }
    }
}
