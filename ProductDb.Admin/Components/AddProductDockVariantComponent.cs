using Microsoft.AspNetCore.Mvc;
using ProductDb.Admin.PageModels.ProductDock;
using ProductDb.Services.ProductDockServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductDb.Admin.Components
{
    [ViewComponent(Name = "AddProductDockVariant")]
    public class AddProductDockVariantComponent: ViewComponent
    {
        private readonly IProductDockService productDockService;

        public AddProductDockVariantComponent(IProductDockService productDockService)
        {
            this.productDockService = productDockService;
        }
        public IViewComponentResult Invoke(int id)
        {
            var parentProductDock = productDockService.ProductDockbyId(id);

            var productDockVariantModel = new ProductDockVariantViewModel
            {
                NotVariantedProductDocks = productDockService.NotVariantedProductDocks(id).ToList(),
                ProductDock = parentProductDock,
                ParentProductDockId = parentProductDock.Id
            };

            return View("_AddProductDockVariant", productDockVariantModel);
        }
    }
}
