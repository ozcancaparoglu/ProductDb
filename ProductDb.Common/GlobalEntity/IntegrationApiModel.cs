using System;
using System.Collections.Generic;
using System.Text;

namespace ProductDb.Common.GlobalEntity
{
    public class IntegrationApiModel
    {
        public int StoreId { get; set; }
        public int LanguageId { get; set; }
        public int CAProductId { get; set; }
        public int ProfileId { get; set; }
        public List<int> ProductIds { get; set; }
    }
}
