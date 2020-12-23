using ProductDb.Common.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductDb.Data.BiggBrandsDb.ProductDocks
{
    public class ProductDockPictures : EntityBase
    {
        public int? ProductDockId { get; set; }
        [ForeignKey("ProductDockId")]
        public virtual ProductDock ProductDock { get; set; }
        public string DownloadUrl { get; set; }
    }
}
