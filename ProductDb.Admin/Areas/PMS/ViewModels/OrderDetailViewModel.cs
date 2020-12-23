using PMS.Common.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductDb.Admin.Areas.PMS.ViewModels
{
    public class OrderDetailViewModel
    {
        public int companyId { get; set; }
        public int orderId { get; set; }
        public string logoMessage { get; set; }
        public List<OrderItem> Items { get; set; }
    }
}
