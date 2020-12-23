using Microsoft.AspNetCore.Http;
using ProductDb.Common.Entities;
using System.Collections.Generic;

namespace ProductDb.Mapping.BiggBrandDbModels
{
    public class PicturesModel : EntityBaseModel
    {
        public int? ProductId { get; set; }
        public ProductModel Product { get; set; }

        public string LocalPath { get; set; }
        public string CdnPath { get; set; }
        public string Alt { get; set; }
        public string Title { get; set; }
        public int Order { get; set; }

        public List<IFormFile> Files { get; set; }
        public string Sku { get; set; }
    }
}
