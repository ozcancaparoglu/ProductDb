using ProductDb.Common.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductDb.Data.BiggBrandsDb
{
    public class Pictures : EntityBase
    {
        public int? ProductId { get; set; }
        [ForeignKey("ProductId")]
        public Product Product { get; set; }
        public string Alt { get; set; }
        public string Title { get; set; }
        public int Order { get; set; }
        [Required]
        [StringLength(5000)]
        public string LocalPath { get; set; }
        [Required]
        [StringLength(5000)]
        public string CdnPath { get; set; }


    }
}
