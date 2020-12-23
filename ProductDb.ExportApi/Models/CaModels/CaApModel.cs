using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductDb.ExportApi.Models.CaModels
{
    public class CaApiModel
    {
        public int StoreId { get; set; }
        public int LanguageId { get; set; } = 2;
        public int CAProductId { get; set; }
        public int ProfileId { get; set; }
        public List<int> ProductIds { get; set; }
    }
}
