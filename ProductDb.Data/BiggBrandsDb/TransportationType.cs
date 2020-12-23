using ProductDb.Common.Entities;
using System.ComponentModel.DataAnnotations;

namespace ProductDb.Data.BiggBrandsDb
{
    public class TransportationType : EntityBase
    {
        [Required]
        [StringLength(500)]
        public string Name { get; set; }

        [Required]
        public int Rank { get; set; }
    }
}
