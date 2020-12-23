using ProductDb.Common.Entities;

namespace ProductDb.Mapping.BiggBrandDbModels
{
    public class LanguageValuesModel : EntityBaseModel
    {
        public int EntityId { get; set; }
        public string TableName { get; set; }
        public string FieldName { get; set; }
        public int? LanguageId { get; set; }
        public virtual LanguageModel Language{ get; set; }
        public string Value { get; set; }
        public string Unit { get; set; }
    }
}
