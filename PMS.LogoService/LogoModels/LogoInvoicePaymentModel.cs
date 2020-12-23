using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace PMS.LogoService.LogoModels
{
    [XmlRoot(ElementName = "PAYMENT")]
    public class LogoInvoicePaymentModel
    {
        public string DATE { get; set; }
        public string MODULENR { get; set; }
        public string TOTAL { get; set; }
        public string PROCDATE { get; set; }
        public string TRCODE { get; set; }
    }
}
