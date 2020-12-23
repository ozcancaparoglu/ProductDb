using ProductDb.Common.Entities;
using System.ComponentModel.DataAnnotations;

namespace ProductDb.Mapping.BiggBrandDbModels
{
    public class SupplierModel : EntityBaseModel
    {
        [Required(ErrorMessage = "*This field is mandatory")]
        public string Name { get; set; }

        public string ManufacturerPartNumber { get; set; }
        public string Prefix { get; set; }
    }
}