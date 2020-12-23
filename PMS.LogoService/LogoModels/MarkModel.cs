using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace PMS.LogoService.LogoModels
{
    [XmlRoot(ElementName = "MARK")]
    public class MarkModel
    {
        [XmlAttribute(AttributeName = "DBOP")]
        public string DBOP { get; set; } = "INS";

        public string CODE { get; set; }
        public string DESCR { get; set; }
        public string SPECODE { get; set; }
        public int DATA_REFERENCE { get; set; } = 1;
    }
}
