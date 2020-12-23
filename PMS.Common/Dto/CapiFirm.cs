using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace PMS.Common.Dto
{
    [Serializable]
    [XmlRoot("RESULTLINE")]
    public class CapiFirm
    {
        public int LOGICALREF { get; set; }
        public int NR { get; set; }
        public string NAME { get; set; }
        public string TITLE { get; set; }
    }
}
