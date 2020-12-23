using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace PMS.LogoService.LogoModels
{
    [XmlRoot(ElementName = "UNIT")]
    public class ItemUnitModel
    {
        public string UNIT_CODE { get; set; }
        public string BARCODE { get; set; }
        public int USEF_MTRLCLASS { get; set; } = 1;
        public int USEF_PURCHCLAS { get; set; } = 1;
        public int USEF_SALESCLAS { get; set; } = 1;
        public int CONV_FACT1 { get; set; } = 1;
        public int CONV_FACT2 { get; set; } = 1;
    }
}
