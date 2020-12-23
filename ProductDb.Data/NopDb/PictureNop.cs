using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductDb.Data.NopDb
{
    [Table("Picture")]
    public class PictureNop
    {
        [Key]
        public int Id { get; set; }

        public string MimeType { get; set; }

        public string SeoFilename { get; set; }

        public string AltAttribute { get; set; }

        public string TitleAttribute { get; set; }

        public bool IsNew { get; set; }

        public string ImageUrl { get; set; }
    }
}
