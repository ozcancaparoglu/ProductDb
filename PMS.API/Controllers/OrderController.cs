using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PMS.API.Models;
using PMS.Common.Dto;
using PMS.Common.Helpers;
using PMS.Data.Entities;
using PMS.Data.Repository;
using PMS.LogoService;
using PMS.LogoService.Helper;
using PMS.LogoService.LogoModels;
using PMS.LogoService.LogoService;
using PMS.Mapping;
using PMS.Service.LoggingService;
using PMS.Service.OrderServices;
using PMS.Service.ProjectServices;
using System;
//using PMS.Data.Models;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace PMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IProjectService _projectService;
        private readonly ILoggingService _loggingService;
        private readonly ILogoService _logoService;
        private readonly IOrderService _orderService;
        private readonly IDbLoggingService _dbLoggingService;

        public OrderController(IOrderService orderService,
                               ILogoService logoService,
                               ILoggingService loggingService,
                               IProjectService projectService,
                               IDbLoggingService dbLoggingService)
        {
            _projectService = projectService;
            _loggingService = loggingService;
            _logoService = logoService;
            _orderService = orderService;
            _dbLoggingService = dbLoggingService;
        }

        [HttpPost]
        [Route("save-order/{companyid}")]
        public IActionResult SaveOrder(int companyid, [FromBody] OrderModel Order)
        {
            DateTime now = DateTime.Now;
            try
            {
                var _logoCompany = _logoService.isLogoCompany(companyid);
                if (_logoCompany == null)
                    return BadRequest("Logo Company is not valid");

                var response = _logoService.isProjectDefinedInLogo(Order);
                if (!response.Status)
                    return BadRequest(response.Message);

                var bgOrder = _orderService.Add(Order);
                var LogoOrder = _logoService.InsertOrder(bgOrder, companyid, _logoCompany);

                if (LogoOrder.Status)
                {
                    _orderService.ChangeOrderStatus(Convert.ToInt32(Order.Id), true);
                    return Ok(LogoOrder);
                }
                else
                {
                    _dbLoggingService.InsertLog(new LogModel() { CompanyId = companyid, Message = LogoOrder.Message, CreateDate = now, EntityKey = " Logo Error", EntityId = 0, CreatedBy = 0 });
                    return BadRequest("Error Message : " + LogoOrder.Message);
                }
            }
            catch (System.Exception ex)
            {
                _dbLoggingService.InsertLog(new LogModel() { CompanyId = companyid, Message = ex.Message, CreateDate = now, EntityKey = "Exception Error", EntityId = 0, CreatedBy = 0 });
                return BadRequest(ex.Message.ToString());
            }
        }

        [HttpPost]
        [Route("save-exorder-to-logo/{companyid}")]
        public IActionResult SaveExOrder(int companyid, [FromBody] OrderModel Order)
        {
            DateTime now = DateTime.Now;
            try
            {
                var _logoCompany = _logoService.isLogoCompany(companyid);
                if (_logoCompany == null)
                    return BadRequest("Logo Company is not valid");
                // get ex order from list
                var data = _orderService.GetOrderFromExOrder(Convert.ToInt32(Order.Id));
                Order.ProjectCode = data.ProjectCode;
                // response
                var response = _logoService.isProjectDefinedInLogo(Order);
                // logo project control
                if (!response.Status)
                    return BadRequest(response.Message);
                // Ex Order Status
                var status = _orderService.ExOrderSendingStatus(_orderService.OrderModel(data));
                if (status)
                    return BadRequest("This Order Sended To Logo Earlier ! Please Check Order List");
                // Ex From Order
                var order = _orderService.IsExOrderAdd(_orderService.OrderModel(data));
                // after change exorder status
                if (order.Id != 0)
                    _orderService.ChangeExOrderStatus(Convert.ToInt32(Order.Id), true);
                //  order send to logo
                var LogoOrder = _logoService.InsertOrder(order, companyid, _logoCompany);
                if (LogoOrder.Status)
                {
                    return Ok("Order Posted Succesfully!");
                }
                else
                {
                    _dbLoggingService.InsertLog(new LogModel() { CompanyId = companyid, Message = LogoOrder.Message, CreateDate = now, EntityKey = "ExOrder Logo Error", EntityId = 0, CreatedBy = 0 });
                    return BadRequest("Error Message : " + LogoOrder.Message);
                }

            }
            catch (System.Exception ex)
            {
                _dbLoggingService.InsertLog(new LogModel() { CompanyId = companyid, Message = ex.Message, CreateDate = now, EntityKey = "ExOrder Exception Error", EntityId = 0, CreatedBy = 0 });
                return BadRequest(ex.Message.ToString());
            }
        }



        [HttpPost]
        [Route("save-exorder")]
        public IActionResult SaveOrder([FromBody] ExOrderModel Order)
        {
            try
            {
                // logo company setting default 215
                var bgOrder = _orderService.AddExOrder(Order);
                return Ok();
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message.ToString());
            }
        }


        [HttpGet]
        [Route("get-order-by-code/{Code}")]
        public async Task<IActionResult> GetOrder(string Code)
        {
            var data = await _orderService.GetOrderByProjeCode(Code);
            return Ok(data);
        }

        // new 

        [HttpGet]
        [Route("get-order-detail/{id}")]
        public IActionResult GetOrderDetailById(long id)
        {
            try
            {
                var order = _orderService.GetOrderById(id);
                return Ok(order);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message.ToString());
            }
        }

        [HttpGet]
        [Route("get-order-count/{companyId}")]
        public IActionResult GetOrderCount(int companyId)
        {
            var data = _orderService.Total(companyId);
            return Ok(data);
        }

        [HttpPost]
        [Route("get-orders/{companyId}")]
        public IActionResult GetOrders(KendoFilterDto kendoFilterDto, int companyId)
        {
            int total = 0;
            var datas = KendoFilterHelperDto.FilterQueryableData(_orderService.AllQueryableOrders(companyId), kendoFilterDto);
            total = datas.Count();

            datas = datas.Skip(kendoFilterDto.skip).Take(kendoFilterDto.take);

            return Ok(new { datas = datas, total = total });
        }

        // for external orders
        [HttpGet]
        [Route("get-exorder-detail/{id}")]
        public IActionResult GetExOrderDetailById(long id)
        {
            try
            {
                var order = _orderService.GetExOrderById(id);
                return Ok(order);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message.ToString());
            }
        }

        [HttpGet]
        [Route("get-exorder-count/{companyId}")]
        public IActionResult GetExOrderCount(int companyId)
        {
            var data = _orderService.ExTotal(companyId);
            return Ok(data);
        }

        [HttpPost]
        [Route("get-exorders/{companyId}")]
        public IActionResult GetExOrders(KendoFilterDto kendoFilterDto, int companyId)
        {
            int total = 0;
            var datas = KendoFilterHelperDto.FilterQueryableData(_orderService.AllQueryableExOrders(companyId), kendoFilterDto);
            total = datas.Count();
            datas = datas.Skip(kendoFilterDto.skip).Take(kendoFilterDto.take);

            return Ok(new { datas = datas, total = total });
        }

        [HttpGet]
        [Route("delete-exorder/{id}")]
        public IActionResult DeleteExOrder(int id)
        {
            try
            {
                _orderService.DeleteExOrder(id, true);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }




        [HttpGet]
        [Route("get-order-project-list/{companyid}")]
        public async Task<IActionResult> GetProjectList(int companyid)
        {
            var list = await _projectService.GetProjectsAsyncByCompanyId(companyid);
            return Ok(list);
        }

        [HttpPost]
        [Route("post-order-project")]
        public IActionResult PostProject([FromBody] ProjectModel project)
        {
            try
            {
                var result = _projectService.AddProject(project);
                return Ok(result);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message.ToString());
            }
        }

        [HttpPost]
        [Route("update-order-project")]
        public IActionResult EditProject([FromBody] ProjectModel project)
        {
            try
            {
                var result = _projectService.UpdateProject(project);
                return Ok(result);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message.ToString());
            }
        }


        [HttpGet]
        [Route("get-order-list/{companyid}")]
        public async Task<IActionResult> GetOrderListByCompanyId(int companyid)
        {
            var orders = await _orderService.AllOrdersByCompanyId(companyid);
            return Ok(orders);
        }



        // Logo Department And Division list
        [HttpGet]
        [Route("get-department-list/{companyid}")]
        public async Task<IActionResult> GetDepartmentList(int companyid)
        {
            var list = await _logoService.ExecuteQuery(LogoHelper.GetCapiDeptQuery(companyid));

            var resultData = XmlSerializerHelper.ParseXMLToObject<DepartmentModel>
                           (StringCompressor.UnzipBase64(list.resultXML.ToString()),
                                "/RESULTXML/RESULTLINE").Where(a => a.NAME != string.Empty);
            return Ok(resultData);
        }

        [HttpGet]
        [Route("get-division-list/{companyid}")]
        public async Task<IActionResult> GetDivisionList(int companyid)
        {
            var list = await _logoService.ExecuteQuery(LogoHelper.GetCapiDivQuery(companyid));

            var resultData = XmlSerializerHelper.ParseXMLToObject<DivisionModel>
                           (StringCompressor.UnzipBase64(list.resultXML.ToString()),
                           "/RESULTXML/RESULTLINE").Where(a => a.NAME != string.Empty);

            return Ok(resultData);
        }
        // tracking numbers
        [HttpPost]
        [Route("update-tracking-numbers/{companyid}/{periodId?}")]
        public async Task<IActionResult> UpdateTrackingNumbers(int companyid, string periodId = "01")
        {
            try
            {
                await _logoService.UpdateTrackingNumbers(companyid, periodId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("get-tracking-numbers-by-orderNo/{companyid}/{periodId?}")]
        public IActionResult GetTrackingNumberByOrderNOs([FromBody] List<string> OrderNOs, int companyid, string periodId = "01")
        {
            try
            {
                var orders = _orderService.GetOrderTrackingNumbers(OrderNOs, companyid);
                return Ok(orders);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("get-tracking-numbers/{companyid}/{periodId?}")]
        public IActionResult GetTrackingNumbers(KendoFilterDto kendoFilterDto, int companyid, string periodId = "01")
        {
            try
            {
                int total = 0;
                var datas = KendoFilterHelperDto.FilterQueryableData(_orderService.AllQueryableTrackingNumbers(companyid),
                   kendoFilterDto);
                total = datas.Count();

                datas = datas.Skip(kendoFilterDto.skip).Take(kendoFilterDto.take);

                return Ok(new { datas = datas, total = total });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("get-tracking-numbers-count/{companyId}")]
        public IActionResult GetTrackingNumbersCount(int companyId)
        {
            var data = _orderService.TrackingNumberTotal(companyId);
            return Ok(data);
        }

        [HttpPut]
        [Route("update-tracking-number-isshipping/{Id}")]
        public IActionResult UpdateTrackingNumberIsShipping(int Id)
        {
            _orderService.UpdateTrackingNumberIsShipping(Id);
            return Ok();
        }

        [HttpGet]
        [Route("get-order-by-orderno/{orderNo}")]
        public IActionResult GetOrderbyOrderNo(string orderNo)
        {
            var data=_orderService.OrderModelbyOrderNo(orderNo);
            return Ok(data);
        }

        //private readonly BGTRANSFER_V2Context _context;

        //public OrderController(BGTRANSFER_V2Context context)
        //{
        //    _context = context;


        // GET: api/Order
        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<Siparis>>> GetSiparis()
        //{
        //    return await _context.Siparis.ToListAsync();
        //}

        // GET: api/Order/5
        //[HttpGet("{id}")]
        //public async Task<ActionResult<Siparis>> GetSiparis(int id)
        //{
        //    var siparis = await _context.Siparis.FindAsync(id);

        //    if (siparis == null)
        //    {
        //        return NotFound();
        //    }

        //    return siparis;
        //}

        //[HttpGet]
        //[Route("GetSiparisDetailBySipNo/{sipNo}")]
        //public IActionResult GetSiparisDetailBySipNo(string sipNo)
        //{
        //    var siparis = _context.Siparis.FirstOrDefault(x => x.SipNo == sipNo);

        //    if (siparis == null)
        //    {
        //        return NotFound();
        //    }

        //    ReturnedOrderDetailDto detail = new ReturnedOrderDetailDto()
        //    {
        //        ReturnedOrderBillingDetail = new ReturnedOrderBillingDetailDto()
        //        {
        //            Address = siparis.FtAdres.EmptyIfNullOrWhiteSpace(),
        //            CitizenshipNumber = siparis.FtTc.EmptyIfNullOrWhiteSpace(),
        //            City = siparis.FtIl.EmptyIfNullOrWhiteSpace(),
        //            Company = siparis.FtFirma.EmptyIfNullOrWhiteSpace(),
        //            Country = siparis.FtUlke.EmptyIfNullOrWhiteSpace(),
        //            District = siparis.FtIlce.EmptyIfNullOrWhiteSpace(),
        //            Email = siparis.FtEmail.EmptyIfNullOrWhiteSpace(),
        //            Name = siparis.FtName.EmptyIfNullOrWhiteSpace(),
        //            Phone1 = siparis.FtTel1.EmptyIfNullOrWhiteSpace(),
        //            Phone2 = siparis.FtTel2.EmptyIfNullOrWhiteSpace(),
        //            Phone3 = siparis.FtTel3.EmptyIfNullOrWhiteSpace(),
        //            PostalCode = siparis.FtPkod.EmptyIfNullOrWhiteSpace(),
        //            TaxNumber = siparis.FtVno.EmptyIfNullOrWhiteSpace(),
        //            TaxOffice = siparis.FtVdaire.EmptyIfNullOrWhiteSpace()
        //        },
        //        ReturnedOrderShippingDetail = new ReturnedOrderShippingDetailDto()
        //        {
        //            Address = siparis.SvkAdres.EmptyIfNullOrWhiteSpace(),
        //            PostalCode = siparis.SvkPkod.EmptyIfNullOrWhiteSpace(),
        //            City = siparis.SvkIl.EmptyIfNullOrWhiteSpace(),
        //            Country = siparis.SvkUlke.EmptyIfNullOrWhiteSpace(),
        //            District = siparis.SvkIlce.EmptyIfNullOrWhiteSpace(),
        //            Email = siparis.SvkEmail.EmptyIfNullOrWhiteSpace(),
        //            Name = siparis.SvkName.EmptyIfNullOrWhiteSpace(),
        //            Phone1 = siparis.SvkTel1.EmptyIfNullOrWhiteSpace(),
        //            Phone2 = siparis.SvkTel2.EmptyIfNullOrWhiteSpace(),
        //            Phone3 = siparis.SvkTel3.EmptyIfNullOrWhiteSpace()
        //        }
        //    };

        //    return Ok(detail);
        //}

        // PUT: api/Order/5
        //[HttpPut("{id}")]
        //public async Task<IActionResult> PutSiparis(int id, Siparis siparis)
        //{
        //    if (id != siparis.Id)
        //    {
        //        return BadRequest();
        //    }

        //    _context.Entry(siparis).State = EntityState.Modified;

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!SiparisExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return NoContent();
        //}



        // DELETE: api/Order/5
        //[HttpDelete("{id}")]
        //public async Task<ActionResult<Siparis>> DeleteSiparis(int id)
        //{
        //    var siparis = await _context.Siparis.FindAsync(id);
        //    if (siparis == null)
        //    {
        //        return NotFound();
        //    }

        //    _context.Siparis.Remove(siparis);
        //    await _context.SaveChangesAsync();

        //    return siparis;
        //}

        //private bool SiparisExists(int id)
        //{
        //    return _context.Siparis.Any(e => e.Id == id);
        //}
    }
}
