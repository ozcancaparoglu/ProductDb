using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace PMS.LogoService.LogoModels.XMLModels
{
    [Serializable]
    [XmlRoot("RESULTLINE")]
    public class DateDiffInvoice
    {
        [XmlElement(Order = 1)]
        public string PROJECTCODE { get; set; }
        [XmlElement(Order = 2)]
        public string ORDERNO { get; set; }
        [XmlElement(Order = 3)]
        public string INVOICENO { get; set; }
        [XmlElement(Order = 4)]
        public string SHIPPINGADRESSCODE { get; set; }
        [XmlElement(Order = 5)]
        public string SHIPPINGNAME { get; set; }
        [XmlElement(Order = 6)]
        public string ADRESS1 { get; set; }
        [XmlElement(Order = 7)]
        public string ADRESS2 { get; set; }
        [XmlElement(Order = 8)]
        public string TOWN { get; set; }
        [XmlElement(Order = 9)]
        public string CITY { get; set; }
        [XmlElement(Order = 10)]
        public string DISTRICT { get; set; }
        [XmlElement(Order = 11)]
        public string TELNRS1 { get; set; }
        [XmlElement(Order = 12)]
        public string TELNRS2 { get; set; }
        [XmlElement(Order = 13)]
        public string TAXOFFICE { get; set; }
        [XmlElement(Order = 14)]
        public string TAXNR { get; set; }

        [XmlElement(Order = 15)]
        public string PRICE { get; set; }
        [XmlElement(Order = 16)]
        public string VATRATE { get; set; }
        [XmlElement(Order = 17)]
        public string DELIVERYCODE { get; set; }
        [XmlElement(Order = 18)]
        public string ITEMNAME { get; set; }
        [XmlElement(Order = 19)]
        public string ITEMCODE { get; set; }
        [XmlElement(Order = 20)]
        public string EMAILADDR { get; set; }
        [XmlElement(Order = 21)]
        public string COUNTRY { get; set; }
        [XmlElement(Order = 22)]
        public string COUNTRYCODE { get; set; }
        [XmlElement(Order = 23)]
        public string INVOICEDATE { get; set; }
        [XmlElement(Order = 24)]
        public string GENEXCTYP { get; set; }
        [XmlElement(Order = 25)]
        public string LINEEXCTYP { get; set; }
        [XmlElement(Order = 26)]
        public string LINELOGICALREF { get; set; }

    }
}
