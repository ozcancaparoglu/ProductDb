using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using ProductDb.Services.ImportServices;
using ProductDb.Services.ImportServices.XmlSupplierServices.SpxServices;
using System;

namespace ProductDb.Admin.Controllers
{
    [AllowAnonymous]
    [Route("xml")]
    public class XmlSupplierController : Controller
    {
        private const string successMsg = "Xml read is completed.";
        private readonly IConfiguration configuration;

        private readonly ISupplierService<SpxService> spxService;
        

        public XmlSupplierController(IConfiguration configuration, ISupplierService<SpxService> spxService)
        {
            this.configuration = configuration;
            this.spxService = spxService;
        }


        //[HttpPost]
        [Route("spx")]
        public ContentResult GetSpx()
        {
            try
            {
                spxService.InsertXmlToDb(
                    configuration.GetValue<int>("XmlUrls_StartNodes:SpxSupplierId"), 
                    configuration.GetValue<int>("XmlUrls_StartNodes:SpxRootCategoryId"), 
                    configuration.GetValue<string>("XmlUrls_StartNodes:SpxUrl"), 
                    configuration.GetValue<string>("XmlUrls_StartNodes:SpxStartNode"));
                return Content(successMsg);
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }
    }
}