using ProductDb.Common.Entities;

namespace ProductDb.Mapping.BiggBrandDbModels
{
    public class LanguageModel : EntityBaseModel
    {
        public string Name { get; set; }
        public string Abbrevation { get; set; }
        public string LogoPath { get; set; }
        public bool IsDefault { get; set; }
    }
}
