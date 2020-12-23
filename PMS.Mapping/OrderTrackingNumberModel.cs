using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace PMS.Mapping
{
    [Serializable]
    [XmlRoot("RESULTLINE")]
    public class OrderTrackingNumberModel: BaseModel
    {
        public string OrderNo { get; set; }
        public string TrackingNumber { get; set; }
        public string Code { get; set; }
        public string CargoFirm { get; set; }
        public int CompanyId { get; set; }
    }
}
