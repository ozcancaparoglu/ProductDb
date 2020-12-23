using ProductDb.Common.Entities;
using System.ComponentModel.DataAnnotations;

namespace ProductDb.Mapping.BiggBrandDbModels
{
    public class StoreModel : EntityBaseModel
    {
        [Required(ErrorMessage = "*This field is mandotary")]
        public string Name { get; set; }

        public int? CurrencyId { get; set; }
        public virtual CurrencyModel Currency { get; set; }

        public int? CargoCurrencyId { get; set; }

        public virtual CurrencyModel CargoCurrency { get; set; }

        public int? FormulaGroupId { get; set; }
        public virtual FormulaGroupModel FormulaGroup { get; set; }

        public int? CargoTypeId { get; set; }
        public virtual CargoTypeModel CargoType { get; set; }

        public int? ErpCompanyId { get; set; }
        public virtual ErpCompanyModel ErpCompany { get; set; }

        public int? StoreTypeId { get; set; }
        public virtual StoreTypeModel StoreType { get; set; }

        public int MinStock { get; set; }
        public int MaxStock { get; set; }

        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public decimal? MinPoint { get; set; }
        public decimal? MaxPoint { get; set; }
        public decimal? Rate { get; set; }
        public decimal? Sarf { get; set; }
        public decimal? DefaultMarj { get; set; }

        public string MinPriceString { get; set; }
        public string MaxPriceString { get; set; }
        public string MinPointString { get; set; }
        public string MaxPointString { get; set; }
        public string RateString { get; set; }
        public string SarfString { get; set; }
        public string DefaultMarjString { get; set; }
        public string StoreTypeString { get; set; }

        public string ProjectCode { get; set; }
        
    }
}
