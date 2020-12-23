using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace PMS.LogoService.LogoModels
{
    [XmlRoot(ElementName = "SALES_ORDERS")]
    public class LogoOrderModel
    {
        [XmlElement(ElementName = "ORDER_SLIP")]
        public OrderSlip ORDER_SLIP { get; set; }
    }
}
