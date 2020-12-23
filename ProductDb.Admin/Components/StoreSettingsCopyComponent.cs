using Microsoft.AspNetCore.Mvc;
using ProductDb.Admin.PageModels.Store;
using ProductDb.Services.StoreServices;
using System.Linq;

namespace ProductDb.Admin.Components
{
    [ViewComponent(Name = "StoreSettingsCopy")]
    public class StoreSettingsCopyComponent : ViewComponent
    {
        private readonly IStoreService storeService;

        public StoreSettingsCopyComponent(IStoreService storeService)
        {
            this.storeService = storeService;
        }

        public IViewComponentResult Invoke(int id, int typeId, string typeName)
        {
            var viewModel = new StoreProductViewModel
            {
                StoreId = id,
                Stores = storeService.AllStores().ToList(),
                StoreCopyType = typeId,
                StoreCopyTypeName = typeName
            };
            return View("_StoreSettingsCopy", viewModel);
        }
    }
}
