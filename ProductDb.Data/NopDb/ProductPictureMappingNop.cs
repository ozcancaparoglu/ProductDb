using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductDb.Data.NopDb
{
    [Table("Product_Picture_Mapping")]
    public class ProductPictureMappingNop
    {
        [Key]
        public int Id { get; set; }

        public int ProductId { get; set; }

        public int PictureId { get; set; }

        public int DisplayOrder { get; set; }

        public bool? IsUpdated { get; set; }
    }
}