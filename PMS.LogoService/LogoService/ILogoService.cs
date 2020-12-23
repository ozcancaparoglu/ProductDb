
using LogoObjectClient;
using PMS.Data.Entities;
using PMS.Data.Entities.Order;
using PMS.LogoService.Helper;
using PMS.LogoService.LogoModels;
using PMS.Mapping;
using System.Collections.Generic;
using System.Threading.Tasks;
using Invoice = PMS.Data.Entities.Invoice.Invoice;

namespace PMS.LogoService.LogoService
{
    public interface ILogoService
    {
        /// <summary>
        ///  Logo Execute Query
        /// </summary>
        /// <param name="sqlText"></param>
        /// <returns></returns>
        Task<ExecQueryResponse> ExecuteQuery(string sqlText);
        /// <summary>
        ///  Logo Direct Execute Query
        /// </summary>
        /// <param name="sqlText"></param>
        /// <returns></returns>
        Task<DirectQueryResponse> DirectQuery(string sqlText);
        /// <summary>
        ///  Send To Logo Generic Method
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="dataType"></param>
        /// <param name="companyId"></param>
        /// <returns></returns>
        Task<AppendDataObjectResponse> SendToLogo<T>(T t, LogoDataType dataType, int companyId) where T : class;
        /// <summary>
        ///  Insert Invoice
        /// </summary>
        /// <param name="order"></param>
        /// <param name="companyId"></param>
        /// <returns></returns>
        Task<LogoResponseModel> InsertInvoice(Invoice order, int companyId);
        /// <summary>
        ///  Insert Order
        /// </summary>
        /// <param name="order"></param>
        /// <param name="companyId"></param>
        /// <returns></returns>
        LogoResponseModel InsertOrder(Order order, int companyId, LogoCompanyModel logoCompanySettings);
        /// <summary>
        ///  Customer Shipment Account
        /// </summary>
        /// <param name="order"></param>
        /// <param name="companyId"></param>
        /// <returns></returns>
        LogoResponseModel CreateShipmentAccount(Order order, int companyId);
        /// <summary>
        ///  Customer Account
        /// </summary>
        /// <param name="order"></param>
        /// <param name="companyId"></param>
        /// <returns></returns>
        LogoResponseModel CreateCheckingAccount(Order order, int companyId);
        /// <summary>
        ///  Convert Order To Logo Order Model
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        LogoOrderModel ConvertOrderToLogoOrder(OrderModel model, LogoCompanyModel logoCompanySettings);
        /// <summary>
        ///  Convert Invoice To Logo Invoice Model
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        LogoInvoiceModel ConvertOrderToLogoDiffInvoice(InvoiceModel model);
        /// <summary>
        ///  Convert Order Customer To Logo Customer
        /// </summary>
        /// <param name="model"></param>
        /// <param name="companyId"></param>
        /// <returns></returns>
        LogoCustomerModel ConvertOrderCustomerToLogoCustomer(OrderModel model, int companyId);
        /// <summary>
        ///  Convert Customer Shipping
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        LogoCustomerShippingModel ConvertLogoCustomerShipping(OrderModel model);

        /// <summary>
        ///  Insert Item Mark
        /// </summary>
        /// <param name="markModel"></param>
        /// <returns></returns>
        Task<LogoResponseModel> InsertItemMark(LogoMarksModel markModel, int companyId);
        /// <summary>
        ///  Insert Metarial Card
        /// </summary>
        /// <param name="markModel"></param>
        /// <returns></returns>
        Task<LogoResponseModel> InsertItem(LogoItemModel itemCardModel, int companyId);
        /// <summary>
        ///  Logo Company Control
        /// </summary>
        /// <param name="CompanyId"></param>
        /// <returns></returns>
        LogoCompanyModel isLogoCompany(int CompanyId);
        /// <summary>
        /// Order Items In Logo Control
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        LogoResponseModel isItemDefinedInLogo(Order model, int companyId);
        /// <summary>
        ///  Logo Project Control
        /// </summary>
        /// <param name="model"></param>
        /// <param name="companyId"></param>
        /// <returns></returns>
        LogoResponseModel isProjectDefinedInLogo(OrderModel model);
        /// <summary>
        ///  Get Order Tracking Numbers By Sku List
        /// </summary>
        /// <param name="SKUs"></param>
        /// <param name="companyId"></param>
        /// <param name="periodid"></param>
        /// <returns></returns>
        OrderTrackingNumberModel GetOrderTrackingNumbersBySku(List<string> SKUs,int companyId,string periodid);
        
        /// <summary>
        ///  Update Tracking Number
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="periodId"></param>
        Task UpdateTrackingNumbers(int companyId,string periodId = "01");

        /// <summary>
        ///  PrepareOrderTracking Number
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="periodId"></param>
        /// <returns></returns>
        Task<List<OrderTrackingNumberModel>> PrepareOrderTrackingNumber(int companyId, string periodId = "01");
    }
}
