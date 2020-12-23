using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PMS.Common;
using PMS.Common.Dto;
using ProductDb.Admin.Areas.PMS.Services;
using ProductDb.Admin.Areas.PMS.ViewModels;
using ProductDb.Admin.PageModels.Filter;
using ProductDb.Common.Cache;
using ProductDb.Services.ErpComanyService;
using ProductDb.Services.ImportServices;
using ProductDb.Services.PermissionServices;
using ProductDb.Services.ProductServices;
using ProductDb.Services.StoreServices;
using ProductDb.Services.UploadService;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ProductDb.Admin.Areas.PMS.Controllers
{
    [Area("PMS")]
    [Route("PMS/order")]
    public class OrderController : BaseController
    {
        private readonly IOrderService orderService;
        private readonly IProductService productService;
        private readonly IStoreService storeService;
        private readonly IUploadService uploadService;
        private readonly IApiExcelService apiExcelService;
        private readonly IExcelService excelService;
        private readonly IErpCompanyService erpCompanyService;
        private readonly ICacheManager cacheManager;
        private readonly IApiRepo apiRepo;
        private readonly IUserRolePermissionService userRolePermissionService;
        public OrderController(IApiRepo apiRepo,
                               ICacheManager cacheManager,
                               IOrderService orderService,
                               IUserRolePermissionService userRolePermissionService, IErpCompanyService erpCompanyService,
                               IExcelService excelService,
                               IApiExcelService apiExcelService,
                               IUploadService uploadService,
                               IStoreService storeService,
                               IProductService productService) : base(userRolePermissionService)
        {
            this.productService = productService;
            this.storeService = storeService;
            this.uploadService = uploadService;
            this.apiExcelService = apiExcelService;
            this.excelService = excelService;
            this.erpCompanyService = erpCompanyService;
            this.cacheManager = cacheManager;
            this.orderService = orderService;
            this.apiRepo = apiRepo;
            this.userRolePermissionService = userRolePermissionService;
        }

        [Route("get-datediff-invoices/{companyid}/{periodid?}")]
        public async Task<IActionResult> GetDateDiffInvoices(int companyid, string periodid = "01", string message = "")
        {
            var groupedInvoices = await orderService.GetInvoices(companyid, periodid);
            ViewBag.Message = message;

            return View("Invoicelist", groupedInvoices);
        }

        [Route("get-datediff-invoice-detail/{companyid}/{logicalRef}/{periodid?}")]
        public async Task<IActionResult> GetDateDiffInvoiceDetail(int companyid, string logicalRef, string periodid = "01")
        {
            var invoices = await orderService.GetCachedValues<DateDiffInvoice>($"invoice/get-datediff-invoices/{companyid}/{periodid}",
            CacheStatics.InvoiceCacheKey, CacheStatics.InvoiceCacheTime);

            var invoiceDetails = invoices.Where(a => a.LINELOGICALREF == logicalRef).ToList();

            return View("InvoiceDetail", invoiceDetails);
        }

        [HttpPost]
        [Route("post-datediff-invoices/{companyid}/{logicalRef}/{periodid?}")]
        public async Task<IActionResult> PostDateDiffInvoices(int companyid, string logicalRef, string periodid = "01")
        {
            var invoices = await orderService.GetCachedValues<DateDiffInvoice>($"invoice/get-datediff-invoices/{companyid}/{periodid}",
              CacheStatics.InvoiceCacheKey, CacheStatics.InvoiceCacheTime);

            var diffInvoice = invoices.Where(a => a.LINELOGICALREF == logicalRef).ToList();

            var response = await apiRepo.Post($"invoice/post-datediff-invoices/{companyid}/{periodid}", diffInvoice,
                "", Endpoints.PMS);

            string Message = string.Empty;

            if (HTMLResponseTypes.Response(response.ResponseCode) == "Successful!")
            {
                Message = $"Logo Logical Ref : {logicalRef} Posted Successful!";
                invoices.Remove(invoices.FirstOrDefault(a => a.LINELOGICALREF == logicalRef));

                cacheManager.Remove(CacheStatics.InvoiceCacheKey);
                cacheManager.Set(CacheStatics.InvoiceCacheKey, invoices, CacheStatics.InvoiceCacheTime);
            }
            else
                Message = response.JsonContent;

            //cacheManager.Remove(CacheStatics.InvoiceCacheKey);

            return RedirectToAction("GetDateDiffInvoices", new { @companyid = companyid, @periodid = periodid, @message = Message });
        }

        [HttpGet]
        [Route("get-order-project-list/{companyid}")]
        public async Task<IActionResult> GetProjects(int companyid)
        {
            var projects = await apiRepo.GetList<Project>($"order/get-order-project-list/{companyid}", "", Endpoints.PMS);
            //var projects = await orderService.GetCachedValues<Project>($"order/get-order-project-list/{companyid}",
            //    CacheStatics.ProjectListCacheKey, CacheStatics.ProjectListCacheTime);

            var viewModel = new ProjectViewModel
            {
                firmNo = companyid,
                Projects = projects
            };

            return View("ProjectList", viewModel);
        }

        [HttpGet]
        [Route("create-project/{companyid}")]
        public IActionResult CreateProject(int companyid)
        {
            var Divisions = apiRepo.GetList<Division>($"order/get-division-list/{companyid}", "", Endpoints.PMS).Result;
            //var Divisions = orderService.GetCachedValues<Division>($"order/get-division-list/{companyid}",
            //    CacheStatics.DivisionCacheKey, CacheStatics.DivisionCacheTime).Result;
            var Departments = apiRepo.GetList<Department>($"order/get-department-list/{companyid}", "", Endpoints.PMS).Result;
            //var Departments = orderService.GetCachedValues<Department>($"order/get-department-list/{companyid}",
            //    CacheStatics.DepartmentCacheKey, CacheStatics.DepartmentCacheTime).Result;

            var project = new Project()
            {
                Divisions = Divisions,
                Departments = Departments,
                LogoFirmCode = $"{companyid}"
            };

            return View(project);
        }

        [HttpPost]
        [Route("create-project/{companyid}")]
        public IActionResult CreateProject(int companyid, Project project)
        {

            if (!ModelState.IsValid)
                return View(project);

            var result = apiRepo.Post<Project>($"order/post-order-project", project, "", Endpoints.PMS).Result;
            cacheManager.Remove(CacheStatics.ProjectListCacheKey);


            return RedirectToAction("GetProjects", new { companyid = companyid });
        }

        [HttpGet]
        [Route("edit-project/{companyid}/{ProjectId}")]
        public IActionResult EditProject(int companyid, int ProjectId)
        {
            var query = apiRepo.GetList<Project>($"order/get-order-project-list/{companyid}", "", Endpoints.PMS).Result;
            //var query = orderService.GetCachedValues<Project>($"order/get-order-project-list",
            //   CacheStatics.ProjectListCacheKey, CacheStatics.ProjectListCacheTime).Result;

            var data = query.FirstOrDefault(a => a.Id == ProjectId);

            data.Divisions = apiRepo.GetList<Division>($"order/get-division-list/{companyid}", "", Endpoints.PMS).Result;

            //data.Divisions = orderService.GetCachedValues<Division>($"order/get-division-list/{companyid}",
            //    CacheStatics.DivisionCacheKey, CacheStatics.DivisionCacheTime).Result;
            data.Departments = apiRepo.GetList<Department>($"order/get-department-list/{companyid}", "", Endpoints.PMS).Result;
            //data.Departments = orderService.GetCachedValues<Department>($"order/get-department-list/{companyid}",
            //  CacheStatics.DepartmentCacheKey, CacheStatics.DepartmentCacheTime).Result;

            data.LogoFirmCode = $"{companyid}";

            return View("EditProject", data);
        }

        [HttpPost]
        [Route("edit-project/{companyid}/{ProjectId}")]
        public async Task<IActionResult> EditProject(Project project, int companyid)
        {

            if (!ModelState.IsValid)
                return View("EditProject", project);

            var result = await apiRepo.Post<Project>($"order/update-order-project", project, "", Endpoints.PMS);
            cacheManager.Remove(CacheStatics.ProjectListCacheKey);

            return RedirectToAction("GetProjects", new { companyid = companyid });
        }

        [HttpGet]
        [Route("changestate-project/{companyid}/{ProjectId}")]
        public async Task<IActionResult> DeleteProject(int companyid, int ProjectId)
        {
            //var projects = await orderService.GetCachedValues<Project>($"order/get-order-project-list/{companyid}",
            //       CacheStatics.ProjectListCacheKey, CacheStatics.ProjectListCacheTime);
            var projects = apiRepo.GetList<Project>($"order/get-order-project-list/{companyid}", "", Endpoints.PMS).Result;
            var project = projects.FirstOrDefault(a => a.Id == ProjectId);
            project.isActive = !project.isActive;

            var result = await apiRepo.Post<Project>($"order/update-order-project", project, "", Endpoints.PMS);

            return RedirectToAction("GetProjects", new { companyid = companyid });
        }

        //[HttpGet]
        //[Route("get-order-list/{companyid}")]
        //public async Task<IActionResult> GetOrderlist(int companyid)
        //{
        //    var orders = await apiRepo.GetList<Order>($"order/get-order-list/{companyid}", "", Endpoints.PMS);
        //    return View("OrderList", orders);
        //}

        [HttpGet]
        [Route("get-order-detail/{id}")]
        public async Task<IActionResult> GetOrderDetailById(int id)
        {
            var order = await apiRepo.GetItem<Order>($"order/get-order-detail/{id}", "", Endpoints.PMS);

            return View("OrderDetail", order);
        }

        // new
        [Route("order-list")]
        public IActionResult OrderList()
        {
            var viewModel = new OrderViewModel
            {
                Firms = erpCompanyService.ErpCompanies().ToList()
            };
            return View(viewModel);
        }

        [Route("order-external-list")]
        public IActionResult OrderExternalList()
        {
            var viewModel = new OrderViewModel
            {
                Firms = erpCompanyService.ErpCompanies().ToList()
            };
            return View(viewModel);
        }


        [HttpPost]
        [Route("order-list/{companyId}")]
        public IActionResult GetOrderList(int companyId, KendoFilterModel kendoFilterModel)
        {
            try
            {
                var orders = orderService.GetOrders(companyId, kendoFilterModel, out int total);
                return Json(new { total, data = orders });
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("order-detail/{id}")]
        public IActionResult GetOrderDetail(int id)
        {
            var result = ViewComponent("OrderDetail", new { id = id });
            return result;
        }

        [Route("order-import")]
        public IActionResult OrderImport()
        {
            return View(new OrderImportViewModel()
            {
                Stores = storeService.AllActiveStores().Where(x => x.ProjectCode != null).ToList(),
                Store = new Mapping.BiggBrandDbModels.StoreModel()
            });
        }

        [Route("order-import")]
        [HttpPost]
        public IActionResult OrderImport(OrderImportViewModel orderImport)
        {
            orderImport.Stores = storeService.AllActiveStores().Where(x => x.ProjectCode != null).ToList();
            orderImport.Store = new Mapping.BiggBrandDbModels.StoreModel();

            string message = string.Empty;
            try
            {
                if (orderImport.StoreId == 0)
                {
                    ViewBag.message = "Store not selected";
                    return View(orderImport);
                }
                if (orderImport.formFile == null)
                {
                    ViewBag.message = "File is not selected";
                    return View(orderImport);
                }
                var isValid = excelService.isValidDocument(Path.GetExtension(orderImport.formFile.FileName));
                if (!isValid)
                {
                    ViewBag.message = "File is not valid";
                    return View(orderImport);
                }

                bool isCompleted = uploadService.isUploadExcelDocumentCompleted(orderImport.formFile, uploadService.GetUplaodedPath(orderImport.formFile.FileName));
                if (!isCompleted)
                {
                    ViewBag.message = "File Not Uploaded";
                    return View(orderImport);
                }
                else
                {
                    var orderList = apiExcelService.ReadOrders(uploadService.GetUplaodedPath(orderImport.formFile.FileName));
                    var claimId = int.Parse(User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Sid).Value);
                    orderService.PostOrderList(orderList, orderImport.StoreId, claimId, out string errorCodes);
                    ViewBag.message = errorCodes;
                }
            }
            catch (Exception ex)
            {
                ViewBag.message = ex.Message;
            }
            return View(orderImport);
        }

        [HttpPost]
        [Route("save-exorders/{companyId}")]
        public IActionResult SaveExOrders(List<Order> orders, int companyId)
        {
            try
            {
                var path = $"order/save-exorder-to-logo/{companyId}";
                string result = orderService.SaveOrders(orders, path, companyId);
                return Json(new { status = true, message = result });
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        [HttpPost]
        [Route("save-orders/{companyId}")]
        public IActionResult SaveOrders(List<Order> orders, int companyId)
        {
            try
            {
                var path = $"order/save-order/{companyId}";

                string result = orderService.SaveOrders(orders, path, companyId);
                return Json(new { status = true, message = result });
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        // for firm selection
        [Route("Firm-Selection")]
        public IActionResult FirmSelection()
        {
            var firmSelection = new ErpSelectionViewModel
            {
                ErpCompanies = erpCompanyService.ErpCompanies().ToList(),
                ErpCompanyModel = new Mapping.BiggBrandDbModels.ErpCompanyModel()
            };
            return View(firmSelection);
        }

        [Route("Firm-Selection")]
        [HttpPost]
        public IActionResult FirmSelection(ErpSelectionViewModel model)
        {
            return RedirectToAction("GetProjects", new { companyid = model.ErpCompanyNo });
        }

        // for external
        [Route("exorder-detail/{id}")]
        public IActionResult GetExOrderDetail(int id)
        {
            var result = ViewComponent("ExOrderDetail", new { id = id });
            return result;
        }

        [Route("exorder-list")]
        public IActionResult ExOrderList()
        {
            var viewModel = new OrderViewModel
            {
                Firms = erpCompanyService.ErpCompanies().ToList()
            };
            return View("ExOrderList", viewModel);
        }

        [HttpPost]
        [Route("exorder-list/{companyId}")]
        public IActionResult GetExOrderList(int companyId, KendoFilterModel kendoFilterModel)
        {
            try
            {
                var orders = orderService.GetExOrders(companyId, kendoFilterModel, out int total);
                return Json(new { total, data = orders });
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("delete-exorder/{id}")]
        public IActionResult DeleteExorder(int id)
        {
            try
            {
                orderService.DeleteExOrder(id);
                return Json(new { status = true, message = "Order Deleted" });
            }
            catch (Exception ex)
            {
                return Json(new { status = false, message = ex.Message });
            }
        }

        [HttpPost]
        [Route("PostSelectedItems")]
        public IActionResult PostSelectedItems(OrderDetailViewModel orderDetailView)
        {
            try
            {
                var checkedItems = orderDetailView.Items.Where(x => x.check);
                // control
                if (checkedItems.Count() == 0)
                {
                    orderDetailView.logoMessage = "Seçim Yapmadınız";
                }
                foreach (var item in checkedItems)
                {
                    var _product = productService.ProductBySku(item.SKU);
                    if (_product != null)
                    {
                        var firmCode = erpCompanyService.ErpCompanies().FirstOrDefault(x => x.FirmNo == orderDetailView.companyId);
                        var result = erpCompanyService.SaveProductToCompany(firmCode.Id, _product).Result;
                        orderDetailView.logoMessage += result;
                    }
                    else
                    {
                        orderDetailView.logoMessage += $"{item.SKU} Ürün Bulunamadı Kontrol Ediniz";
                    }
                }

                return Json(new { status = true, message = orderDetailView.logoMessage });
            }
            catch (Exception ex)
            {
                return Json(new { status = true, message = ex.Message });
            }
        }

        // tracking Numbers

        [Route("order-tracking-number-list")]
        public IActionResult OrderTrackingNumberList()
        {
            var viewModel = new OrderViewModel
            {
                Firms = erpCompanyService.ErpCompanies().ToList()
            };
            return View(viewModel);
        }

        [HttpPost]
        [Route("order-tracking-number-list-by-filter/{companyId}")]
        public IActionResult OrderTrackingNumberListByFilter(int companyId, KendoFilterModel kendoFilterModel)
        {
            try
            {
                var orders = orderService.GetOrderTrackingNumbers(companyId, kendoFilterModel, out int total);
                return Json(new { total, data = orders });
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("update-tracking-numbers/{companyId}")]
        public IActionResult UpdateTrackingNumber(int companyId)
        {
            try
            {
                var result = orderService.UpdateTrackingNumbers(companyId);
                return Json(new { status = result, message = "OK" });
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { status = false, message = ex.Message });
            }
        }
    }
}