using ProductDb.Common.Entities;
using System.ComponentModel.DataAnnotations;

namespace ProductDb.Data.BiggBrandsDb
{
    public class MarginType : EntityBase
    {
        [Required]
        [StringLength(250)]
        public string Name { get; set; }

        [Required]
        public int Rank { get; set; }
    }
}
