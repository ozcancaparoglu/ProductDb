using ProductDb.Common.Entities;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductDb.Data.BiggBrandsDb
{
    public class ProductAttributeMapping : EntityBase
    {
        public int? ProductId { get; set; }

        [ForeignKey("ProductId")]
        public Product Product { get; set; }

        public int? AttributesId { get; set; }

        [ForeignKey("AttributesId")]
        public Attributes Attributes { get; set; }

        public int? AttributeValueId { get; set; }

        [ForeignKey("AttributeValueId")]
        public AttributesValue AttributesValue{ get; set; }

        public string RequiredAttributeValue { get; set; }

        public bool IsRequired { get; set; }

    }
}
