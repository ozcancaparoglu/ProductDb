using ProductDb.Common.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductDb.Data.BiggBrandsDb
{
    public class CategoryAttributeMapping : EntityBase
    {
        public int? CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public Category Category { get; set; }
        public int? AttributesId { get; set; }
        [ForeignKey("AttributesId")]
        public Attributes Attributes { get; set; }
        public bool IsRequired { get; set; }

    }
}
