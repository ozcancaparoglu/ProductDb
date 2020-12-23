using ProductDb.Common.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductDb.Data.BiggBrandsDb
{
    public class LanguageValues : EntityBase
    {
        public int EntityId { get; set; }
        public string TableName { get; set; }
        public string FieldName { get; set; }
        public int? LanguageId { get; set; }
        [ForeignKey("LanguageId")]
        public virtual Language Language{ get; set; }
        public string Value { get; set; }
    }
}
