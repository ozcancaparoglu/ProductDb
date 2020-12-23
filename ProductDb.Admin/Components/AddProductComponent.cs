using Microsoft.AspNetCore.Mvc;
using ProductDb.Admin.PageModels.Product;
using ProductDb.Services.ProductServices;
using System.Linq;

namespace ProductDb.Admin.Components
{
    public class AddProductViewComponent : ViewComponent
    {
        private readonly IProductService productService;

        public AddProductViewComponent(IProductService productService)
        {
            this.productService = productService;
        }

        public IViewComponentResult Invoke(int storeId)
        {
            var model = new AddProductViewModel()
            {
                StoreId = storeId,
                ProductId = 0,
                Products = productService.ProductsNotInStore(storeId).ToList()
            };

            return View("_AddProductView", model);
        }
    }
}
