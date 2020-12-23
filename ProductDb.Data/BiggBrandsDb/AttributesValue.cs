using ProductDb.Common.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductDb.Data.BiggBrandsDb
{
    public class AttributesValue : EntityBase
    {
        public int? AttributesId { get; set; }
        [ForeignKey("AttributesId")]
        public Attributes Attributes { get; set; }
        public string Value { get; set; }
        public string Unit { get; set; }
    }
}
