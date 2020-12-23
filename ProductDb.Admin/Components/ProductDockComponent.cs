using Microsoft.AspNetCore.Mvc;
using ProductDb.Services.ProductDockServices;
using ProductDb.Services.TaxServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductDb.Admin.Components
{
    [ViewComponent(Name = "ProductDock")]
    public class ProductDockComponent : ViewComponent
    {
        private readonly ITaxService taxService;
        private readonly IProductDockService productDockService;

        public ProductDockComponent(IProductDockService productDockService,
                                    ITaxService taxService)
        {
            this.taxService = taxService;
            this.productDockService = productDockService;
        }
        public IViewComponentResult Invoke(int id)
        {
            var productDock = productDockService.ProductDockbyId(id);

            productDock.PsfPriceString = productDock.PsfPrice.ToString();
            productDock.CostString = productDock.CostString ?? 0.ToString();
            productDock.DesiString = productDock.DesiString ?? 0.ToString();
            productDock.CostString = productDock.CostString ?? 0.ToString();

            productDock.VatRates = taxService.AllTaxRate();
            productDock.Categories = productDockService.ProductDockCategories();

            return View("_ProductDock", productDock);
        }
    }
}
