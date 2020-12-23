using ProductDb.Common.Entities;

namespace ProductDb.Mapping.BiggBrandDbModels
{
    public class TransportationModel : EntityBaseModel
    {
        public int? StoreId { get; set; }
        public virtual StoreModel Store { get; set; }

        public int? TransportationTypeId { get; set; }
        public virtual TransportationTypeModel TransportationType { get; set; }

        public int? EntityId { get; set; }

        public decimal Value { get; set; }

        public int? CurrencyId { get; set; }
        public virtual CurrencyModel Currency { get; set; }
        // for navigations

        public string Name { get; set; }

    }
}
