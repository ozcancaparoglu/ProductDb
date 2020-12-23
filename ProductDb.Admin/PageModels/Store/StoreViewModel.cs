using ProductDb.Mapping.BiggBrandDbModels;
using System.Collections.Generic;

namespace ProductDb.Admin.PageModels.Store
{
    public class StoreViewModel
    {
        public StoreModel Store { get; set; }
        public List<WarehouseTypeModel> Warehouses { get; set; }
        public List<StoreWarehouseMappingModel> StoreWarehouseMappings { get; set; }
        public List<CurrencyModel> Currencies { get; set; }
        public List<FormulaGroupModel> FormulaGroups { get; set; }
        public List<CargoTypeModel> CargoTypes { get; set; }
        public List<ErpCompanyModel> ErpCompanies { get; set; }
        public List<StoreTypeModel> StoreTypes { get; set; }
    }
}
