using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductDb.ExportApi.DTOs
{
    public class ProductAttributeDTO
    {
        //public int EntityId { get; set; }
        //public string TableName { get; set; }
        public string FieldName { get; set; }
        //public int? LanguageId { get; set; }
        public string Value { get; set; }
        public string Unit { get; set; }
    }
}
