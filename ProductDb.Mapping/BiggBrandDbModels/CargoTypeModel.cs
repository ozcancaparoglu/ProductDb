using ProductDb.Common.Entities;
using System.ComponentModel.DataAnnotations;

namespace ProductDb.Mapping.BiggBrandDbModels
{
    public class CargoTypeModel : EntityBaseModel
    {
        [Required]
        [StringLength(250)]
        public string Name { get; set; }
    }
}