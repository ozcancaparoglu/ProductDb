using PMS.Common;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PMS.Data.Entities.Project
{
    public class Project : BaseEntity
    {
        [StringLength(50)]
        public string ProjectPrefix { get; set; }

        [Required]
        [StringLength(50)]
        public string ProjectCode { get; set; }

        [StringLength(200)]
        public string ProjectName { get; set; }

        /// <summary>
        /// Cari Hesap
        /// </summary>
        [StringLength(200)]
        public string CheckingAccount { get; set; }

        /// <summary>
        ///  Cari Hesap Açılacak Mı?
        /// </summary>
        public bool? CheckingAccountToBeCreated { get; set; }

        /// <summary>
        /// Cari Hesap Kodu
        /// </summary>
        [StringLength(50)]
        public string CheckingAccountCode { get; set; }

        [StringLength(20)]
        public string LogoFirmCode { get; set; }

        [StringLength(50)]
        public string ProjectGroupCode { get; set; }

        [StringLength(50)]
        public string Factory { get; set; }
        [StringLength(50)]
        public string Departmant { get; set; }
        [StringLength(100)]
        public string Warehouse { get; set; }
  
        /// <summary>
        /// Puan İndirimi Var mı?
        /// </summary>
        public bool PointIsOrderDiscount { get; set; }

        /// <summary>
        ///  Kdv Dahil Mi?
        /// </summary>
        public bool TaxIncluded { get; set; }

        /// <summary>
        /// Fiyat Kontrolü Yapılacak
        /// </summary>
        public bool PriceControl { get; set; }

        /// <summary>
        /// Fiyat Farkı
        /// </summary>
        public bool PriceDifference { get; set; }

        /// <summary>
        /// Tahmini Gönderim Tarihi
        /// </summary>
        /// 
        public int DueDateDifference { get; set; }
        [StringLength(50)]
        public string Division { get; set; }
        public string Source_Cost_GRP { get; set; } = "15";
        public string DefaultCurrency { get; set; }
        public ProjectType ProjectType { get; set; }
        public ICollection<ProjectProduct> ProjectProducts { get; set; }

    }
}