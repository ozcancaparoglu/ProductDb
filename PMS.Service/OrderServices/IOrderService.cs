using PMS.Data.Entities.Order;
using PMS.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PMS.Service.OrderServices
{
    public interface IOrderService
    {
        Order Add(OrderModel model);
        bool ExOrderSendingStatus(OrderModel model);
        Order IsExOrderAdd(OrderModel model);
        ExOrder UpdateExOrder(ExOrder model);
        void UpdateExOrderStatus(int id);
        OrderModel OrderModel(Order model);
        ExOrder AddExOrder(ExOrderModel model);
        void ChangeExOrderStatus(int id, bool status);
        void DeleteExOrder(int id, bool status);
        void ChangeOrderStatus(int id, bool status);
        IEnumerable<OrderModel> AllOrders();
        IQueryable<Order> AllQueryableOrders(int companyId);
        IQueryable<ExOrder> AllQueryableExOrders(int companyId);
        IEnumerable<OrderModel> AllQueryableOrders(int companyid, int skip, int take);
        Task<IEnumerable<OrderModel>> AllOrdersByCompanyId(int companyid);
        Task<IEnumerable<OrderModel>> GetOrderByProjeCode(string Code);
        OrderModel GetOrderById(long id);
        ExOrderModel GetExOrderById(long id);
        int Total(int companyId);
        int ExTotal(int companyId);
        IEnumerable<OrderModel> AllQueryableOrders(int companyid, int skip, int take, DateTime startDate, DateTime endDate);
        Order GetOrderFromExOrder(int id);
        ExOrder ExOrderById(int id);
        List<OrderTrackingNumberModel> GetOrderTrackingNumbers(List<string> OrderNOs,int CompanyId);
        IQueryable<OrderTrackingNumber> AllQueryableTrackingNumbers(int companyId);
        OrderModel OrderModelbyOrderNo(string orderNo);
        void UpdateTrackingNumberIsShipping(int id);
        int TrackingNumberTotal(int companyId);
    }
}
