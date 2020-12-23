using Microsoft.AspNetCore.Mvc;
using ProductDb.Admin.Areas.PMS.Services;
using ProductDb.Admin.Areas.PMS.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductDb.Admin.Areas.PMS.Components
{
    [ViewComponent(Name = "ExOrderDetail")]
    public class ExOrderDetailComponent : ViewComponent
    {
        private readonly IOrderService orderService;

        public ExOrderDetailComponent(IOrderService orderService)
        {
            this.orderService = orderService;
        }

        public IViewComponentResult Invoke(int id)
        {
            var _orderDetial = orderService.GetExOrderDetail(id);

            var viewModel = new OrderDetailViewModel
            {
                Items = _orderDetial.OrderItems.ToList()
            };

            return View("_ExOrderDetail", viewModel);
        }
    }
}
