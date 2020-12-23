using System.Xml.Serialization;

namespace PMS.LogoService.LogoModels
{
    [XmlRoot(ElementName = "ORDER_SLIP")]
    public class OrderSlip
    {
        [XmlAttribute(AttributeName = "DBOP")]
        public string DBOP { get; set; } = "INS";

        public string DOC_NUMBER { get; set; }
        public string AUXIL_CODE { get; set; }
        public string SHIPLOC_CODE { get; set; }
        public string SOURCE_WH { get; set; }
        public string PROJECT_CODE { get; set; }
        public string NUMBER { get; set; } = "~";
        public string DATE { get; set; }
        public long TIME { get; set; }
        public string ARP_CODE { get; set; }
        public string CURRSEL_TOTAL { get; set; }
        public string CURRSEL_DETAILS { get; set; }
        public LogoOrderTransactionsModel TRANSACTIONS { get; set; }
        public string ORDER_STATUS { get; set; }
        public string UPD_CURR { get;  set; }
        public string UPD_TRCURR { get; set; }
        public string DOC_TRACK_NR { get; set; }
        public string FACTORY { get;  set; }
        public string DIVISION { get; set; }
        public string DEPARTMENT { get; set; }
        public string NOTES2 { get; set; }
        public string NOTES1 { get; set; }
        public string NOTES4 { get; set; }
        public string NOTES3 { get; set; }
    }
}