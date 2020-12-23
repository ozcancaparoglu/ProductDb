using PMS.Common.Dto;
using ProductDb.Admin.Areas.PMS.Models;
using ProductDb.Admin.PageModels.Filter;
using ProductDb.Mapping.BiggBrandDbModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductDb.Admin.Areas.PMS.Services
{
    public interface IOrderService
    {
        Task<List<T>> GetCachedValues<T>(string path, string CacheKey, int CacheTime);
        Task<List<DateDiffInvoice>> GetInvoices(int companyid, string periodid = "01");
        List<Order> GetOrders(int companyid, KendoFilterModel kendoFilterModel, out int total);
        List<Order> GetExOrders(int companyid, KendoFilterModel kendoFilterModel, out int total);
        Order GetOrderDetail(int id);
        Order GetExOrderDetail(int id);
        /// <summary>
        ///  POst Order To Api
        /// </summary>
        /// <param name="order"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        string PostOrder(Order order, string path);

        /// <summary>
        ///  Load Excel Order List To Post Order
        /// </summary>
        /// <param name="orderList"></param>
        void PostOrderList(IList<ExcelOrderViewModel> orderList, int storeId, int claimId, out string errorList);
        /// <summary>
        ///  Prepare Excel Order List
        /// </summary>
        /// <param name="orderList"></param>
        /// <returns></returns>
        List<Order> PrepareOrders(IList<ExcelOrderViewModel> orderList, int storeId, int claimId, out string errorList);
        /// <summary>
        ///  Save Ex Orders
        /// </summary>
        /// <param name="selectedOrders"></param>
        string SaveOrders(List<Order> selectedOrders, string path, int companyId);
        /// <summary>
        ///  Delete Exorder From Api
        /// </summary>
        /// <param name="id"></param>
        void DeleteExOrder(int id);
        /// <summary>
        ///  Tracking Number By Company Id
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        List<OrderTrackingNumberDto> GetOrderTrackingNumbers(int companyid, KendoFilterModel kendoFilterModel, out int total);
        /// <summary>
        ///  Update Tracking Number
        /// </summary>
        /// <param name="companyId"></param>
        bool UpdateTrackingNumbers(int companyId);
    }
}
