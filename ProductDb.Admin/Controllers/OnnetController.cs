using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ProductDb.Data.OnnetDb;
using ProductDb.Services.OnnetServices;

namespace ProductDb.Admin.Controllers
{
    [Route("onnet")]
    public class OnnetController : Controller
    {
        private IOnnetService _onnetService;

        public OnnetController(IOnnetService onnetService)
        {
            _onnetService = onnetService;
        }

        [HttpPost]
        [Route("get-onnet-productdb-stocks/{projectCode}")]
        public IEnumerable<OnnetProduct> GetOnnetProducts([FromBody] List<OnnetProduct> SKUs,string projectCode)
        {
            List<string> SKUs_ = new List<string>();

            foreach (var item in SKUs)
                SKUs_.Add(item.LogoCode);

            var datas = _onnetService.GetOnnetProducts(projectCode, SKUs_);

            return datas;
        }
    }
}