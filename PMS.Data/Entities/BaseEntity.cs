using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.Data.Entities
{
    public abstract class BaseEntity
    {
        public long Id { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreateDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public bool isActive { get; set; } = true;
        public bool isDeleted { get; set; }
    }
}
