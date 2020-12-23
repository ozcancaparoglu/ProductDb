using ProductDb.Common.Entities;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProductDb.Data.BiggBrandsDb
{
    public class Attributes : EntityBase
    {
        [Required]
        [StringLength(250)]
        public string Name { get; set; }
        public bool IsRequired { get; set; }
        public bool IsVariantable { get; set; } = false;
        public ICollection<AttributesValue> AttributesValues { get; set; }
    }
}