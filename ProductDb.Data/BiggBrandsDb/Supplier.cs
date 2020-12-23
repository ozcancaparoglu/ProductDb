using ProductDb.Common.Entities;
using System.ComponentModel.DataAnnotations;

namespace ProductDb.Data.BiggBrandsDb
{
    public class Supplier : EntityBase
    {
        [StringLength(250)]
        public string Name { get; set; }

        [StringLength(100)]
        public string ManufacturerPartNumber { get; set; }

        [StringLength(100)]
        public string Prefix { get; set; }

    }
}