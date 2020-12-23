using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace PMS.LogoService.LogoModels
{
    [XmlRoot(ElementName = "TRANSACTIONS")]
    public class LogoInvoiceTransactionsModel
    {
        [XmlElement(ElementName = "TRANSACTION")]
        public List<LogoInvoiceTransactionModel> TRANSACTION { get; set; }
    }
}
