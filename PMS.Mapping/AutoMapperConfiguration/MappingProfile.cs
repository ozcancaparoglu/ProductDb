using AutoMapper;
using PMS.Data.Entities.Invoice;
using PMS.Data.Entities.Item;
using PMS.Data.Entities.Logo;
using PMS.Data.Entities.Logs;
using PMS.Data.Entities.Order;
using PMS.Data.Entities.Project;

namespace PMS.Mapping.AutoMapperConfiguration
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<OrderModel, Order>().ReverseMap();
            CreateMap<OrderItemModel, OrderItem>().ReverseMap();
            // External Models
            CreateMap<ExOrderModel, ExOrder>().ReverseMap();
            CreateMap<Order, ExOrder>().ReverseMap();
            CreateMap<OrderModel, ExOrder>().ReverseMap();
            CreateMap<OrderItemModel, ExOrderItem>().ReverseMap();
            CreateMap<LogoCompanyModel, LogoCompany>().ReverseMap();
            // Tracking Numbers of Orders
            CreateMap<OrderTrackingNumber, OrderTrackingNumberModel>().ReverseMap();

            CreateMap<OrderItem, ExOrderItem>().ReverseMap();
            CreateMap<ExOrderItemModel, ExOrderItem>().ReverseMap();

            CreateMap<ProjectModel, Project>().ReverseMap();
            CreateMap<Invoice, InvoiceModel>().ReverseMap();
            CreateMap<InvoiceItem, InvoiceItemModel>().ReverseMap();
            CreateMap<ItemVatRate, ItemVatRateModel>().ReverseMap();
            CreateMap<Logs, LogModel>().ReverseMap();
        }
    }
}
