using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace PMS.LogoService.LogoModels
{
    [XmlRoot(ElementName = "AR_APS")]
    public class LogoCustomerModel
    {
        [XmlElement(ElementName = "AR_AP")]
        public CustomerArAp AR_AP { get; set; }
      
    }
}
