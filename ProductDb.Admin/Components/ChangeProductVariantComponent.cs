using Microsoft.AspNetCore.Mvc;
using ProductDb.Admin.PageModels.ProductVariant;
using ProductDb.Services.ProductVariantServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductDb.Admin.Components
{
    [ViewComponent(Name = "ChangeProductVariant")]
    public class ChangeProductVariantComponent : ViewComponent
    {
        private readonly IProductVariantService productVariantService;

        public ChangeProductVariantComponent(IProductVariantService productVariantService)
        {
            this.productVariantService = productVariantService;
        }
        public IViewComponentResult Invoke(int id)
        {
            var model = new ChangeProductVariantViewModel
            {
                ProductId = id,
                Products = productVariantService.AllProductVariantWithProduct().ToList()
            };
            // prepare
            model.Products.ToList().ForEach(x => x.Title = $"{x.Sku}/ {x.Title}");
            return View("_ChangeProductVariant", model);
        }
    }
}
