using ProductDb.Common.Entities;
using System.ComponentModel.DataAnnotations;

namespace ProductDb.Mapping.BiggBrandDbModels
{
    public class TransportationTypeModel : EntityBaseModel
    {
        [Required]
        [StringLength(500)]
        public string Name { get; set; }

        [Required]
        public int Rank { get; set; }
    }
}