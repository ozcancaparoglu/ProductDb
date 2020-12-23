using System;
using System.Collections.Generic;
using System.Text;

namespace ProductDb.Common.GlobalEntity
{
    public class IntegrationOrderItemDto
    {
        public long Id { get; set; }
        public int CreatedBy { get; set; } = 1;
        public DateTime CreateDate { get; set; } = DateTime.Now;
        public int? UpdatedBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public long OrderId { get; set; }
        public virtual IntegrationOrderItemDto Order { get; set; }
        public string SKU { get; set; }

        public string ProductName { get; set; }
        public int Quantity { get; set; }

        public decimal Price { get; set; }

        public decimal VAT { get; set; }//kdv oranı

        public string Currency { get; set; }
        public int Desi { get; set; }//shipping weight
        public int Points { get; set; } = 0;

        public decimal PointsValue { get; set; } = 0;
        public string CatalogCode { get; set; } = "";

        public bool EmailSent { get; set; } = false;
        public bool IARSent { get; set; } = false;
        public bool IsInChequeManage { get; set; } = false;
        public decimal? CargoPrice { get; set; }
        public decimal? Consumable { get; set; } = 0;
        public string CargoCurrency { get; set; } = "";
        public string ConsumableCurrency { get; set; } = "";
    }
}
