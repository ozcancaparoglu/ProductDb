using Microsoft.AspNetCore.Mvc;
using ProductDb.Admin.PageModels.Picture;
using ProductDb.Mapping.BiggBrandDbModels;

namespace ProductDb.Admin.Components
{
    public class AddPictureViewComponent : ViewComponent
    {

        public IViewComponentResult Invoke(string sku, int productId)
        {
            var model = new PictureViewModel
            {
                Sku = sku,
                Picture = new PicturesModel() { ProductId = productId }
            };

            return View("_AddPictureView", model);
        }

    }
}
