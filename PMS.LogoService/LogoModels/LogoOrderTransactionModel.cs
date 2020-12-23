using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace PMS.LogoService.LogoModels
{
    [XmlRoot(ElementName = "TRANSACTION")]
    public class LogoOrderTransactionModel : BaseTransaction
    {
        public string DUE_DATE { get; set; }
        public string PROJECT_CODE { get; set; }
        public int VAT_INCLUDED { get; set; }
        public string CURR_PRICE { get; set; }
        public LogoOrderTransactionDefinedLines DEFNFLDS { get; set; }
    }
}
