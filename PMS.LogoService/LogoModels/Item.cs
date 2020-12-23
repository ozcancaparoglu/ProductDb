using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace PMS.LogoService.LogoModels
{
    [XmlRoot(ElementName = "ITEM")]
    public class Item
    {
        [XmlAttribute(AttributeName = "DBOP")]
        public string DBOP { get; set; } = "INS";
        public int CARD_TYPE { get; set; } = 1;
        public string CODE { get; set; }
        public string NAME { get; set; }
        public string GROUP_CODE { get; set; } = string.Empty;
        public int USEF_PURCHASING { get; set; } = 1;
        public int USEF_SALES { get; set; } = 1;
        public int USEF_MM { get; set; } = 1;
        public int VAT { get; set; }
        public int AUTOINCSL { get; set; } = 1;
        public int LOTS_DIVISIBLE { get; set; } = 1;
        public string UNITSET_CODE { get; set; }
        public int CREATED_BY { get; set; } = 1;
        public string DATE_CREATED { get; set; }
        public int DATA_REFERENCE { get; set; }
        public int EXT_ACC_FLAGS { get; set; } = 3;
        public int PACKET { get; set; } = 0;
        public int SELVAT { get; set; }
        public int RETURNVAT { get; set; }
        public int SELPRVAT { get; set; }
        public int RETURNPRVAT { get; set; }
        public string MARKCODE { get; set; }
        public int UPDATECHILDS { get; set; } = 1;
        public ItemUnitsModel UNITS { get; set; }
    }
}
