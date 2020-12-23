using Microsoft.AspNetCore.Mvc;
using ProductDb.Mapping.BiggBrandDbModels;
using ProductDb.Services.ProductServices;
using ProductDb.Services.ProductVariantServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductDb.Admin.Components
{
    [ViewComponent(Name = "AddProductVariant")]
    public class AddProductVariantComponent: ViewComponent
    {
        private readonly IProductService productService;
        private readonly IProductVariantService productVariantService;

        public AddProductVariantComponent(IProductVariantService productVariantService,IProductService productService)
        {
            this.productService = productService;
            this.productVariantService = productVariantService;
        }

        public IViewComponentResult Invoke(int id, int parentProductId,List<int> IDs)
        {
            var currProduct = productService.ProductById(id);

            var data = new ProductVariantModel
            {
                ParentProductId = parentProductId,
                Product = currProduct,
                IDs = IDs
            };
            return View("_AddProductVariant", data);
        }

    }
}
