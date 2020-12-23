using System;
using System.Collections.Generic;
using System.Text;

namespace ProductDb.Common.GlobalEntity
{
    public class OrderTrackingModel
    {
        public List<OrderTrackingData> datas { get; set;}
        public int total { get; set; }
    }

    public class OrderTrackingData
    {
        public long Id { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreateDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public bool isActive { get; set; } = true;
        public bool isDeleted { get; set; }
        public string OrderNo { get; set; }
        public string TrackingNumber { get; set; }
        public string Code { get; set; }
        public string CargoFirm { get; set; }
        public int CompanyId { get; set; }
    }
}
