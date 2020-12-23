using ProductDb.Common.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProductDb.Data.BiggBrandsDb
{
    public class ErpCompany: EntityBase
    {
        public int ErpRef { get; set; }
        public int FirmNo { get; set; }
        public string FirmName { get; set; }
        public string Title { get; set; }
    }
}
