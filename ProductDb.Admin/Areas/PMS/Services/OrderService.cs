using Newtonsoft.Json;
using PMS.Common;
using PMS.Common.Dto;
using ProductDb.Admin.Areas.PMS.Models;
using ProductDb.Admin.PageModels.Filter;
using ProductDb.Common.Cache;
using ProductDb.Data.BiggBrandsDb;
using ProductDb.Mapping.BiggBrandDbModels;
using ProductDb.Services.ErpComanyService;
using ProductDb.Services.ProductServices;
using ProductDb.Services.StoreServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ProductDb.Admin.Areas.PMS.Services
{
    public class OrderService : IOrderService
    {
        private readonly IProductService productService;
        private readonly IErpCompanyService erpCompanyService;
        private readonly IStoreService storeService;
        private readonly IApiRepo apiRepo;
        private readonly ICacheManager cacheManager;

        public OrderService(ICacheManager cacheManager, IApiRepo apiRepo, IStoreService storeService, IErpCompanyService erpCompanyService,
            IProductService productService)
        {
            this.productService = productService;
            this.erpCompanyService = erpCompanyService;
            this.storeService = storeService;
            this.apiRepo = apiRepo;
            this.cacheManager = cacheManager;
        }

        public void DeleteExOrder(int id)
        {
            var result = apiRepo.GetItem<int>($"order/delete-exorder/{id}", "", Endpoints.PMS);
        }

        public async Task<List<T>> GetCachedValues<T>(string path, string CacheKey, int CacheTime)
        {
            var datas = new List<T>();

            if (!cacheManager.TryGetValue(CacheKey, out datas))
            {
                datas = cacheManager.Set(CacheKey,
                              await apiRepo.GetList<T>(path, "", Endpoints.PMS),
                              CacheTime);

            }
            return datas;
        }

        public Order GetExOrderDetail(int id)
        {
            var _orderDetail = apiRepo.GetItem<Order>($"order/get-exorder-detail/{id}", "", Endpoints.PMS).Result;
            return _orderDetail;
        }

        public List<Order> GetExOrders(int companyid, KendoFilterModel kendoFilterModel, out int total)
        {

            // server side company order list
            var orders = apiRepo.Post($"order/get-exorders/{companyid}", kendoFilterModel,
                "", Endpoints.PMS).Result;

            var list = JsonConvert.DeserializeObject<OrderDataModel>(orders.JsonContent);
            // server side company order total
            //total = apiRepo.GetItem<int>($"order/get-exorder-count/{companyid}",
            //    "", Endpoints.PMS).Result;
            total = list.total;

            return list.datas;
        }

        public async Task<List<DateDiffInvoice>> GetInvoices(int companyid, string periodid = "01")
        {
            var invoices = await GetCachedValues<DateDiffInvoice>($"invoice/get-datediff-invoices/{companyid}/{periodid}",
                CacheStatics.InvoiceCacheKey, CacheStatics.InvoiceCacheTime);

            invoices.ForEach(a => a.COMPANYCODE = $"{companyid}");
            //var invoices = await GetCachedInvoices(companyid);
            //var CompanyCode = $"{companyid}";
            //var groupedInvoices = invoices.GroupBy(a => new
            //{
            //    a.PROJECTCODE,
            //    a.INVOICENO,
            //    a.ORDERNO,
            //    a.SHIPPINGADRESSCODE,
            //    a.SHIPPINGNAME,
            //    a.ADRESS1,
            //    a.ADRESS2,
            //    a.CITY,
            //    a.TOWN,
            //    a.TELNRS1,
            //    a.COMPANYCODE
            //}).Select(a => new DateDiffInvoice()
            //{
            //    INVOICENO = a.Key.INVOICENO,
            //    ADRESS1 = a.Key.ADRESS1,
            //    ADRESS2 = a.Key.ADRESS2,
            //    CITY = a.Key.CITY,
            //    ORDERNO = a.Key.ORDERNO,
            //    TOWN = a.Key.TOWN,
            //    SHIPPINGADRESSCODE = a.Key.SHIPPINGADRESSCODE,
            //    SHIPPINGNAME = a.Key.SHIPPINGNAME,
            //    TELNRS1 = a.Key.TELNRS1,
            //    PROJECTCODE = a.Key.PROJECTCODE,
            //    COMPANYCODE = CompanyCode
            //}).ToList();

            return invoices;
        }

        public Order GetOrderDetail(int id)
        {
            var _orderDetail = apiRepo.GetItem<Order>($"order/get-order-detail/{id}", "", Endpoints.PMS).Result;
            return _orderDetail;
        }

        public List<Order> GetOrders(int companyid, KendoFilterModel kendoFilterModel, out int total)
        {

            // server side company order list
            var orders = apiRepo.Post($"order/get-orders/{companyid}", kendoFilterModel,
                "", Endpoints.PMS).Result;

            var list = JsonConvert.DeserializeObject<OrderDataModel>(orders.JsonContent);
            // server side company order total
            //total = apiRepo.GetItem<int>($"order/get-order-count/{companyid}",
            //    "", Endpoints.PMS).Result;
            total = list.total;

            return list.datas;
        }

        public List<OrderTrackingNumberDto> GetOrderTrackingNumbers(int companyid, KendoFilterModel kendoFilterModel, out int total)
        {
            List<OrderTrackingNumberDto> list = new List<OrderTrackingNumberDto>();
            // server side company order list
            var trackingNumbers = apiRepo.Post($"order/get-tracking-numbers/{companyid}", kendoFilterModel,
               "", Endpoints.PMS).Result;


            var modelList = JsonConvert.DeserializeObject<OrderTrackingNumberDtoModel>(trackingNumbers.JsonContent);

            total = modelList.total;
            list = modelList.datas;
            return list;
        }
        public string PostOrder(Order order, string path)
        {
            string apiResponseMessage = string.Empty;
            try
            {
                var result = apiRepo.Post($"{path}", order, "", Endpoints.PMS).Result;
                apiResponseMessage = result.JsonContent;
            }
            catch (System.Exception ex)
            {
                apiResponseMessage = ex.Message;
            }
            return apiResponseMessage;
        }

        public void PostOrderList(IList<ExcelOrderViewModel> orderList, int storeId, int claimId, out string errorList)
        {
            var path = "order/save-exorder";
            StringBuilder strBuilder = new StringBuilder();
            // prepare order data
            var preparedProducts = PrepareOrders(orderList, storeId, claimId, out errorList);

            if (errorList != string.Empty)
                return;

            var oLenght = preparedProducts.Count;
            for (int i = 0; i < oLenght; i++)
            {
                try
                {
                    PostOrder(preparedProducts[i], path);
                    strBuilder.AppendLine($"{preparedProducts[i].OrderNo} Posted Succesfully !" + Environment.NewLine);
                }
                catch (Exception ex)
                {
                    strBuilder.AppendLine(ex.Message + Environment.NewLine);
                }
            }
            strBuilder.AppendLine("Excel Posted");

            errorList = strBuilder.ToString();
        }

        public List<Order> PrepareOrders(IList<ExcelOrderViewModel> orderList, int storeId, int claimId, out string errorList)
        {
            errorList = string.Empty;

            List<Order> _orderList = new List<Order>();

            var now = DateTime.Now;

            var store = storeService.StoreById(storeId);

            var logoCompanyCode = erpCompanyService.ErpCompanyById(store.ErpCompanyId ?? 0);

            if (logoCompanyCode != null)
            {
                //var storeProducts = storeService.StoreProducts(storeId);

                orderList.ToList().ForEach(x => x.OrderNumber = x.OrderNumber.Trim());
                var oList = orderList;

                var orders = oList.GroupBy(x => new
                {
                    x.ProjectCode,
                    x.OrderNumber,
                    x.OrderDate,
                    x.District,
                    x.Address,
                    x.CustomerName,
                    x.Phone1,
                    x.TaxNumber,
                    x.TaxOffice,
                    x.CustomerCode,
                    x.City,
                    x.EMailAddress,
                    x.ShippingAddress1,
                    x.ShippingName,
                    x.CargoBarcode
                });

                foreach (var item in orders)
                {
                    var key = item.Key;

                    Order order = new Order();

                    order.BillingAddressName = key.CustomerName;
                    order.ShippingAddressName = key.ShippingName;
                    order.ShippingAddress = key.ShippingAddress1;
                    order.ShippingTelNo1 = key.Phone1;
                    order.BillingTelNo1 = key.Phone1;
                    order.BillingAddress = key.Address;
                    order.BillingCity = key.City;
                    order.BillingEmail = key.EMailAddress;
                    order.ProjectCode = store.ProjectCode; // key.ProjectCode;
                    order.OrderDate = Convert.ToDateTime(key.OrderDate);
                    order.BillingTaxNumber = key.TaxNumber;
                    order.BillingTaxOffice = key.TaxOffice;
                    order.BillingCity = key.City;
                    order.Explanation1 = key.CustomerCode;
                    order.Explanation2 = key.CustomerName;
                    order.Explanation3 = key.CargoBarcode;
                    order.ShippingCity = key.City;
                    order.OrderNo = key.OrderNumber;
                    order.CreateDate = now;
                    order.CreatedBy = claimId;
                    order.isExternal = true;
                    // logo company code
                    order.LogoCompanyCode = logoCompanyCode.FirmNo;

                    order.OrderItems = new List<OrderItem>();
                    // order lines
                    foreach (var groupedItem in item.ToList())
                    {
                        OrderItem orderItem = new OrderItem();
                        // control from db
                        //var flexibleValues = storeProducts.FirstOrDefault(x => x.Product.Sku == groupedItem.ProductCode && x.IsFixed == true);
                        //var pointFixed = storeProducts.FirstOrDefault(x => x.Product.Sku == groupedItem.ProductCode && x.IsFixedPoint == true);

                        var product = productService.ProductBySku(groupedItem.ProductCode, isActive: false);
                        if (product != null && product.VatRate != null)
                        {
                            // Quantity
                            orderItem.Quantity = Convert.ToInt32(groupedItem.Quantity);
                            // Create Date
                            orderItem.CreateDate = now;
                            // Claim
                            orderItem.CreatedBy = claimId;
                            // SKU
                            orderItem.SKU = groupedItem.ProductCode;
                            // Price
                            orderItem.Price = Convert.ToDecimal(groupedItem.Price);
                            // Currency
                            orderItem.Currency = groupedItem.CurrencyCost;
                            // Vat
                            orderItem.VAT = product.VatRate.Value;
                            // if desi not defined default item weight
                            orderItem.Desi = Convert.ToInt32(product.Desi ?? int.Parse(groupedItem.Weight));
                            // if product name not defined default item product name
                            orderItem.ProductName = product.Name;

                            order.OrderItems.Add(orderItem);
                        }
                        else
                        {
                            errorList += $"{groupedItem.ProductCode} SKU Tanımlı Değil ya da KDV Girilmemiş Kontrol Ediniz.";
                        }
                    }

                    _orderList.Add(order);
                }
            }


            return _orderList;
        }

        public string SaveOrders(List<Order> selectedOrders, string path, int companyId)
        {
            //var path = $"order/save-exorder-to-logo/{companyId}";
            StringBuilder strBuilder = new StringBuilder();

            foreach (var order in selectedOrders)
            {
                try
                {
                    var apiResult = PostOrder(order, path);
                    strBuilder.AppendLine(apiResult);
                }
                catch (Exception ex)
                {
                    strBuilder.AppendLine($"{order.OrderNo} Error Exception : " + ex.Message);
                }
            }

            return strBuilder.ToString();
        }

        public bool UpdateTrackingNumbers(int companyId)
        {
            // server side
            try
            {
                var result = apiRepo.Post($"order/update-tracking-numbers/{companyId}", "",
                        "", Endpoints.PMS).Result;

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }
    }
}
