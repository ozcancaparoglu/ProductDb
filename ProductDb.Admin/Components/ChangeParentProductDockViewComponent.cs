using Microsoft.AspNetCore.Mvc;
using ProductDb.Admin.PageModels.ProductDock;
using ProductDb.Services.ProductDockServices;
using ProductDb.Services.ProductServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductDb.Admin.Components
{
    [ViewComponent(Name = "ChangeParentProductDock")]
    public class ChangeParentProductDockViewComponent : ViewComponent
    {
        private readonly IProductDockService productDockService;
        private readonly IParentProductDockService parentProductDockService;

        public ChangeParentProductDockViewComponent(IParentProductDockService parentProductDockService,
                                                    IProductDockService productDockService)
        {
            this.productDockService = productDockService;
            this.parentProductDockService = parentProductDockService;
        }
        public IViewComponentResult Invoke(int id)
        {
            var productDock = productDockService.GetProductDockbyId(id);

            var viewModel = new ChangeParentProductDockViewModel
            {
                ParentProductDocks = parentProductDockService.AllParentProductDocks().Where(x => x.Id !=
                productDock.ParentProductDockId).ToList(),
                id = id
            };
            return View("_ChangeParentProductDock", viewModel);
        }
    }
}
