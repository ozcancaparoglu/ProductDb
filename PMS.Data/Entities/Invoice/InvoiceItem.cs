using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PMS.Data.Entities.Invoice
{
    public class InvoiceItem: BaseEntity
    {
        public long InvoiceId { get; set; }
        public virtual Invoice Invoice { get; set; }
        [StringLength(50)]
        public string SKU { get; set; }
        public int Quantity { get; set; }
        [Column(TypeName = "decimal(18, 4)")]
        public decimal Price { get; set; }
        [Column(TypeName = "decimal(18, 4)")]
        public decimal VAT { get; set; }
        [StringLength(5)]
        public string Currency { get; set; }
        public string Defination { get; set; }
    }
}
