using ProductDb.Common.Entities;

namespace ProductDb.Mapping.BiggBrandDbModels
{
    public class CategoryAttributeMappingModel : EntityBaseModel
    {
        public int? CategoryId { get; set; }
        public CategoryModel Category { get; set; }

        public int? AttributesId { get; set; }
        public AttributesModel Attributes { get; set; }

        public bool IsRequired { get; set; }

    }
}
