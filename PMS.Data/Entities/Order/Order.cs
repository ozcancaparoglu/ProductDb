using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PMS.Data.Entities.Order
{
    public class Order : BaseEntity
    {
        [StringLength(50)]
        public string OrderNo { get; set; }
        [StringLength(50)]
        public string ProjectCode { get; set; }
        [StringLength(50)]
        public string ShipmentCode { get; set; }

        /// <summary>
        ///  Ft_Name
        /// </summary>
        [StringLength(100)]
        public string BillingAddressName { get; set; }

        [StringLength(200)]
        public string BillingCompany { get; set; }

        [Column(TypeName ="NVARCHAR(MAX)")]
        public string BillingAddress { get; set; }
        [StringLength(100)]
        public string BillingTown { get; set; }
        [StringLength(100)]
        public string BillingCity { get; set; }
        [StringLength(100)]
        public string BillingCountry { get; set; }
        [StringLength(50)]
        public string BillingTelNo1 { get; set; }
        [StringLength(50)]
        public string BillingTelNo2 { get; set; }
        [StringLength(50)]
        public string BillingTelNo3 { get; set; }
        [StringLength(50)]
        public string BillingPostalCode { get; set; }
        /// <summary>
        /// TCKN
        /// </summary>
        /// 
        [StringLength(15)]
        public string BillingIdentityNumber { get; set; }
        [StringLength(50)]
        public string BillingTaxOffice { get; set; }
        [StringLength(15)]
        public string BillingTaxNumber { get; set; }
        [StringLength(150)]
        public string BillingEmail { get; set; }
        [StringLength(100)]
        public string ShippingAddressName { get; set; }
        [Column(TypeName = "NVARCHAR(MAX)")]
        public string ShippingAddress { get; set; }
        [StringLength(100)]
        public string ShippingTown { get; set; }
        [StringLength(100)]
        public string ShippingCity { get; set; }
        [StringLength(100)]
        public string ShippingCountry { get; set; }
        [StringLength(50)]
        public string ShippingTelNo1 { get; set; }
        [StringLength(50)]
        public string ShippingTelNo2 { get; set; }
        [StringLength(50)]
        public string ShippingTelNo3 { get; set; }
        [StringLength(50)]
        public string ShippingPostalCode { get; set; }
        [StringLength(150)]
        public string ShippingEmail { get; set; }

        /// <summary>
        /// Tahsilat Kredi Kartı
        /// </summary>
        /// 
        [Column(TypeName = "decimal(18, 4)")]
        public decimal CollectionViaCreditCard { get; set; }

        /// <summary>
        /// Tahsilat Havale
        /// </summary>
        /// 
        [Column(TypeName = "decimal(18, 4)")]
        public decimal CollectionViaTransfer { get; set; }

        /// <summary>
        /// Kargo Bedeli
        /// </summary>
        /// 
        [Column(TypeName = "decimal(18, 4)")]
        public decimal ShippingCost { get; set; }
        [StringLength(100)]

        public string Explanation1 { get; set; }
        [StringLength(100)]
        public string Explanation2 { get; set; }
        [StringLength(100)]
        public string Explanation3 { get; set; }

        public bool IsTransferred { get; set; } = false;
        public bool IsAccountCreated { get; set; } = false;
        public bool IsAccountShippingCreated { get; set; } = false;
        public string ErrorMessage { get; set; }
        public DateTime? LastTryDate { get; set; }
        public string ThirdPartyTransactionId { get; set; }
        /// <summary>
        ///  Logo Transfered Id
        /// </summary>
        public int LogoTransferedId { get; set; }
        public int LogoCompanyCode { get; set; }
        public DateTime? OrderDate { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; }

    }
}
