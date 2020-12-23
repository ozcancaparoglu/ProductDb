using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.Mapping
{
    public class BaseModel
    {
        public long Id { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreateDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdateDate { get; set; }
       
    }
}
