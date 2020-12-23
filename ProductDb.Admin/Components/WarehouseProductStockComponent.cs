using Microsoft.AspNetCore.Mvc;
using ProductDb.Services.WarehouseServices;

namespace ProductDb.Admin.Components
{
    [ViewComponent(Name = "WarehouseProductStock")]
    public class WarehouseProductStockComponent: ViewComponent
    {
        private IWarehouseService _warehouseService;

        public WarehouseProductStockComponent(IWarehouseService warehouseService)
        {
            _warehouseService = warehouseService;
        }

        public IViewComponentResult Invoke(int? id = null)
        {
            var productStocksModel = id == null ? _warehouseService.WarehouseProductStockModels() :
                                _warehouseService.WarehouseProductStockModels(id.Value);
            return View("_WarehouseProductStockView", productStocksModel);
        }
    }
}
