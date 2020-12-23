using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace PMS.LogoService.LogoModels
{
    [XmlRoot(ElementName = "ARP_SHIPMENT_LOCATIONS")]
    public class LogoCustomerShippingModel
    {
        [XmlElement(ElementName = "SHIPMENT_LOC")]
        public ShipmentLoc ShipmentLoc { get; set; }
    }
}
