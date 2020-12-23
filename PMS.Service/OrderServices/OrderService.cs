using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PMS.Data.Entities;
using PMS.Data.Entities.Order;
using PMS.Data.Repository;
using PMS.Mapping;
using PMS.Mapping.AutoMapperConfiguration;

namespace PMS.Service.OrderServices
{
    public class OrderService : IOrderService
    {
        private IRepository<Order> _orderRepository;
        private IRepository<OrderItem> _orderItemRepository;
        private IAutoMapperService _autoMapperService;
        private IRepository<ExOrder> _exOrderRepository;
        private IRepository<OrderTrackingNumber> _orderTrackingNumberRepository;

        public OrderService(IAutoMapperService autoMapperService,
                            IRepository<Order> orderRepository, IRepository<ExOrder> exOrderRepository,
                            IRepository<OrderItem> orderItemRepository,
                            IRepository<OrderTrackingNumber> orderTrackingNumberRepository)
        {
            _orderItemRepository = orderItemRepository;
            _orderRepository = orderRepository;
            _autoMapperService = autoMapperService;
            _exOrderRepository = exOrderRepository;
            _orderTrackingNumberRepository = orderTrackingNumberRepository;
        }
        public Order IsExOrderAdd(OrderModel model)
        {
            var isContain = _orderRepository.Table().FirstOrDefault(x => x.OrderNo.Trim() == model.OrderNo.Trim());
            if (isContain == null)
            {
                model.Id = 0;
                model.OrderItems.ToList().ForEach(x => x.Id = 0);
                var data = _orderRepository.Add(_autoMapperService.Map<OrderModel, Order>(model));
                return data;
            }
            else
            {
                return _orderRepository.Table().Include(x => x.OrderItems).FirstOrDefault(x => x.OrderNo == model.OrderNo);
            }

        }

        public Order Add(OrderModel model)
        {
            Order order = null;

            var isOrderContain = _orderRepository.Table().FirstOrDefault(x => x.OrderNo == model.OrderNo);

            if (isOrderContain == null)
                order = _orderRepository.Add(_autoMapperService.Map<OrderModel, Order>(model));
            else
            {
                order = _orderRepository.Table().Include(x => x.OrderItems).FirstOrDefault(x => x.OrderNo == model.OrderNo);
            }

            return order;
        }

        public ExOrder AddExOrder(ExOrderModel model)
        {
            var data = _exOrderRepository.Add(_autoMapperService.Map<ExOrderModel, ExOrder>(model));
            return data;
        }

        public IEnumerable<OrderModel> AllOrders()
        {
            var orders = _orderRepository.GetAll();
            return _autoMapperService.MapCollection<Order, OrderModel>(orders);
        }

        public async Task<IEnumerable<OrderModel>> AllOrdersByCompanyId(int companyid)
        {
            var orders = await _orderRepository.FilterAsync(a => a.LogoCompanyCode == companyid);

            return _autoMapperService.MapCollection<Order, OrderModel>(orders);
        }

        public IQueryable<ExOrder> AllQueryableExOrders(int companyId)
        {
            return _exOrderRepository.Table().Where(x => x.LogoCompanyCode == companyId && x.isDeleted == false && x.IsTransferred == false).OrderByDescending(x => x.Id);
        }

        public IEnumerable<OrderModel> AllQueryableOrders(int companyId, int skip, int take)
        {
            var orders = _orderRepository.Table().Where(x => x.LogoCompanyCode == companyId).Skip(skip).Take(take);
            return _autoMapperService.MapCollection<Order, OrderModel>(orders);
        }

        public IQueryable<Order> AllQueryableOrders(int companyId)
        {
            return _orderRepository.Table().Where(x => x.LogoCompanyCode == companyId).OrderByDescending(x => x.Id);
        }

        public IEnumerable<OrderModel> AllQueryableOrders(int companyid, int skip, int take, DateTime startDate, DateTime endDate)
        {
            var orders = _orderRepository.Table()
                .Where(x => x.LogoCompanyCode == companyid && x.CreateDate >= startDate && x.CreateDate <= endDate)
                .Skip(skip).Take(take);

            return _autoMapperService.MapCollection<Order, OrderModel>(orders);
        }

        public void ChangeExOrderStatus(int id, bool status)
        {
            var order = _exOrderRepository.Table().FirstOrDefault(x => x.Id == id);
            if (order != null)
            {
                order.IsTransferred = status;
                _exOrderRepository.Update(order);
            }
        }

        public void ChangeOrderStatus(int id, bool status)
        {
            var order = _orderRepository.Table().FirstOrDefault(x => x.Id == id);
            if (order != null)
            {
                order.IsTransferred = status;
                _orderRepository.Update(order);
            }
        }

        public void DeleteExOrder(int id, bool status)
        {
            var data = _exOrderRepository.Filter(x => x.Id == id).FirstOrDefault();
            if (data != null)
            {
                data.isDeleted = status;
                _exOrderRepository.Update(data);
            }
        }

        public int ExTotal(int companyId)
        {
            var total = _exOrderRepository.Table().Where(x => x.LogoCompanyCode == companyId && x.IsTransferred == false && x.isDeleted == false).Count();
            return total;
        }

        public ExOrderModel GetExOrderById(long id)
        {
            var data = _exOrderRepository.Filter(a => a.Id == id, null, "OrderItems").FirstOrDefault();
            return _autoMapperService.Map<ExOrder, ExOrderModel>(data);
        }

        public OrderModel GetOrderById(long id)
        {
            var data = _orderRepository.Filter(a => a.Id == id, null, "OrderItems").FirstOrDefault();
            return _autoMapperService.Map<Order, OrderModel>(data);
        }
        public async Task<IEnumerable<OrderModel>> GetOrderByProjeCode(string Code)
        {
            var datas = await _orderRepository.FilterAsync(a => a.ProjectCode == Code, null, "OrderItems");

            return _autoMapperService.MapCollection<Order, OrderModel>(datas).ToList();
        }

        public Order GetOrderFromExOrder(int id)
        {
            var orderModel = _autoMapperService.Map<ExOrder, Order>(
                    _exOrderRepository.Table().Where(x => x.Id == id).Include(x => x.OrderItems).FirstOrDefault());

            return orderModel;
        }

        public OrderModel OrderModel(Order model)
        {
            return _autoMapperService.Map<Order, OrderModel>(model);
        }

        public int Total(int companyId)
        {
            var total = _orderRepository.Table().Where(x => x.LogoCompanyCode == companyId).Count();
            return total;
        }

        public int TrackingNumberTotal(int companyId)
        {
            var total = _orderTrackingNumberRepository.Table().Where(x => x.CompanyId == companyId).Count();
            return total;
        }


        public void Update(OrderModel model)
        {
            var data = _autoMapperService.Map<OrderModel, Order>(model);
            _orderRepository.Update(data);
        }

        public void UpdateExOrderStatus(int id)
        {
            var exOrder = _exOrderRepository.Table().FirstOrDefault(x => x.Id == id);
            if (exOrder != null)
            {
                exOrder.IsTransferred = true;
                _exOrderRepository.Update(exOrder);
            }
        }

        public ExOrder UpdateExOrder(ExOrder model)
        {
            _exOrderRepository.Update(model);
            return model;
        }

        public ExOrder ExOrderById(int id)
        {
            var data = _exOrderRepository.Table().FirstOrDefault(x => x.Id == id);
            return data;
        }

        public bool ExOrderSendingStatus(OrderModel model)
        {
            bool status = false;

            var isContain = _orderRepository.Table().FirstOrDefault(x => x.OrderNo.Trim() == model.OrderNo.Trim());
            if (isContain != null)
                status = true;

            return status;
        }

        public List<OrderTrackingNumberModel> GetOrderTrackingNumbers(List<string> OrderNOs, int CompanyId)
        {
            var orders = _autoMapperService.MapCollection<OrderTrackingNumber, OrderTrackingNumberModel>(_orderTrackingNumberRepository.Table()
                .Where(x => OrderNOs.Any(k => k == x.OrderNo) && x.CompanyId == CompanyId))
                .ToList();

            return orders;
        }

        public IQueryable<OrderTrackingNumber> AllQueryableTrackingNumbers(int companyId)
        {
            return _orderTrackingNumberRepository.Table().Where(x => x.CompanyId == companyId).OrderByDescending(x => x.Id);
        }

        public OrderModel OrderModelbyOrderNo(string orderNo)
        {
            return _autoMapperService.Map<Order, OrderModel>(_orderRepository.Table().FirstOrDefault(x => x.OrderNo == orderNo));
        }

        public void UpdateTrackingNumberIsShipping(int id)
        {
            var orderTrackingNumber = _orderTrackingNumberRepository.Table().FirstOrDefault(x => x.Id == id);
            orderTrackingNumber.isActive = false;
            _orderTrackingNumberRepository.Update(orderTrackingNumber);
        }
    }
}
