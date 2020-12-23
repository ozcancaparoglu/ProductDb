using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductDb.Data.NopDb
{
    [Table("LocalizedProperty")]
    public class LocalizedPropertyNop
    {
        [Key]
        public int Id { get; set; }

        public int EntityId { get; set; }

        public int LanguageId { get; set; }

        public string LocaleKeyGroup { get; set; }

        public string LocaleKey { get; set; }

        public string LocaleValue { get; set; }
    }
}
