using Microsoft.AspNetCore.Mvc;
using ProductDb.Admin.PageModels.Store;
using ProductDb.Services.StoreServices;
using System.Linq;

namespace ProductDb.Admin.Components
{
    [ViewComponent(Name = "StoreCopy")]
    public class StoreCopyComponent : ViewComponent
    {
        private readonly IStoreService storeService;

        public StoreCopyComponent(IStoreService storeService)
        {
            this.storeService = storeService;
        }
        public IViewComponentResult Invoke(int id)
        {
            var viewModel = new StoreProductViewModel
            {
                StoreId = id,
                Stores = storeService.AllStores().ToList()
            };
            return View("_StoreCopy", viewModel);
        }
    }
}
