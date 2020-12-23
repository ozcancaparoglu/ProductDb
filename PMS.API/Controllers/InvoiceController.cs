using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PMS.Data.Entities.Invoice;
using PMS.Data.Entities.Order;
using PMS.LogoService;
using PMS.LogoService.Helper;
using PMS.LogoService.LogoModels.XMLModels;
using PMS.LogoService.LogoService;
using PMS.Service.ItemService;
using PMS.Service.ProjectServices;

namespace PMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvoiceController : ControllerBase
    {
        private readonly IItemService _itemService;
        private readonly IProjectService _projectService;
        private readonly ILogoService _logoService;

        public InvoiceController(ILogoService logoService,
                                 IProjectService projectService,
                                 IItemService itemService)
        {
            this._itemService = itemService;
            this._projectService = projectService;
            this._logoService = logoService;
        }

        [Route("get-datediff-invoices/{companyid}/{periodid?}")]
        public async Task<IActionResult> GetDateDiffInvoices(int companyid, string periodid = "01")
        {
            var result = await _logoService.ExecuteQuery(LogoHelper.GetDateDiffInvoiceQuery(companyid, periodid));

            ICollection<DateDiffInvoice> resultData = new List<DateDiffInvoice>();

            if (result.resultXML.ToString() != string.Empty)
            {
                resultData = XmlSerializerHelper.ParseXMLToObject<DateDiffInvoice>
                            (StringCompressor.UnzipBase64(result.resultXML.ToString()), "/RESULTXML/RESULTLINE");
            }

            var projectList = await _projectService.GetProjectsAsync();

            resultData = resultData.Where(a => projectList.Any(x => x.ProjectCode ==
            a.PROJECTCODE && x.PriceDifference == true)).ToList();

            return Ok(resultData);
        }

        [Route("post-datediff-invoices/{companyid}/{periodid?}")]
        public async Task<IActionResult> PostDateDiffInvoices(List<DateDiffInvoice> dateDiffInvoices, int companyid, string periodid = "01")
        {
            try
            {
                var dateDiff = dateDiffInvoices.FirstOrDefault();
                var projectCode = dateDiffInvoices.FirstOrDefault().PROJECTCODE;
                var project = _projectService.GetProjects().FirstOrDefault(a => a.ProjectCode == projectCode);

                var Date = DateTime.Now;

                var data = new Order()
                {
                    BillingTelNo1 = dateDiff.TELNRS1,
                    OrderNo = dateDiff.SHIPPINGADRESSCODE,
                    BillingAddressName = dateDiff.SHIPPINGNAME,
                    BillingTown = dateDiff.TOWN,
                    BillingCity = dateDiff.CITY,
                    BillingAddress = dateDiff.ADRESS1 + " " + dateDiff.ADRESS2,
                    BillingEmail = dateDiff.EMAILADDR,
                    BillingCountry = dateDiff.COUNTRYCODE,
                    BillingTaxNumber = dateDiff.TAXNR,
                    BillingTaxOffice = dateDiff.TAXOFFICE,
                    ProjectCode = projectCode,
                    ShippingAddress = dateDiff.ADRESS1 + " " + dateDiff.ADRESS2,
                    ShipmentCode = dateDiff.SHIPPINGADRESSCODE,
                    ShippingAddressName = dateDiff.SHIPPINGNAME,
                    ShippingCity = dateDiff.CITY,
                    ShippingEmail = dateDiff.EMAILADDR,
                    ShippingTown = dateDiff.TOWN,
                    ShippingCountry = dateDiff.COUNTRY
                };

                // customer inserted
                var checkingAccount = _logoService.CreateCheckingAccount(data, companyid);
                // customer shipment inserted
                var shipmentResult = _logoService.CreateShipmentAccount(data, companyid);

                var invoiceModel = new Invoice();

                invoiceModel.Address1 = dateDiff.ADRESS1;
                invoiceModel.Address2 = dateDiff.ADRESS2;
                invoiceModel.City = dateDiff.CITY;
                invoiceModel.CreateDate = Date;
                invoiceModel.CreatedBy = 1;
                invoiceModel.ErrorMessage = "";
                invoiceModel.InvoiceNo = dateDiff.INVOICENO;
                invoiceModel.OrderNo = dateDiff.ORDERNO;
                invoiceModel.ProjectCode = projectCode;
                invoiceModel.TaxNo = dateDiff.TAXNR;
                invoiceModel.TaxOffice = dateDiff.TAXOFFICE;
                invoiceModel.ShipmentCode = dateDiff.SHIPPINGADRESSCODE;
                invoiceModel.ShipmentDefination = dateDiff.SHIPPINGNAME;
                invoiceModel.Telephone1 = dateDiff.TELNRS1;
                invoiceModel.Telephone2 = dateDiff.TELNRS2;
                invoiceModel.isActive = true;
                invoiceModel.IsTransferred = true;
                invoiceModel.Town = dateDiff.TOWN;
                invoiceModel.InvoiceDate = Convert.ToDateTime(dateDiff.INVOICEDATE).ToShortDateString();
                invoiceModel.InvoiceItems = new List<InvoiceItem>();
                invoiceModel.Genexctyp = Convert.ToInt32(dateDiff.GENEXCTYP);
                invoiceModel.Lineexctyp = Convert.ToInt32(dateDiff.LINEEXCTYP);
                invoiceModel.isEINVOICE = checkingAccount.isGIBUSER;

                var itemVatRates = _itemService.ItemVateRateByRate();

                foreach (var item in dateDiffInvoices)
                {
                    var MasterCode = itemVatRates.FirstOrDefault(a => a.VateRate == Convert.ToInt32(item.VATRATE));
                    var price = LogoHelper.FormattedPrice(item.PRICE);
                   
                    var vatRate = Convert.ToDecimal(item.VATRATE);

                    price = project.TaxIncluded ? (price / (1 + vatRate / 100)) : price;

                    InvoiceItem iModel = new InvoiceItem();

                    iModel.CreateDate = Date;
                    iModel.Currency = "TL";
                    iModel.Price = price;
                    iModel.Quantity = 1;
                    iModel.VAT = vatRate;
                    iModel.SKU = MasterCode == null ? string.Empty : MasterCode.ItemCode;

                    invoiceModel.InvoiceItems.Add(iModel);
                }
                // invoice inserted
                var invoiceResult = await _logoService.InsertInvoice(invoiceModel, companyid);
                // delivery code updated
                var orderUpdateResult = await _logoService.DirectQuery(LogoHelper.
                                        GetUpdateORFCDeliveryCodeQuery(companyid, periodid,
                                        dateDiffInvoices.FirstOrDefault().LINELOGICALREF));

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message.ToString());
            }

        }
    }
}