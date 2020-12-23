using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.LogoService.LogoModels
{
    public abstract class BaseTransaction
    {
        public int TYPE { get; set; }
        public string MASTER_CODE { get; set; }
        public int QUANTITY { get; set; }
        public string PRICE { get; set; }
        public int VAT_RATE { get; set; }
        public string UNIT_CODE { get; set; }
        public string AUXIL_CODE { get; set; }
        public string PC_PRICE { get; set; }
        public string SOURCE_WH { get; set; }
        public string PR_RATE { get; set; }
        public string CURR_TRANSACTIN { get; set; }
        public string RC_XRATE { get; set; }
        public string EDT_PRICE { get; set; }
        public string DISCOUNT_RATE { get; set; }
        public string EDT_CURR { get; set; }
    }
}
