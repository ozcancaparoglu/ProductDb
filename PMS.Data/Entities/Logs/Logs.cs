using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.Data.Entities.Logs
{
    public class Logs : BaseEntity
    {
        public int EntityId { get; set; }
        public string EntityKey { get; set; }
        public string Message { get; set; }
        public int CompanyId { get; set; }
        public string Message2 { get; set; }
    }
}
