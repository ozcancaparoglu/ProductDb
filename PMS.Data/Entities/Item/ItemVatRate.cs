using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PMS.Data.Entities.Item
{
    public class ItemVatRate: BaseEntity
    {
        [StringLength(50)]
        public string ItemCode { get; set; }
        public int VateRate { get; set; }
        [StringLength(200)]
        public string Defination { get; set; }
    }
}
