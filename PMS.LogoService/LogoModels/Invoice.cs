using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace PMS.LogoService.LogoModels
{
    [XmlRoot(ElementName = "INVOICE")]
    public class Invoice
    {
        [XmlAttribute(AttributeName = "DBOP")]
        public string DBOP { get; set; } = "INS";

        public int TYPE { get; set; }
        public string NUMBER { get; set; }
        public string DATE { get; set; }
        public string DOC_NUMBER { get; set; }
        public string ARP_CODE { get; set; }
        public string SOURCE_WH { get; set; }
        public string NOTES1 { get; set; }
        public string NOTES2 { get; set; }
        public int VAT_RATE { get; set; }
        public string SOURCE_COST_GRP { get; set; }
        public long TIME { get; set; }
        public string DIVISION { get; set; }
        public string DEPARTMENT { get; set; }
        public string DOC_DATE { get; set; }
        public int CURR_INVOICE { get; set; }
        public int CURRSEL_TOTALS { get; set; }
        public int CURRSEL_DETAILS { get; set; }
        public string EINVOICE { get; set; }
        public string VATEXCEPT_CODE { get; set; }
        public int EARCHIVEDETR_INTPAYMENTTYPE { get; set; }
        public int EINVOICE_TYPE { get; set; }
        public int EARCHIVEDETR_SENDMOD { get; set; }
        //public string GL_CODE { get; set; }

        public LogoInvoiceTransactionsModel TRANSACTIONS { get; set; }
        public LogoInvoicePaymentsModel PAYMENT_LIST { get; set; }
    }
}
