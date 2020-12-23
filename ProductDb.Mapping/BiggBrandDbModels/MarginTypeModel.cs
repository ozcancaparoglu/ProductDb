using ProductDb.Common.Entities;
using System.ComponentModel.DataAnnotations;

namespace ProductDb.Mapping.BiggBrandDbModels
{
    public class MarginTypeModel : EntityBaseModel
    {
        [Required]
        [StringLength(250)]
        public string Name { get; set; }

        [Required]
        public int Rank { get; set; }
    }
}