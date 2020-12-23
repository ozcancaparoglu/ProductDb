using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.Mapping
{
    public class LogModel: BaseModel
    {
        public int EntityId { get; set; }
        public string EntityKey { get; set; }
        public string Message { get; set; }
        public int CompanyId { get; set; }
    }
}
