using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PMS.Data.Entities.Order
{
    public class OrderTrackingNumber: BaseEntity
    {
        [StringLength(50)]
        public string OrderNo { get; set; }

        [StringLength(50)]
        public string TrackingNumber { get; set; }

        [StringLength(50)]
        public string Code { get; set; }

        [StringLength(100)]
        public string CargoFirm { get; set; }

        public int CompanyId { get; set; }
    }
}
