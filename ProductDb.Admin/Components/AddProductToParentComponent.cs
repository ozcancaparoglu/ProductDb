using Microsoft.AspNetCore.Mvc;
using ProductDb.Admin.PageModels.Product;
using ProductDb.Services.ProductServices;
using System.Linq;

namespace ProductDb.Admin.Components
{
    public class AddProductToParentViewComponent : ViewComponent
    {
        private readonly IProductService productService;

        public AddProductToParentViewComponent(IProductService productService)
        {
            this.productService = productService;
        }

        public IViewComponentResult Invoke(int parentProductId)
        {
            var model = new AddProductParentViewModel()
            {
                ParentProductId = parentProductId,
                ProductId = 0,
                Products = productService.AllProducts().Where(x => x.ParentProductId != parentProductId).ToList()
            };

            return View("_AddProductToParentView", model);
        }
    }
}
