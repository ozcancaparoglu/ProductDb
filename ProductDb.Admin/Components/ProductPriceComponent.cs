using Microsoft.AspNetCore.Mvc;
using ProductDb.Services.ProductServices;
using System.Linq;

namespace ProductDb.Admin.Components
{
    public class ProductPriceViewComponent : ViewComponent
    {
        private readonly IProductService productService;

        public ProductPriceViewComponent(IProductService productService)
        {
            this.productService = productService;
        }

        public IViewComponentResult Invoke(int productId)
        {
            var model = productService.ProductPricesWithStore(productId).ToList();

            return View("_ProductPriceView", model);
        }
    }
}
