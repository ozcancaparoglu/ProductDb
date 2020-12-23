using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace PMS.LogoService.LogoModels
{
    [XmlRoot(ElementName = "PAYMENT_LIST")]
    public class LogoInvoicePaymentsModel
    {
        [XmlElement(ElementName = "PAYMENT")]
        public List<LogoInvoicePaymentModel> PAYMENT { get; set; }
    }
}
