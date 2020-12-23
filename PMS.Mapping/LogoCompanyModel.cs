using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.Mapping
{
    public class LogoCompanyModel: BaseModel
    {
        public int CompanyId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Setting1 { get; set; }
    }
}
