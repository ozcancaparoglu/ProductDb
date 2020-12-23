using ProductDb.Admin.Areas.PMS.Models;
using System.Collections.Generic;

namespace ProductDb.Admin.Areas.PMS.Services
{
    public interface IApiExcelService
    {
        IList<ExcelOrderViewModel> ReadOrders(string path);
    }
}
