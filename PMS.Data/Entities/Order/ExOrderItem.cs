using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PMS.Data.Entities.Order
{
    public class ExOrderItem: BaseEntity
    {
        public long OrderId { get; set; }
        public virtual ExOrder Order { get; set; }
        [StringLength(50)]
        public string SKU { get; set; }
        [StringLength(500)]
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        [Column(TypeName = "decimal(18, 4)")]
        public decimal Price { get; set; }
        [Column(TypeName = "decimal(18, 4)")]
        public decimal VAT { get; set; }
        [StringLength(5)]
        public string Currency { get; set; }
        public int Desi { get; set; }
        public int Points { get; set; }
        [Column(TypeName = "decimal(18, 4)")]
        public decimal PointsValue { get; set; }

        public bool EmailSent { get; set; }
        public bool IARSent { get; set; }
        public bool IsInChequeManage { get; set; }
        public decimal? CargoPrice { get; set; }
        public decimal? Consumable { get; set; }
        public string CargoCurrency { get; set; }
        public string ConsumableCurrency { get; set; }
        public string CatalogCode { get; set; }

        //  [Id]
        //,[SipId]
        //,[MasterKod]
        //,[ProductName]
        //,[Quantiy]
        //,[Kdv]
        //,[Price]
        //,[Currency]
        //,[Desi]
        //,[Puan]
        //,[PuanTutar]
        //,[SendEmail]
        //,[IARSend]
        //,[IsInBiggflex]
        //,[IsInChequeManage]
    }
}
