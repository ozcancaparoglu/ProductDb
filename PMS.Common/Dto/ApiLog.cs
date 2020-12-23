using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.Common.Dto
{
    public class ApiLogModel
    {
        public List<ApiLog> datas { get; set; }
        public int total { get; set; }
    }

    public class ApiLog
    {
        public int EntityId { get; set; }
        public string EntityKey { get; set; }
        public string Message { get; set; }
        public int CompanyId { get; set; }
        public DateTime? CreateDate { get; set; }
    }
}
