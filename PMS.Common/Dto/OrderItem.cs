using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.Common.Dto
{
    public class OrderItem
    {
        public bool check { get; set; }
        public long Id { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreateDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public long OrderId { get; set; }
        public virtual Order Order { get; set; }
        public string SKU { get; set; }

        public string ProductName { get; set; }
        public int Quantity { get; set; }

        public decimal Price { get; set; }

        public decimal VAT { get; set; }

        public string Currency { get; set; }
        public int Desi { get; set; }
        public int Points { get; set; }

        public decimal PointsValue { get; set; }
        public string CatalogCode { get; set; }

        public bool EmailSent { get; set; }
        public bool IARSent { get; set; }
        public bool IsInChequeManage { get; set; }
        public decimal? CargoPrice { get; set; }
        public decimal? Consumable { get; set; }
        public string CargoCurrency { get; set; }
        public string ConsumableCurrency { get; set; }
    }
}
