using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PMS.Data.Entities.Invoice
{
    public class Invoice : BaseEntity
    {
        [StringLength(50)]
        public string OrderNo { get; set; }
        public int Type{ get; set; }

        [StringLength(50)]
        public string InvoiceNo { get; set; }
        [StringLength(50)]
        public string ProjectCode { get; set; }
        [StringLength(50)]
        public string ShipmentCode { get; set; }
        [StringLength(200)]
        public string ShipmentDefination { get; set; }
        [StringLength(50)]
        public string Telephone1 { get; set; }
        [StringLength(50)]
        public string Telephone2 { get; set; }
        [StringLength(200)]
        public string Address1 { get; set; }
        [StringLength(200)]
        public string Address2 { get; set; }
        [StringLength(50)]
        public string Town { get; set; }
        [StringLength(50)]
        public string City { get; set; }
        [StringLength(50)]
        public string TaxOffice { get; set; }
        [StringLength(50)]
        public string TaxNo { get; set; }
        [StringLength(100)]
        public string Explanation1 { get; set; }
        [StringLength(100)]
        public string Explanation2 { get; set; }
        [StringLength(100)]
        public string Explanation3 { get; set; }
        public bool IsTransferred { get; set; } = false;
        public string ErrorMessage { get; set; }
        public int LogoTransferedId { get; set; }
        public int LogoCompanyCode { get; set; }
        public string InvoiceDate { get; set; }
        public int Genexctyp { get; set; }
        public int Lineexctyp { get; set; }
        public bool isEINVOICE { get; set; } = false;
        public ICollection<InvoiceItem> InvoiceItems { get; set; }
    }
}
