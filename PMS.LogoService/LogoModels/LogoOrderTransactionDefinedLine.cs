using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace PMS.LogoService.LogoModels
{
    [XmlRoot(ElementName = "DEFNFLD")]
    public class LogoOrderTransactionDefinedLine
    {
        public int MODULENR { get; set; }
        public int LEVEL_ { get; set; }
        public int NUMFLDS1 { get; set; }
        public decimal NUMFLDS2 { get; set; }
        //public string NUMFLDS2 { get; set; }
        public int NUMFLDS3 { get; set; }
        public decimal? NUMFLDS4 { get; set; }
        public int NUMFLDS8 { get; set; }
        public decimal? NUMFLDS10 { get; set; }
        public string TEXTFLDS11 { get; set; }
        public string TEXTFLDS5 { get; set; }
        public string TEXTFLDS6 { get; set; }
        public string TEXTFLDS7 { get; set; }
        public string TEXTFLDS8 { get; set; }
        public string TEXTFLDS12 { get; set; }
    }
}
