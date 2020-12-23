using ProductDb.Common.Entities;
using System.ComponentModel.DataAnnotations;

namespace ProductDb.Mapping.BiggBrandDbModels
{
    public class PermissionModel: EntityBaseModel
    {
        [Required]
        [StringLength(200)]
        public string name { get; set; }

        [Required]
        [StringLength(200)]
        public string key { get; set; }

        [Required]
        public string Description { get; set; }
    }
}
