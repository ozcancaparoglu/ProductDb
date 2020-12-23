using ProductDb.Common.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductDb.Data.BiggBrandsDb
{
    public class Store : EntityBase
    {
        public string Name { get; set; }

        public int? CurrencyId { get; set; }

        [ForeignKey("CurrencyId")]
        public virtual Currency Currency { get; set; }

        public int? CargoCurrencyId { get; set; }

        [ForeignKey("CargoCurrencyId")]
        public virtual Currency CargoCurrency { get; set; }

        public int? FormulaGroupId { get; set; }
        [ForeignKey("FormulaGroupId")]
        public virtual FormulaGroup FormulaGroup { get; set; }

        public int? CargoTypeId { get; set; }
        [ForeignKey("CargoTypeId")]
        public virtual CargoType CargoType { get; set; }

        public int? ErpCompanyId { get; set; }
        [ForeignKey("ErpCompanyId")]
        public virtual ErpCompany ErpCompany { get; set; }

        public int? StoreTypeId { get; set; }
        [ForeignKey("StoreTypeId")]
        public virtual StoreType StoreType { get; set; }

        public int MinStock { get; set; }
        public int MaxStock { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? MinPrice { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? MinPoint { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? MaxPrice { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? MaxPoint { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? Rate { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? DefaultMarj { get; set; }
        public string ProjectCode { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? Sarf { get; set; }
    }
}
