using ProductDb.Common.Entities;

namespace ProductDb.Mapping.BiggBrandDbModels
{
    public class CargoModel : EntityBaseModel
    {
        public int? StoreId { get; set; }
        public virtual StoreModel Store { get; set; }

        public decimal MinDesi { get; set; }

        public decimal MaxDesi { get; set; }

        public decimal Value { get; set; }

        public bool IsLastDesi { get; set; }


    }
}
