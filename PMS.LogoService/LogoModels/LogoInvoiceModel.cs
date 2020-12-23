using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace PMS.LogoService.LogoModels
{
    [XmlRoot(ElementName = "SALES_INVOICES")]
    public class LogoInvoiceModel
    {
        [XmlElement(ElementName = "INVOICE")]
        public Invoice INVOICE { get; set; }
    }
}
