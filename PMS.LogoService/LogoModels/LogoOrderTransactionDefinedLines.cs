using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace PMS.LogoService.LogoModels
{

    [XmlRoot(ElementName = "DEFNFLDS")]
    public class LogoOrderTransactionDefinedLines
    {
        [XmlElement(ElementName = "DEFNFLD")]
        public List<LogoOrderTransactionDefinedLine> DEFNFLD { get; set; }
    }
}
