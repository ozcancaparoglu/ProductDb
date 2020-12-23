using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace PMS.LogoService.LogoModels
{
    [XmlRoot(ElementName = "ITEMS")]
    public class LogoItemModel
    {
        [XmlElement(ElementName = "ITEM")]
        public Item Item { get; set; }
    }
}
