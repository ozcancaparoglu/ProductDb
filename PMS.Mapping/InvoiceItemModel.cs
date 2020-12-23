using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.Mapping
{
    public class InvoiceItemModel : BaseModel
    {
        public long InvoiceId { get; set; }
        public virtual InvoiceModel Invoice { get; set; }
        public string SKU { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal VAT { get; set; }
        public string Currency { get; set; }
        public string Defination { get; set; }
    }
}
