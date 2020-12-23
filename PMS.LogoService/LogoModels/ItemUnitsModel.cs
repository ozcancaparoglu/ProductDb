using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace PMS.LogoService.LogoModels
{
    [XmlRoot(ElementName = "UNITS")]
    public class ItemUnitsModel
    {
        [XmlElement(ElementName = "UNIT")]
        public List<ItemUnitModel> ItemUnitModel { get; set; }
    }
}
