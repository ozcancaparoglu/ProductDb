using System;
using System.Collections.Generic;
using System.Text;

namespace ProductDb.Common.GlobalEntity
{
    public class IntegrationOrderDto
    {
        public long Id { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreateDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdateDate { get; set; }

        public string OrderNo { get; set; }

        public string ProjectCode { get; set; }

        public string ShipmentCode { get; set; }

        public string BillingAddressName { get; set; }

        public string BillingAddress { get; set; }

        public string BillingTown { get; set; }

        public string BillingCity { get; set; }

        public string BillingCountry { get; set; }

        public string BillingTelNo1 { get; set; }

        public string BillingTelNo2 { get; set; }

        public string BillingTelNo3 { get; set; }

        public string BillingPostalCode { get; set; }

        public string BillingIdentityNumber { get; set; }

        public string BillingTaxOffice { get; set; }

        public string BillingTaxNumber { get; set; }

        public string BillingEmail { get; set; }

        public string ShippingAddressName { get; set; }

        public string ShippingAddress { get; set; }

        public string ShippingTown { get; set; }

        public string ShippingCity { get; set; }
        public string ShippingCountry { get; set; }
        public string ShippingTelNo1 { get; set; }
        public string ShippingTelNo2 { get; set; }
        public string ShippingTelNo3 { get; set; }
        public string ShippingPostalCode { get; set; }
        public string ShippingEmail { get; set; }
        public string CountryCode { get; set; }
        public decimal CollectionViaCreditCard { get; set; }
        public decimal CollectionViaTransfer { get; set; }
        public decimal ShippingCost { get; set; }
        public string Explanation1 { get; set; }
        public string Explanation2 { get; set; }

        public string Explanation3 { get; set; }

        public DateTime? OrderDate { get; set; }
        public bool IsTransferred { get; set; } = false;
        public bool IsAccountCreated { get; set; } = false;
        public bool IsAccountShippingCreated { get; set; } = false;
        public string ErrorMessage { get; set; }
        public DateTime? LastTryDate { get; set; }
        public int LogoCompanyCode { get; set; }
        public bool isExternal { get; set; }
        public string ThirdPartyTransactionId { get; set; }
        public ICollection<IntegrationOrderItemDto> OrderItems { get; set; }
        // for navigation
        public int Total { get; set; }
    }
}
