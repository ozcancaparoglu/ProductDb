using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PMS.API.Models
{
    public class KendoFilterDto
    {
        public int skip { get; set; }
        public int take { get; set; }
        public int page { get; set; }
        public int pageSize { get; set; }
        public KendoFilter filter { get; set; }
    }

    public class KendoFilter
    {
        public List<KendoFilterItem> filters { get; set; }
    }
    public class KendoFilterItem
    {
        public string field { get; set; }
        public string @operator { get; set; }
        public string value { get; set; }
    }
}
