using Microsoft.AspNetCore.Mvc;
using PMS.Common;
using ProductDb.Admin.Areas.PMS.Services;
using ProductDb.Admin.Areas.PMS.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductDb.Admin.Areas.PMS.Components
{
    [ViewComponent(Name = "OrderDetail")]
    public class OrderDetailComponent : ViewComponent
    {
        private readonly IOrderService orderService;

        public OrderDetailComponent(IOrderService orderService)
        {
            this.orderService = orderService;
        }

        public IViewComponentResult Invoke(int id)
        {
            var _orderDetial = orderService.GetOrderDetail(id);

            var viewModel = new OrderDetailViewModel
            {
                companyId = _orderDetial.LogoCompanyCode,
                Items = _orderDetial.OrderItems.ToList()
            };

            return View("_OrderDetail", viewModel);
        }
    }
}
