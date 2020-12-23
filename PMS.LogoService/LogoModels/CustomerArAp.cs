using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace PMS.LogoService.LogoModels
{
    public class CustomerArAp
    {
        [XmlAttribute(AttributeName = "DBOP")]
        public string DBOP { get; set; } = "INS";
        public string ACCOUNT_TYPE { get; set; }
        public string CODE { get; set; }
        public string TITLE { get; set; }
        public string AUTH_CODE { get; set; }
        public string NAME { get; set; }
        public string SURNAME { get; set; }
        public string TELEPHONE1 { get; set; }
        public string ADDRESS1 { get; set; }
        public string ADDRESS2 { get; set; }
        public string TOWN { get; set; }
        public string CITY { get; set; }
        public string COUNTRY_CODE { get; set; }
        public string COUNTRY { get; set; }
        public int PERSCOMPANY { get; set; }
        public string TCKNO { get; set; }
        public string TAX_ID { get; set; }
        public string TAX_OFFICE { get; set; }
        public string E_MAIL { get; set; }
        public int PURCHBRWS { get; set; }
        public int SALESBRWS { get; set; }
        public int IMPBRWS { get; set; }
        public int EXPBRWS { get; set; }
        public int FINBRWS { get; set; }
        public string GL_CODE { get; set; }
        public int ACCEPT_EINV { get; set; }
        //public string PROFILEID_DESP { get; set; }
        //public string POST_LABEL { get; set; }
        //public string SENDER_LABEL { get; set; }
    }
}
