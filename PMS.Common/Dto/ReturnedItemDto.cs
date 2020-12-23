using System.ComponentModel.DataAnnotations;

namespace PMS.Common.Dto
{
    public class ReturnedItemDto
    {
        public int Id { get; set; }

        [Display(Name = "Firma")]
        public string CompanyName { get; set; }

        [Display(Name = "Proje")]
        public string ProjectCode { get; set; }

        [Display(Name = "MÖK")]
        public string MOK { get; set; }

        [Display(Name = "Sipariş No")]
        public string OrderNumber { get; set; }

        [Display(Name = "Tarih")]
        public string ReturnDateStr { get; set; }

        [Display(Name = "Logo Kodu")]
        public string ProductCode { get; set; }

        [Display(Name = "Ürün")]
        public string ProductDesc { get; set; }

        [Display(Name = "Adet")]
        public int ReturnAmount { get; set; }

        [Display(Name = "Neden")]
        public string ReturnReason { get; set; }

        public string LogoReferans { get; set; }
        public string DeliveryCode { get; set; }

        public Operation Operation { get; set; }
        public string Comment { get; set; }
    }

    public enum Operation
    {
        Cancel = 0,
        Reshipment = 1
    }
}