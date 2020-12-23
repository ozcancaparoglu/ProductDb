using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PMS.Common.Dto
{
    public class Project
    {
        public long Id { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreateDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdateDate { get; set; }

        [MaxLength(20, ErrorMessage = "Max 20 Karakter Olmaldırı.")]
        [Display(Name = "Proje Prefix")]
        public string ProjectPrefix { get; set; }

        [Required(ErrorMessage = "Project Code Girilmelidir.")]
        [MaxLength(50, ErrorMessage = "Max 50 Karakter Olmalıdır.")]
        [Display(Name = "Proje Kodu")]
        public string ProjectCode { get; set; }

        [MaxLength(200, ErrorMessage = "Max 200 Karakter Olmalıçdırı.")]
        [Required(ErrorMessage = "Project Name Girilmelidir.")]
        [Display(Name = "Proje Adı")]
        public string ProjectName { get; set; }
        [MaxLength(50, ErrorMessage = "Max 50 Karakter Olmalıdır.")]

        [Display(Name = "Cari Adı")]
        public string CheckingAccount { get; set; }

        [Display(Name = "Cari Açılacak Mı ?")]
        public bool CheckingAccountToBeCreated { get; set; }
        [MaxLength(50, ErrorMessage = "Max 50 Karakter Olmalıdır.")]

        [Display(Name = "Cari Kodu")]
        public string CheckingAccountCode { get; set; }
        [MaxLength(20, ErrorMessage = "Max 20 Karakter Olmalıdır.")]
        public string LogoFirmCode { get; set; }

        public string GroupCode { get; set; }
        [MaxLength(50, ErrorMessage = "Max 50 Karakter Olmalıdır.")]

        public string Factory { get; set; }
        [MaxLength(50, ErrorMessage = "Max 50 Karakter Olmalıdır.")]

        [Display(Name = "Departman (Logo) ?")]
        public string Departmant { get; set; }

        public string Warehouse { get; set; }

        public bool PointIsOrderDiscount { get; set; }
        [Display(Name = "KDV Dahil Mi ?")]
        public bool TaxIncluded { get; set; }
        public bool isActive { get; set; }
        [MaxLength(50, ErrorMessage = "Max 50 Karakter Olmalıdır.")]
        [Display(Name = "Proje Grup Kodu")]
        public string ProjectGroupCode { get; set; }
        [Display(Name = "Fiyat Kontrolü ?")]

        public bool PriceControl { get; set; }
        [Display(Name = "Fiyat Farkı ?")]
        public bool PriceDifference { get; set; }
        public int DueDateDifference { get; set; }
        [MaxLength(50, ErrorMessage = "Max 50 Karakter Olmalıdır.")]
        [Display(Name = "Bölüm (Logo) ?")]
        public string Division { get; set; }
        public string Source_Cost_GRP { get; set; } = "15";
        [Display(Name = "Proje Tipi ?")]
        public ProjectType ProjectType { get; set; }


        [Display(Name = "Currency (Default) ?")]
        [Required(ErrorMessage = "Project Currency Girilmelidir.")]
        public string DefaultCurrency { get; set; }

        public ICollection<Division> Divisions { get; set; }
        public ICollection<Department> Departments { get; set; }
        // for navigations
        public int? FirmNo { get; set; }
    }
}
