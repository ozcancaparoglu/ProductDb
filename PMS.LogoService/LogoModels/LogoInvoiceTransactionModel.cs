using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace PMS.LogoService.LogoModels
{
    [XmlRoot(ElementName = "TRANSACTION")]
    public class LogoInvoiceTransactionModel: BaseTransaction
    {
        public int SOURCECOSTGRP { get; set; }
        public string MASTER_DEF { get; set; }
        public int SOURCEINDEX { get; set; }
        public decimal VAT_AMOUNT { get; set; }
        public string CURR_PRICE { get; set; }
        public string TRCURR { get; set; }
        public string VATEXCEPT_CODE { get; set; }

    }
}
