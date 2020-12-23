using ProductDb.Common.Entities;

namespace ProductDb.Data.BiggBrandsDb
{
    public class VatRate : EntityBase
    {
        public string Name { get; set; }
        public int Value { get; set; }
    }
}
