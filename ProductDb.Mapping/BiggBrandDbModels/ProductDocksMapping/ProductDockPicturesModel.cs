using ProductDb.Common.Entities;

namespace ProductDb.Mapping.BiggBrandDbModels.ProductDocksMapping
{
    public class ProductDockPicturesModel : EntityBaseModel
    {
        public int? ProductDockId { get; set; }
        public virtual ProductDockModel ProductDock { get; set; }
        public string DownloadUrl { get; set; }
    }
}
