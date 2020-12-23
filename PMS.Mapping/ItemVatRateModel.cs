using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.Mapping
{
    public class ItemVatRateModel: BaseModel
    {
        public string ItemCode { get; set; }
        public int VateRate { get; set; }
        public string Defination { get; set; }
    }
}
