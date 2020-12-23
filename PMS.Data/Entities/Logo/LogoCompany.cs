using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.Data.Entities.Logo
{
    public class LogoCompany: BaseEntity
    {
        public int CompanyId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Setting1 { get; set; }
    }
}
