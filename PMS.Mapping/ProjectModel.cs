using PMS.Common;

namespace PMS.Mapping
{
    public class ProjectModel : BaseModel
    {
        public string ProjectPrefix { get; set; }

        public string ProjectCode { get; set; }

        public string ProjectName { get; set; }

        /// <summary>
        /// Cari Hesap
        /// </summary>

        public string CheckingAccount { get; set; }

        /// <summary>
        ///  Cari Hesap Açılacak Mı?
        /// </summary>
        public bool CheckingAccountToBeCreated { get; set; }

        /// <summary>
        /// Cari Hesap Kodu
        /// </summary>

        public string CheckingAccountCode { get; set; }

        public string LogoFirmCode { get; set; }

        public string GroupCode { get; set; }

        public string Factory { get; set; }

        public string Departmant { get; set; }

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
        ///  is Active project
        /// </summary>
        public bool isActive { get; set; }

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
        public int DueDateDifference { get; set; }

        public string ProjectGroupCode { get; set; }

        public string DefaultCurrency { get; set; }
        public string Division { get; set; }
        public string Source_Cost_GRP { get; set; } = "15";
        public ProjectType ProjectType { get; set; }
    }
}