using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace PMS.LogoService.LogoModels
{
    [XmlRoot(ElementName = "TRANSACTIONS")]
    public class LogoOrderTransactionsModel
    {
        [XmlElement(ElementName = "TRANSACTION")]
        public List<LogoOrderTransactionModel> TRANSACTION { get; set; }
    }
}
