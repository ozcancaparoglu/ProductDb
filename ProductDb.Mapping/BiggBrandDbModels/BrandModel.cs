using ProductDb.Common.Entities;
using System.ComponentModel.DataAnnotations;

namespace ProductDb.Mapping.BiggBrandDbModels
{
    public class BrandModel : EntityBaseModel
    {
        [Required(ErrorMessage = "*This field is mandotary")]
        public string Name { get; set; }
        [Required(ErrorMessage = "*This field is mandotary")]
        public string Prefix { get; set; }
    }
}