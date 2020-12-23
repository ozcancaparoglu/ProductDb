using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace PMS.LogoService.LogoModels
{
    [XmlRoot(ElementName = "MARKS")]
    public class LogoMarksModel
    {
        [XmlElement(ElementName = "MARK")]
        public MarkModel MarkModel { get; set; }
    }
}
