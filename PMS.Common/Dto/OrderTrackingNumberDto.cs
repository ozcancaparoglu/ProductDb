using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.Common.Dto
{
    public class OrderTrackingNumberDtoModel
    {
        public List<OrderTrackingNumberDto> datas { get; set; }
        public int total { get; set; }
    }
    public class OrderTrackingNumberDto
    {
        public long Id { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreateDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string OrderNo { get; set; }
        public string TrackingNumber { get; set; }
        public string Code { get; set; }
        public string CargoFirm { get; set; }
        public int CompanyId { get; set; }
        // for navigation
        public int Total { get; set; }
    }
}
