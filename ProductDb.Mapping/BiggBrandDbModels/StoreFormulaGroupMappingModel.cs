using ProductDb.Common.Entities;

namespace ProductDb.Mapping.BiggBrandDbModels
{
    public class StoreFormulaGroupMappingModel : EntityBaseModel
    {
        public int? StoreId { get; set; }
        public virtual StoreModel Store { get; set; }

        public int? FormulaGroupId { get; set; }
        public virtual FormulaGroupModel FormulaGroup { get; set; }
    }
}
