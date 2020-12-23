
//using PMS.LogoService.Helper;
//using LogoObjectClient;
//using PMS.LogoService.Helper;
using LogoObjectClient;
using Newtonsoft.Json;
using PMS.Common;
using PMS.Data.Entities.Logo;
using PMS.Data.Entities.Order;
using PMS.Data.Entities.Statics;
using PMS.Data.Repository;
using PMS.LogoService.Helper;
using PMS.LogoService.LogoModels;
using PMS.Mapping;
using PMS.Mapping.AutoMapperConfiguration;
using PMS.Service.LoggingService;
using PMS.Service.OrderServices;
using PMS.Service.ProjectServices;
using ProductDb.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Invoice = PMS.Data.Entities.Invoice.Invoice;

namespace PMS.LogoService.LogoService
{
    public class LogoService : ILogoService
    {
        private readonly IRepository<OrderTrackingNumber> _orderTrackingNumberRepo;
        private readonly IDbLoggingService _dbLoggingService;
        private readonly ILogoInvService _logoInvService;
        private readonly IRepository<Invoice> _invoiceRepository;
        private readonly IAutoMapperService _autoMapperService;
        private readonly IRepository<Order> _orderRepository;
        private readonly IRepository<ExOrder> _exOrderRepository;
        private readonly IRepository<LogoCompany> _companyRepository;
        private readonly IOrderService _orderService;
        private readonly ILoggingService _loggingService;
        private readonly IProjectService _projectService;
        private readonly ILogoServiceClient _logoServerClient;

        SvcClient SvClient = null;

        public LogoService(IProjectService projectService,
                           ILoggingService loggingService,
                           IOrderService orderService,
                           IRepository<LogoCompany> companyRepository,
                           ILogoServiceClient logoServerClient,
                           IRepository<Order> orderRepository,
                           IRepository<Invoice> invoiceRepository,
                           IAutoMapperService autoMapperService,
                           ILogoInvService logoInvService, IDbLoggingService dbLoggingService,
                           IRepository<ExOrder> exOrderRepository,
                           IRepository<OrderTrackingNumber> orderTrackingNumberRepo)
        {
            _orderTrackingNumberRepo = orderTrackingNumberRepo;
            _dbLoggingService = dbLoggingService;
            _logoInvService = logoInvService;
            _invoiceRepository = invoiceRepository;
            _autoMapperService = autoMapperService;
            _orderRepository = orderRepository;
            _exOrderRepository = exOrderRepository;
            _companyRepository = companyRepository;
            _orderService = orderService;
            _loggingService = loggingService;
            _projectService = projectService;
            _logoServerClient = logoServerClient;

            SvClient = LogoServiceClient.SvcClient;
            //if (ProjectModels == null)
            //ProjectModels = _projectService.GetProjects().ToList();
        }

        #region Convert
        /// <summary>
        ///  Convert Order To Logo Order Model
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public LogoOrderModel ConvertOrderToLogoOrder(OrderModel model, LogoCompanyModel logoCompanySettings)
        {
            //var project = ProjectModels.FirstOrDefault(a => a.ProjectCode == model.ProjectCode);
            var project = _projectService.GetProjectByCode(model.ProjectCode);
            //var auxil_code = string.Empty;
            //if (model.OrderNo.Contains("-TG"))
            //    auxil_code = "TEKRAR GÖN";
            var now = LogoHelper.LogoDateFormatter(DateTime.Now);

            var Currencies = CurrencyHelper.GetCurrencies();
            //Anıl 19/03/2019
            //Kargo Takip numarası gelirse kullanılan alan, TEXTFLDS9'a yazılacak
            //var kargoTakipNumarasi = string.Empty;
            //if (!string.IsNullOrWhiteSpace(model.Explanation2) && model.Explanation2.Contains("KTN-"))
            //{
            //    kargoTakipNumarasi = model.Explanation2.Remove(0, 4);
            //    model.Explanation2 = string.Empty;
            //}
            string AuxilCode = GetAuxilCode(model, project);

            OrderSlip orderSlip = new OrderSlip();

            orderSlip.NUMBER = model.OrderNo;
            orderSlip.DOC_NUMBER = model.OrderNo;
            orderSlip.DOC_TRACK_NR = model.OrderDate.ToString();
            orderSlip.ARP_CODE = project.CheckingAccountToBeCreated ? model.OrderNo : project.CheckingAccountCode;
            orderSlip.SHIPLOC_CODE = model.OrderNo;
            orderSlip.FACTORY = project.Factory;
            orderSlip.DIVISION = project.Division;
            orderSlip.DEPARTMENT = project.Departmant;
            orderSlip.PROJECT_CODE = project.ProjectCode;
            orderSlip.NOTES1 = "";
            orderSlip.NOTES2 = "";
            orderSlip.NOTES3 = "";
            orderSlip.NOTES4 = "";
            orderSlip.DATE = now[0]; // now.ToShortDateString();
            orderSlip.TIME = (Convert.ToInt32(now[4]) + (Convert.ToInt32(now[3]) * 256) + (Convert.ToInt32(now[2]) * 65536) + (Convert.ToInt32(now[1]) * 16777216));
            //Statics
            orderSlip.CURRSEL_TOTAL = "1";
            orderSlip.CURRSEL_DETAILS = "4";
            // Yüksel & Yonca & Anıl -- Dinamik ve sabit proje ayrımı için yapıldı  
            orderSlip.AUXIL_CODE = AuxilCode;
            orderSlip.ORDER_STATUS = "1";
            orderSlip.UPD_CURR = "1";
            orderSlip.UPD_TRCURR = "1";
            orderSlip.SOURCE_WH = project.Warehouse;


            var LogoOrderModel = new LogoOrderModel();
            var LogoOrderTransaction = new LogoOrderTransactionsModel();

            LogoOrderModel.ORDER_SLIP = orderSlip;
            LogoOrderModel.ORDER_SLIP.TRANSACTIONS = LogoOrderTransaction;

            List<LogoOrderTransactionModel> transactionModels =
                new List<LogoOrderTransactionModel>();

            foreach (var item in model.OrderItems)
            {
                LogoOrderTransactionModel orderLine = new LogoOrderTransactionModel();

                var siparisUrunCurrencyType = string.Empty;
                var siparisUrunCurrencyRate = string.Empty;

                decimal price = 0;
                decimal currencyAmount = 0;

                siparisUrunCurrencyType = CurrencyHelper.GetSiparisUrunCurrencyType(item.Currency);

                if (item.Currency == project.DefaultCurrency)
                    siparisUrunCurrencyRate = "1";
                else
                    siparisUrunCurrencyRate = CurrencyHelper.GetCurrency(Currencies, item.Currency);

                //if (item.Currency == "TL")
                //    siparisUrunCurrencyRate = "1";
                //else
                //    siparisUrunCurrencyRate = CurrencyHelper.GetCurrency(Currencies, item.Currency);

                currencyAmount = decimal.Parse(siparisUrunCurrencyRate, CultureInfo.CreateSpecificCulture("en-UK"));
                price = item.Price * currencyAmount;

                orderLine.TYPE = 0;
                orderLine.MASTER_CODE = item.SKU;
                orderLine.AUXIL_CODE = item.CatalogCode;
                orderLine.QUANTITY = item.Quantity;
                orderLine.DUE_DATE = project.DueDateDifference > 0 ? (Convert.ToDateTime(now).AddDays(project.DueDateDifference)).ToShortDateString() : string.Empty;
                orderLine.PRICE = price.ToString("#.##").Replace(",", ".");
                orderLine.UNIT_CODE = logoCompanySettings.Setting1 ?? "ADET";
                orderLine.PC_PRICE = CultureInfo.CurrentCulture.Name != "tr-TR" ? item.Price.ToString("#.##") :
                                    item.Price.ToString("#.##").Replace(",", "."); /*item.Price.ToString("#.##").Replace(",", ".");*/
                orderLine.PROJECT_CODE = project.ProjectCode;
                orderLine.VAT_RATE = (int)item.VAT;
                orderLine.SOURCE_WH = project.Warehouse;
                orderLine.VAT_INCLUDED = project.TaxIncluded ? 1 : 0;
                orderLine.CURR_PRICE = siparisUrunCurrencyType;
                orderLine.PR_RATE = CultureInfo.CurrentCulture.Name != "tr-TR" ? currencyAmount.ToString("#.####") :
                                    currencyAmount.ToString("#.####").Replace(",", "."); /*currencyAmount.ToString("#.####").Replace(",", ".");*/
                orderLine.CURR_TRANSACTIN = "160";
                orderLine.RC_XRATE = "";
                orderLine.EDT_CURR = siparisUrunCurrencyType;
                orderLine.EDT_PRICE = CultureInfo.CurrentCulture.Name != "tr-TR" ? currencyAmount.ToString("#.####") :
                                    currencyAmount.ToString("#.####").Replace(",", ".");

                orderLine.DEFNFLDS = new LogoOrderTransactionDefinedLines();
                orderLine.DEFNFLDS.DEFNFLD = new List<LogoOrderTransactionDefinedLine>();

                LogoOrderTransactionDefinedLine dfLine = new LogoOrderTransactionDefinedLine();

                dfLine.MODULENR = 8;
                dfLine.LEVEL_ = 1;
                dfLine.NUMFLDS1 = item.Points;
                dfLine.NUMFLDS2 = item.PointsValue;
                dfLine.NUMFLDS3 = item.Desi;
                // Kargo Price
                dfLine.NUMFLDS4 = item.CargoPrice ?? 0;
                dfLine.NUMFLDS8 = item.Points;
                dfLine.NUMFLDS10 = item.Consumable ?? 0;
                dfLine.TEXTFLDS5 = model.Explanation1;
                dfLine.TEXTFLDS6 = model.Explanation2;
                dfLine.TEXTFLDS7 = model.Explanation3;
                dfLine.TEXTFLDS8 = !string.IsNullOrEmpty(item.CargoCurrency) ? item.CargoCurrency : " ";
                dfLine.TEXTFLDS11 = !string.IsNullOrEmpty(item.ConsumableCurrency) ? item.ConsumableCurrency : " ";
                dfLine.TEXTFLDS12 = model.Explanation3;

                orderLine.DEFNFLDS.DEFNFLD.Add(dfLine);

                if (item.Points > 0 && project.PointIsOrderDiscount == true)
                {
                    decimal discount_rate = 0;
                    var PuanTutar = item.PointsValue;
                    try
                    {
                        discount_rate = ((PuanTutar / item.Price) / item.Quantity) * 100;
                    }
                    catch (Exception e)
                    {
                        discount_rate = 0;
                    }
                    if (discount_rate > 100)
                    {
                        discount_rate = 100;
                    }
                    orderLine.TYPE = 2;
                    orderLine.MASTER_CODE = "";
                    orderLine.DISCOUNT_RATE = CultureInfo.CurrentCulture.Name != "tr-TR" ? discount_rate.ToString("#.##") :
                                    discount_rate.ToString("#.##").Replace(",", "."); /*discount_rate.ToString("#.##").Replace(",", "."); // discount_rate; // static*/
                    orderLine.SOURCE_WH = project.Warehouse;
                }

                transactionModels.Add(orderLine);
            }

            LogoOrderModel.ORDER_SLIP.TRANSACTIONS.TRANSACTION = transactionModels;
            return LogoOrderModel;
        }
        public LogoMarksModel ConvertMarkToLogoMark(LogoMarksModel markModel)
        {
            // Convert Logo If Necessary
            return markModel;
        }
        public LogoInvoiceModel ConvertOrderToLogoDiffInvoice(InvoiceModel model)
        {
            //var project = ProjectModels.FirstOrDefault(a => a.ProjectCode == model.ProjectCode);
            var project = _projectService.GetProjectByCode(model.ProjectCode);
            var now = LogoHelper.LogoDateFormatter(DateTime.Now);

            PMS.LogoService.LogoModels.Invoice invoiceSlip = new PMS.LogoService.LogoModels.Invoice();

            invoiceSlip.TYPE = 9;
            invoiceSlip.NUMBER = "~";
            //invoiceSlip.NUMBER = "~";
            invoiceSlip.DOC_NUMBER = $"FF {model.OrderNo}";
            invoiceSlip.DATE = now[0]; // now.ToShortDateString();
            invoiceSlip.TIME = (Convert.ToInt32(now[4]) + (Convert.ToInt32(now[3]) * 256) + (Convert.ToInt32(now[2]) * 65536) + (Convert.ToInt32(now[1]) * 16777216));
            //(now.Millisecond + (now.Second * 256) + (now.Minute * 65536) + (now.Hour * 16777216));
            invoiceSlip.ARP_CODE = model.ShipmentCode;
            invoiceSlip.SOURCE_WH = "15";
            invoiceSlip.NOTES1 = $"{LogoHelper.LogoDateFormatter(Convert.ToDateTime(model.InvoiceDate))[0]} {model.InvoiceNo} NOLU FAT.İST";
            invoiceSlip.NOTES2 = "FİYAT FARKI";
            invoiceSlip.SOURCE_COST_GRP = "15";
            invoiceSlip.DIVISION = project.Division;
            invoiceSlip.DEPARTMENT = project.Departmant;
            invoiceSlip.DOC_DATE = now[0];
            invoiceSlip.CURR_INVOICE = 0;
            invoiceSlip.CURRSEL_TOTALS = model.Genexctyp;
            invoiceSlip.CURRSEL_DETAILS = model.Lineexctyp;
            invoiceSlip.EINVOICE = model.isEINVOICE == true ? "1" : "2";
            // Gönderim Şekli
            invoiceSlip.EARCHIVEDETR_SENDMOD = 1;


            var LogoInvoiceModel = new LogoInvoiceModel();
            var LogoInvoiceTransaction = new LogoInvoiceTransactionsModel();
            var PaymentList = new LogoInvoicePaymentsModel();

            LogoInvoiceModel.INVOICE = invoiceSlip;
            LogoInvoiceModel.INVOICE.TRANSACTIONS = LogoInvoiceTransaction;
            LogoInvoiceModel.INVOICE.PAYMENT_LIST = PaymentList;

            List<LogoInvoiceTransactionModel> transactionModels =
                new List<LogoInvoiceTransactionModel>();

            List<LogoInvoicePaymentModel> paymentModels =
                new List<LogoInvoicePaymentModel>();

            foreach (var item in model.InvoiceItems)
            {

                LogoInvoiceTransactionModel invoiceLine = new LogoInvoiceTransactionModel();

                invoiceLine.TYPE = 4;
                invoiceLine.MASTER_CODE = item.SKU;
                invoiceLine.QUANTITY = item.Quantity;
                invoiceLine.PRICE = CultureInfo.CurrentCulture.Name != "tr-TR" ? item.Price.ToString("#.##") :
                                    item.Price.ToString("#.##").Replace(",", ".");

                invoiceLine.UNIT_CODE = "ADET";
                invoiceLine.VAT_RATE = (int)item.VAT;
                invoiceLine.SOURCECOSTGRP = 0;
                invoiceLine.SOURCEINDEX = 0;
                invoiceLine.MASTER_DEF = "FİYAT FARKI";

                transactionModels.Add(invoiceLine);
            }

            // Logo Fatura istisna durumu yönetimi
            var lines = transactionModels.Any(a => a.VAT_RATE == 0);
            if (lines)
            {
                invoiceSlip.VATEXCEPT_CODE = "350";
                invoiceSlip.EINVOICE_TYPE = 2;
            }

            LogoInvoicePaymentModel payment = new LogoInvoicePaymentModel();

            payment.DATE = now[0];
            payment.PROCDATE = now[0];

            payment.TOTAL = CultureInfo.CurrentCulture.Name != "tr-TR" ? model.InvoiceItems.Sum(a => a.Price).ToString("#.##")
                            : model.InvoiceItems.Sum(a => a.Price).ToString("#.##").Replace(",", ".");

            payment.TRCODE = "9";
            payment.MODULENR = "4";

            paymentModels.Add(payment);

            LogoInvoiceModel.INVOICE.TRANSACTIONS.TRANSACTION = transactionModels;
            LogoInvoiceModel.INVOICE.PAYMENT_LIST.PAYMENT = paymentModels;

            return LogoInvoiceModel;
        }
        public LogoItemModel ConvertItemToLogoItem(LogoItemModel item)
        {
            // Convert Operations If is necessary
            return item;
        }
        public LogoCustomerModel ConvertOrderCustomerToLogoCustomer(OrderModel model, int companyId = 215)
        {
            LogoCustomerModel logoCustomerModel = new LogoCustomerModel();

            CustomerArAp customerArAp = new CustomerArAp();

            var nameParsed = GetNameSurnameParsed(model.BillingAddressName);

            customerArAp.ACCOUNT_TYPE = "3";
            customerArAp.CODE = model.OrderNo;
            customerArAp.TITLE = model.BillingAddressName;
            customerArAp.AUTH_CODE = "GENEL";


            customerArAp.NAME = nameParsed[0];
            customerArAp.SURNAME = nameParsed[1];

            customerArAp.TELEPHONE1 = model.BillingTelNo1;

            var addressParsed = GetAddressParsed(model.BillingAddress);

            customerArAp.ADDRESS1 = addressParsed[0];
            customerArAp.ADDRESS2 = addressParsed[1];
            customerArAp.TOWN = model.BillingTown;
            customerArAp.CITY = model.BillingCity;
            customerArAp.COUNTRY_CODE = LogoHelper.GetCountryCode(model.BillingCountry);
            customerArAp.COUNTRY = model.BillingCountry;
            customerArAp.GL_CODE = LogoHelper.GL_CODE;

            int countTcNo;
            int countVNo;
            if (model.BillingTaxNumber == null) { model.BillingTaxNumber = ""; }
            if (model.BillingIdentityNumber == null) { model.BillingIdentityNumber = ""; }
            countVNo = model.BillingTaxNumber.Count(); // *** null exception
            countTcNo = model.BillingIdentityNumber.Count();// *** null exception
            string tcNo = "11111111111";
            string vNo = "1111111111";
            int persCompany = 1;
            vNo = model.BillingTaxNumber;

            if (model.BillingTaxNumber != "" && countVNo == 10)
            {
                tcNo = model.BillingIdentityNumber;
                persCompany = 0;
                customerArAp.PERSCOMPANY = persCompany;
                customerArAp.TAX_ID = vNo;
                customerArAp.TAX_OFFICE = model.BillingTaxOffice;
            }

            if (model.BillingIdentityNumber != "" && countVNo == 11)
            {
                tcNo = model.BillingTaxNumber;
                customerArAp.TCKNO = tcNo;
                persCompany = 1;
            }

            if (persCompany == 1)
            {
                customerArAp.TCKNO = tcNo;
            }
            else
            {
                customerArAp.TAX_ID = vNo;
                customerArAp.TAX_OFFICE = model.BillingTaxNumber;
            }

            int ACCEPT_EINV = 0;
            if (vNo != string.Empty)
            {
                // Login GIB And Control Customer VKN
                var GIBLOGIN = _logoInvService.ConnectService($"{companyId}").Result;
                ACCEPT_EINV = _logoInvService.GetValidateGIBUser(GIBLOGIN.sessionID,
                    new string[] { $"VKN={vNo}" }).Result
                                    .outputList[0] == "ISGIBUSER=1" ? 1 : 0;
            }

            // GIB CONTROL FLAG
            customerArAp.ACCEPT_EINV = ACCEPT_EINV;

            customerArAp.PERSCOMPANY = persCompany;
            customerArAp.E_MAIL = model.BillingEmail;
            customerArAp.PURCHBRWS = 1;
            customerArAp.SALESBRWS = 1;
            customerArAp.IMPBRWS = 1;
            customerArAp.EXPBRWS = 1;
            customerArAp.FINBRWS = 1;
            logoCustomerModel.AR_AP = customerArAp;

            return logoCustomerModel;
        }
        public LogoCustomerShippingModel ConvertLogoCustomerShipping(OrderModel model)
        {
            //var project = ProjectModels.FirstOrDefault(a => a.ProjectCode == model.ProjectCode);
            var project = _projectService.GetProjectByCode(model.ProjectCode);
            LogoCustomerShippingModel customerShippingModel = new LogoCustomerShippingModel();
            ShipmentLoc shipmentLoc = new ShipmentLoc();

            if (project.CheckingAccountToBeCreated)
                shipmentLoc.ARP_CODE = model.OrderNo;
            else
            {
                //shipmentLoc.ARP_CODE = model.OrderNo;
                // tEKRAR AÇILACAK
                shipmentLoc.ARP_CODE = project.CheckingAccountCode;
            }


            shipmentLoc.CODE = model.OrderNo;
            shipmentLoc.DESCRIPTION = model.ShippingAddressName;
            shipmentLoc.TITLE = model.ShippingAddressName;
            shipmentLoc.TELEPHONE1 = model.ShippingTelNo1;
            shipmentLoc.TELEPHONE2 = !string.IsNullOrEmpty(model.ShippingTelNo2) ? model.ShippingTelNo2 : string.Empty;

            var addressParsed = GetAddressParsed(model.ShippingAddress);
            shipmentLoc.EMAIL_ADDR = model.ShippingEmail;
            shipmentLoc.POSTAL_CODE = model.ShippingPostalCode;
            shipmentLoc.ADDRESS1 = addressParsed[0];
            shipmentLoc.ADDRESS2 = addressParsed[1];
            shipmentLoc.TOWN = model.ShippingTown;
            shipmentLoc.CITY = model.ShippingCity;
            shipmentLoc.COUNTRY_CODE = LogoHelper.GetCountryCode(model.CountryCode);
            shipmentLoc.COUNTRY = model.ShippingCountry;

            customerShippingModel.ShipmentLoc = shipmentLoc;

            return customerShippingModel;
        }
        #endregion
        private static string GetAuxilCode(OrderModel model, ProjectModel project)
        {
            var AuxileCode = string.Empty;

            if (model.OrderNo.Contains("-TG"))
            {
                AuxileCode = "TEKRAR GÖN";
            }
            else
            {
                // Yüksel & Yonca & Anıl -- Dinamik ve sabit proje ayrımı için yapıldı   
                if (!string.IsNullOrEmpty(project.GroupCode))
                    AuxileCode = project.GroupCode;                                                                                               //order.DataFields.FieldByName("AUTH_CODE").Value = siparis.Project.GrupKodu;
                else
                    AuxileCode = project.GroupCode = "0";
            }

            return AuxileCode;
        }
        public static List<string> GetNameSurnameParsed(string nameSurname)
        {
            List<string> newList = new List<string>();
            if (nameSurname == null)
            {
                newList.Add("");
                newList.Add("");
                return newList;
            }

            char[] spaceSeparator = new char[] { ' ' };
            int lengthAddress = nameSurname.Length;
            var splittedNameSurname = nameSurname.Split(spaceSeparator);
            string name1 = "";
            string name2 = "";
            switch (splittedNameSurname.Length)
            {
                case 0:
                    break;
                case 1:
                    name1 = splittedNameSurname[0];
                    name2 = "";
                    break;
                case 2:
                    name1 = splittedNameSurname[0];
                    name2 = splittedNameSurname[1];
                    break;
                case 3:
                    name1 = splittedNameSurname[0] + " " + splittedNameSurname[1];
                    name2 = splittedNameSurname[2];
                    break;
                case 4:
                    name1 = splittedNameSurname[0] + " " + splittedNameSurname[1] + " " + splittedNameSurname[2];
                    name2 = splittedNameSurname[3];
                    break;
                default:
                    name1 = splittedNameSurname[0];
                    name2 = splittedNameSurname[1];
                    break;
            }

            try
            {
                newList.Add(name1.ToUpper());
                newList.Add(name2.ToUpper());

            }
            catch (Exception e)
            {
                return newList;
            }
            return newList;
        }
        public static List<string> GetAddressParsed(string address)
        {
            List<string> newList = new List<string>();
            if (address == null)
            {
                //return new List<string>();
                newList.Add("");
                newList.Add("");
                return newList;
            }

            char[] spaceSeparator = new char[] { ' ' };
            int lengthAddress = address.Length;
            var splittedAddress = address.Split(spaceSeparator);
            string adr1 = "";
            string adr2 = "";
            foreach (string str in splittedAddress)
            {
                if (adr1.Length < lengthAddress / 2)
                {
                    adr1 = adr1 + " " + str;
                }
                else
                {
                    adr2 = adr2 + " " + str;
                }
            }

            try
            {
                newList.Add(adr1.ToUpper());
                newList.Add(adr2.ToUpper());
            }
            catch (Exception e)
            {
                return newList;
            }
            return newList;
        }
        public async Task<ExecQueryResponse> ExecuteQuery(string sqlText)
        {
            var req = _logoServerClient.CreateExecuteRequest(sqlText);
            return await SvClient.ExecQueryAsync(req);
        }
        public async Task<AppendDataObjectResponse> SendToLogo<T>(T t, LogoDataType dataType, int companyId)
            where T : class
        {
            var dataXML = XmlSerializerHelper.ParseToXML(t);
            var req = _logoServerClient.CreateAppendRequest(dataXML, dataType, companyId);

            return await SvClient.AppendDataObjectAsync(req);
        }
        public LogoResponseModel InsertOrder(Order order, int companyId, LogoCompanyModel logoCompanySettings)
        {
            //var Project = ProjectModels.FirstOrDefault(a => a.ProjectCode == order.ProjectCode);
            var Project = _projectService.GetProjectByCode(order.ProjectCode);
            var now = DateTime.Now;

            // Order Items Control From Logo
            var result = isItemDefinedInLogo(order, companyId);

            if (!result.Status)
            {
                _dbLoggingService.InsertLog(new LogModel
                {
                    CreateDate = now,
                    CreatedBy = 1,
                    EntityId = Convert.ToInt32(order.Id),
                    Message = result.Message,
                    UpdateDate = now,
                    EntityKey = EntityStatics.OrderKey,
                    UpdatedBy = 1,
                    CompanyId = companyId
                });
                return result;
            }

            // Customer Account Control Insert And Logger
            if (Project.CheckingAccountToBeCreated)
            {
                var accountResponse = CreateCheckingAccount(order, companyId);
                _dbLoggingService.InsertLog(new LogModel
                {
                    CreateDate = now,
                    CreatedBy = 1,
                    EntityId = Convert.ToInt32(order.Id),
                    EntityKey = EntityStatics.OrderCustomerAccountKey,
                    Message = JsonConvert.SerializeObject(accountResponse),
                    UpdateDate = now,
                    UpdatedBy = 1,
                    CompanyId = companyId
                });
            }

            // Shipping Account Control Insert And Logger
            var shippingAccountResponse = CreateShipmentAccount(order, companyId);
            _dbLoggingService.InsertLog(new LogModel
            {
                CreateDate = now,
                CreatedBy = 1,
                EntityId = Convert.ToInt32(order.Id),
                Message = JsonConvert.SerializeObject(shippingAccountResponse),
                UpdateDate = now,
                EntityKey = EntityStatics.OrderShipping,
                UpdatedBy = 1,
                CompanyId = companyId
            });

            // Order Insert And Logger
            var orderResponse = CreateOrder(order, companyId, logoCompanySettings);

            _dbLoggingService.InsertLog(new LogModel
            {
                CreateDate = now,
                CreatedBy = 1,
                EntityId = Convert.ToInt32(order.Id),
                Message = JsonConvert.SerializeObject(orderResponse),
                UpdateDate = now,
                EntityKey = EntityStatics.OrderKey,
                UpdatedBy = 1,
                CompanyId = companyId
            });

            return orderResponse;
        }
        public async Task<LogoResponseModel> InsertInvoice(Invoice invoice, int companyId)
        {
            _loggingService.LogInfo($"Invoice Number: {invoice.OrderNo} Transfer Started...");

            var LogoInvoice = ConvertOrderToLogoDiffInvoice(_autoMapperService.Map<Invoice, InvoiceModel>(invoice));

            var LogoResponse = await SendToLogo<LogoInvoiceModel>(LogoInvoice,
                LogoDataType.doSalesInvoice, companyId);

            var response = new LogoResponseModel()
            {
                Message = LogoResponse.errorString,
                Status = LogoResponse.status != (byte)LogoResponseEnum.Failed ? true : false,
            };

            _loggingService.LogInfo($"Invoice Number: {invoice.OrderNo} Transfer Message : {response.Message} Transfer Status : {response.Status}");


            return response;
        }
        private LogoResponseModel CreateOrder(Order order, int companyId, LogoCompanyModel logoCompanySettings)
        {
            _loggingService.LogInfo($"Order Card Number: {order.OrderNo} Transfer Started...");

            var LogoOrder = ConvertOrderToLogoOrder(_autoMapperService.Map<Order, OrderModel>(order), logoCompanySettings);

            var LogoResponse = SendToLogo<LogoOrderModel>(LogoOrder,
                LogoDataType.doSalesOrderSlip, companyId).Result;

            var response = new LogoResponseModel()
            {
                Message = LogoResponse.errorString == "" ? "Posted Successfully!" : LogoResponse.errorString,
                Status = LogoResponse.status != (byte)LogoResponseEnum.Failed ? true : false,
                LogicalRef = LogoResponse.dataReference
            };

            // recontrol logo status from error message
            if (!response.Status)
                response.Status = LogoHelper.LogoResponseStatus(response.Message);

            _loggingService.LogInfo($"Order Number: {order.OrderNo} Transfer Message : {response.Message} Transfer Status : {response.Status}");

            order.IsTransferred = response.Status;
            order.ErrorMessage = response.Message == string.Empty ? "OK" : response.Message;
            order.LogoTransferedId = response.LogicalRef;

            _orderRepository.Update(order);

            return response;
        }
        public LogoResponseModel CreateShipmentAccount(Order order, int companyId)
        {
            _loggingService.LogInfo($"Customer Card Number: {order.OrderNo} Transfer Started...");

            var LogoCustomerShpping = ConvertLogoCustomerShipping(_autoMapperService.Map<Order, OrderModel>(order));

            var LogoResponse = SendToLogo<LogoCustomerShippingModel>(LogoCustomerShpping,
                LogoDataType.doArpShipLic, companyId).Result;

            var response = new LogoResponseModel()
            {
                Message = LogoResponse.errorString,
                Status = LogoResponse.status != (byte)LogoResponseEnum.Failed ? true : false,
                LogicalRef = LogoResponse.dataReference
            };

            _loggingService.LogInfo($"Customer Number: {order.OrderNo} Transfer Message : {response.Message} Transfer Status : {response.Status}");

            //order.IsAccountShippingCreated = response.Status;
            //_orderRepository.Update(order);

            return response;
        }
        public LogoResponseModel CreateCheckingAccount(Order order, int companyId)
        {
            try
            {
                var _isDefinedInLogo = isDefinedInLogo(order, companyId).Result;
                if (_isDefinedInLogo.Status)
                    return _isDefinedInLogo;

                _loggingService.LogInfo($"Customer  Number: {order.OrderNo} Transfer Started...");

                var LogoCustomer = ConvertOrderCustomerToLogoCustomer(_autoMapperService.Map<Order, OrderModel>(order));

                var LogoResponse = SendToLogo<LogoCustomerModel>(LogoCustomer,
                    LogoDataType.doAccountsRP, companyId).Result;

                var response = new LogoResponseModel()
                {
                    Message = LogoResponse.errorString,
                    Status = LogoResponse.status != (byte)LogoResponseEnum.Failed ? true : false,
                    isGIBUSER = LogoCustomer.AR_AP.ACCEPT_EINV == 1 ? true : false,
                    LogicalRef = LogoResponse.dataReference
                };

                _loggingService.LogInfo($"Customer Number: {order.OrderNo} Transfer Message : {response.Message} Transfer Status : {response.Status}");

                //order.IsAccountCreated = response.Status;

                //_orderRepository.Update(order);

                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public LogoCompanyModel isLogoCompany(int CompanyId)
        {
            var logoCompany = _companyRepository.Table().FirstOrDefault(x => x.CompanyId == CompanyId);
            return _autoMapperService.Map<LogoCompany, LogoCompanyModel>(logoCompany);
            //return _companyRepository.Filter(a => a.CompanyId == CompanyId).Any();
        }
        public async Task<LogoResponseModel> isDefinedInLogo(Order order, int companyId)
        {
            _loggingService.LogInfo($"Checking Account: {order.OrderNo}");

            var result = await ExecuteQuery(LogoHelper.GetCheckingAccountQuery(companyId, order.OrderNo));

            var xml = (StringCompressor.UnzipBase64(result.resultXML.ToString()));

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);

            XmlNodeList nodeList = doc.SelectNodes("/RESULTXML/RESULTLINE/CODE");
            var nodeResult = int.Parse(nodeList.Cast<XmlNode>().FirstOrDefault().InnerText);

            string message = string.Empty;
            bool status = false;

            if (nodeResult == (int)LogoStatus.Contain)
            {
                _loggingService.LogInfo($"{order.OrderNo} Is Already Created: {order.OrderNo}");
                message = $"{order.OrderNo} Is Already Created: {order.OrderNo}";
                status = true;
            }

            return new LogoResponseModel()
            {
                Message = message,
                Status = status
            };
        }

        public async Task<DirectQueryResponse> DirectQuery(string sqlText)
        {
            var req = _logoServerClient.CreateDirectRequest(sqlText);
            return await SvClient.DirectQueryAsync(req);
        }

        public async Task<LogoResponseModel> InsertItemMark(LogoMarksModel markModel, int companyId)
        {
            var logoMark = ConvertMarkToLogoMark(markModel);

            var LogoResponse = await SendToLogo<LogoMarksModel>(logoMark,
                LogoDataType.doMark, companyId);

            var response = new LogoResponseModel()
            {
                Message = LogoResponse.errorString == string.Empty ? "Successfully !" : LogoResponse.errorString,
                Status = LogoResponse.status != (byte)LogoResponseEnum.Failed ? true : false,
                statusValue = LogoResponse.status
            };

            return response;
        }

        public async Task<LogoResponseModel> InsertItem(LogoItemModel itemCardModel, int companyId)
        {
            var logoItem = ConvertItemToLogoItem(itemCardModel);

            var LogoResponse = await SendToLogo<LogoItemModel>(logoItem,
                LogoDataType.doMaterial, companyId);

            var response = new LogoResponseModel()
            {
                Message = LogoResponse.errorString == string.Empty ? "Successfully !" : LogoResponse.errorString,
                Status = LogoResponse.status != (byte)LogoResponseEnum.Failed ? true : false,
                statusValue = LogoResponse.status
            };

            return response;
        }

        public LogoResponseModel isItemDefinedInLogo(Order model, int companyId)
        {
            _loggingService.LogInfo($"Checking Items For : {model.OrderNo}");
            // Order Item Control From Logo
            var iLenght = model.OrderItems.Count;
            var items = model.OrderItems.ToList();
            // invalid items
            StringBuilder invalidItems = new StringBuilder();

            for (int i = 0; i < iLenght; i++)
            {
                var result = ExecuteQuery(LogoHelper.ItemControl(companyId, items[i].SKU)).Result;
                var xml = (StringCompressor.UnzipBase64(result.resultXML.ToString()));
                if (xml == string.Empty)
                {
                    var item = new OrderItem
                    {
                        SKU = items[i].SKU,
                        ProductName = items[i].ProductName,
                        CreateDate = items[i].CreateDate,
                        Quantity = items[i].Quantity,
                        Price = items[i].Price,
                        VAT = items[i].VAT,
                        OrderId = items[i].OrderId
                    };

                    invalidItems.Append($"Invalid Item : " + JsonConvert.SerializeObject(item));
                }
            }
            var itemsLog = invalidItems.ToString();
            var response = new LogoResponseModel()
            {
                Message = itemsLog,
                Status = itemsLog != string.Empty ? false : true
            };
            // order message
            if (!response.Status)
            {
                model.IsTransferred = response.Status;
                model.ErrorMessage = response.Message;

                _orderRepository.Update(model);
                // update ex order
            }

            return response;
        }

        public LogoResponseModel isProjectDefinedInLogo(OrderModel model)
        {
            //var data = ProjectModels.FirstOrDefault(x => x.ProjectCode == model.ProjectCode);
            var data = _projectService.GetProjectByCode(model.ProjectCode);
            return new LogoResponseModel()
            {
                Status = data != null ? true : false,
                Message = data != null ? string.Empty : "Project Not Defined ! Before Sending To Logo Make Sure Project Defined."
            };
        }

        public OrderTrackingNumberModel GetOrderTrackingNumbersBySku(List<string> SKUs, int companyId, string periodid)
        {
            var formattedSkus = string.Format("'{0}'", string.Join("','", SKUs.Select(i => i.Replace("'", "''"))));

            return new OrderTrackingNumberModel();
        }

        public async Task UpdateTrackingNumbers(int companyId, string periodId = "01")
        {
            var trackingNumbers = await PrepareOrderTrackingNumber(companyId, periodId);
            foreach (var item in trackingNumbers)
            {
                var tNumber = _orderTrackingNumberRepo.Table().FirstOrDefault(x => x.OrderNo == item.OrderNo && x.Code == item.Code);
                if (tNumber == null)
                    _orderTrackingNumberRepo.Add(_autoMapperService.Map<OrderTrackingNumberModel, OrderTrackingNumber>(item));
            }
        }

        public async Task<List<OrderTrackingNumberModel>> PrepareOrderTrackingNumber(int companyId, string periodId = "01")
        {
            var now = DateTime.Now;
            List<OrderTrackingNumberModel> resultData = new List<OrderTrackingNumberModel>();

            var exceptedList = _orderRepository.Table().Where(x => x.LogoCompanyCode == companyId).Select(x => x.OrderNo).
                               Except(_orderTrackingNumberRepo.Table().Where(x => x.CompanyId == companyId).Select(x => x.OrderNo)).ToList();

            List<string> SKUs = new List<string>();

            foreach (var orderNo in exceptedList)
                SKUs.Add(orderNo);

            var formattedSkus = string.Format("'{0}'", string.Join("','", SKUs.Select(i => i.Replace("'", "''"))));
            var sqlQuery = LogoHelper.GetOrderTrackingNumberQuery(companyId, periodId, formattedSkus);

            var list = await ExecuteQuery(sqlQuery);

            if (list.resultXML.ToString() != string.Empty)
            {
                resultData = XmlSerializerHelper.ParseXMLToObject<OrderTrackingNumberModel>
                           (StringCompressor.UnzipBase64(list.resultXML.ToString()),
                           "/RESULTXML/RESULTLINE").ToList();

                resultData.ForEach(x => x.CompanyId = companyId);
                resultData.ForEach(x => x.CreateDate = now);
            }

            return resultData;
        }
    }
}
