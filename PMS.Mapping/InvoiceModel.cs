using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.Mapping
{
    public class InvoiceModel: BaseModel
    {
        public string OrderNo { get; set; }
        public int Type { get; set; }
        public string InvoiceNo { get; set; }
        public string ProjectCode { get; set; }
        public string ShipmentCode { get; set; }
        public string ShipmentDefination { get; set; }
        public string Telephone1{ get; set; }
        public string Telephone2 { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Town { get; set; }
        public string City { get; set; }
        public string TaxOffice { get; set; }
        public string TaxNo { get; set; }
        public string Explanation1 { get; set; }
        public string Explanation2 { get; set; }
        public string Explanation3 { get; set; }
        public bool IsTransferred { get; set; } = false;
        public string ErrorMessage { get; set; }
        public int LogoTransferedId { get; set; }
        public int LogoCompanyCode { get; set; }
        public string InvoiceDate { get; set; }
        public int Genexctyp { get; set; }
        public int Lineexctyp { get; set; }
        public bool isEINVOICE { get; set; }
        public ICollection<InvoiceItemModel> InvoiceItems { get; set; }
    }
}
