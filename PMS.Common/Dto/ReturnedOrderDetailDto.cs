using System.ComponentModel.DataAnnotations;

namespace PMS.Common.Dto
{
    public class ReturnedOrderDetailDto
    {
        public ReturnedOrderBillingDetailDto ReturnedOrderBillingDetail { get; set; }
        public ReturnedOrderShippingDetailDto ReturnedOrderShippingDetail { get; set; }
    }

    public class ReturnedOrderBillingDetailDto : ReturnedOrderShippingDetailDto
    {
        [Display(Name = "TCKN")]
        public string CitizenshipNumber { get; set; }

        [Display(Name = "Firma")]
        public string Company { get; set; }

        [Display(Name = "Vergi Dairesi")]
        public string TaxOffice { get; set; }

        [Display(Name = "Vergi No")]
        public string TaxNumber { get; set; }
    }

    public class ReturnedOrderShippingDetailDto
    {
        [Display(Name = "Ad")]
        public string Name { get; set; }

        [Display(Name = "Ülke")]
        public string Country { get; set; }

        [Display(Name = "İl")]
        public string City { get; set; }

        [Display(Name = "İlçe")]
        public string District { get; set; }

        [Display(Name = "Posta Kodu")]
        public string PostalCode { get; set; }

        [Display(Name = "Adres")]
        public string Address { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Telefon 1")]
        public string Phone1 { get; set; }

        [Display(Name = "Telefon 2")]
        public string Phone2 { get; set; }

        [Display(Name = "Telefon 3")]
        public string Phone3 { get; set; }
    }
}