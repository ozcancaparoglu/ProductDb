using ProductDb.Common.Entities;

namespace ProductDb.Mapping.BiggBrandDbModels
{
    public class CurrencyModel : EntityBaseModel
    {
        public string Name { get; set; }
        public string Abbrevation { get; set; }
        public decimal Value { get; set; }
        public decimal LiveValue { get; set; }
        public bool IsInMB { get; set; }

    }
}
