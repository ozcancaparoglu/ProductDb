using Microsoft.AspNetCore.Http;
using ProductDb.Mapping.BiggBrandDbModels;
using System.Collections.Generic;

namespace ProductDb.Admin.PageModels.Picture
{
    public class PictureViewModel
    {
        public string Sku { get; set; }
        public string PictureName { get; set; }
        public List<IFormFile> Files { get; set; }
        public PicturesModel Picture { get; set; }
    }
}
