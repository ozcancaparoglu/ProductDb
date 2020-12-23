using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace PMS.LogoService.LogoModels
{
    [Serializable]
    [XmlRoot("RESULTLINE")]
    public class DivisionModel
    {
        public int LOGICALREF { get; set; }
        public string FIRMNR { get; set; }
        public string NR { get; set; }
        public string NAME { get; set; }
    }
}
