using ProductDb.Common.Entities;
using System.ComponentModel.DataAnnotations;

namespace ProductDb.Mapping.BiggBrandDbModels
{
    public class ProductGroupModel : EntityBaseModel
    {
        [Required]
        public string Name { get; set; }

        public string Code { get; set; }
    }
}
