using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace PMS.LogoService.LogoModels
{
    public class ShipmentLoc
    {
        [XmlAttribute(AttributeName = "DBOP")]
        public string DBOP { get; set; } = "INS";

        public string ARP_CODE { get; set; }
        public string CODE { get; set; }
        public string DESCRIPTION { get; set; }
        public string TITLE { get; set; }
        public string TELEPHONE1 { get; set; }
        public string TELEPHONE2 { get; set; }
        public string EMAIL_ADDR { get; set; }
        public string POSTAL_CODE { get; set; }
        public string ADDRESS1 { get; set; }
        public string ADDRESS2 { get; set; }
        public string TOWN { get; set; }
        public string CITY { get; set; }
        public string COUNTRY_CODE { get; set; }
        public string COUNTRY { get; set; }
    }
}
