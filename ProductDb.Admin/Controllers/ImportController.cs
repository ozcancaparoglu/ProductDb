using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ProductDb.Admin.Helpers.LocalizationHelpers;
using ProductDb.Admin.PageModels.Import;
using ProductDb.Admin.PageModels.Store;
using ProductDb.Mapping.BiggBrandDbModels.ImportModels;
using ProductDb.Services.CurrencyServices;
using ProductDb.Services.ImportServices;
using ProductDb.Services.LanguageServices;
using ProductDb.Services.StoreServices;
using ProductDb.Services.UploadService;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ProductDb.Admin.Controllers
{
    [MiddlewareFilter(typeof(LocalizationPipeline))]
    [Route("{lang}/import")]
    [Authorize]
    public class ImportController : Controller
    {
        private readonly IStoreService storeService;
        private readonly ILanguageService languageService;
        private readonly IUploadService uploadService;
        private readonly IExcelService excelService;
        private readonly ICurrencyService currencyService;


        public ImportController(IExcelService excelService,
                                IUploadService uploadService,
                                ILanguageService languageService,
                                IStoreService storeService,
                                ICurrencyService currencyService)
        {
            this.storeService = storeService;
            this.languageService = languageService;
            this.uploadService = uploadService;
            this.excelService = excelService;
            this.currencyService = currencyService;
        }

        [Route("excelproductimport")]
        [HttpGet]
        public IActionResult ExcelDocument()
        {
            var languages = languageService.AllLanguagesWithDefault().ToList();

            return View(new ExcelImportViewModel
            {
                languages = languages,
                Currencies = currencyService.AllActiveCurrencies().ToList(),
                message = string.Empty
            });
        }

        [Route("excelproductimport")]
        [HttpPost]
        public IActionResult ExcelDocument(ExcelImportViewModel viewModel)
        {
            var errorList =  new StringBuilder();
            var isValid = excelService.isValidDocument(Path.GetExtension(viewModel.file.FileName));

            if (isValid)
            {
                bool isCompleted = uploadService.isUploadExcelDocumentCompleted(viewModel.file, uploadService.GetUplaodedPath(viewModel.file.FileName));
                if (isCompleted)
                {
                    excelService.UpdateProductModel(excelService.ReadProductModel(uploadService.GetUplaodedPath(viewModel.file.FileName)),
                        viewModel.languageId.Value, out errorList);

                    viewModel.message += "File Uploaded Successfully" + Environment.NewLine;
                }
                else
                    viewModel.message += "File Not Uploaded" + Environment.NewLine;
            }
            else
            {
                viewModel.message += "Invalid Document Type" + Environment.NewLine;
            }

            if (errorList.Length > 0)
                viewModel.message += errorList.ToString();

            viewModel.languages = languageService.AllLanguagesWithDefault().ToList();
            viewModel.languageId = viewModel.languageId;
            viewModel.Currencies = currencyService.AllActiveCurrencies().ToList();

            return View(viewModel);
        }

        [Route("downloadexceltemplate")]
        [HttpGet]
        public IActionResult DownloadExcelTemplate()
        {
            byte[] excelTemplate = System.IO.File.ReadAllBytes(excelService.DownloadExcelTemplateUrl());
            string fileName = "ExcelTemplate.xlsx";
            return File(excelTemplate, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }

        [Route("excelnodelListImport")]
        [HttpGet]
        public IActionResult ExcelNodelListImport()
        {
            var viewModel = new StoreNodeSynchronizationViewModel
            {
                message = string.Empty,
            };
            return View(viewModel);
        }

        [Route("excelnodelListImport")]
        [HttpPost]
        public IActionResult ExcelNodelListImport(StoreNodeSynchronizationViewModel viewModel)
        {
            var errorCodes = new  List<string>();
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }
            else
            {
                try
                {
                    var isValid = excelService.isValidDocument(Path.GetExtension(viewModel.file.FileName));
                    if (!isValid)
                    {
                        viewModel.message = "File is not Valid";
                        return View(viewModel);
                    }

                    bool isCompleted = uploadService.isUploadExcelDocumentCompleted(viewModel.file, uploadService.GetUplaodedPath(viewModel.file.FileName));
                    if (!isCompleted)
                    {
                        viewModel.message = "File Not Uploaded";
                        return View(viewModel);
                    }

                    excelService.UpdateStoreNodes(excelService.ReadSynchronizationNodeModel(uploadService.GetUplaodedPath(viewModel.file.FileName)), out errorCodes);
                    viewModel.message = "File Upload Successfully!";
                    if (errorCodes.Count > 0)
                    {
                        viewModel.message += "Error Codes : " + JsonConvert.SerializeObject(errorCodes);
                    }
                }
                catch (Exception ex)
                {
                    viewModel.message = ex.Message + Environment.NewLine + "Error Json : " + JsonConvert.SerializeObject(errorCodes);
                }

                return View(viewModel);
            }
        }



        [Route("ProductFixingImport")]
        [HttpPost]
        public IActionResult ProductFixingImport(StoreProductViewModel viewModel)
        {
            //  Get Store List
            viewModel.Stores = storeService.AllActiveStores().ToList();
            viewModel.StoreId = viewModel.StoreId;

            var errorCodes = new List<string>();
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }
            else
            {
                try
                {
                    if (viewModel.file == null)
                    {
                        viewModel.message = "File is not selected";
                        return View("../Store/ProductFixing", viewModel);
                    }
                    var isValid = excelService.isValidDocument(Path.GetExtension(viewModel.file.FileName));
                    if (!isValid)
                    {
                        viewModel.message = "File is not Valid";
                        return View("../Store/ProductFixing", viewModel);
                    }

                    bool isCompleted = uploadService.isUploadExcelDocumentCompleted(viewModel.file, uploadService.GetUplaodedPath(viewModel.file.FileName));
                    if (!isCompleted)
                    {
                        viewModel.message = "File Not Uploaded";
                        return View("../Store/ProductFixing",viewModel);
                    }

                    excelService.UpdateStoreProductFixing(excelService.ReadStoreProductFixing(uploadService.GetUplaodedPath(viewModel.file.FileName)), out errorCodes);
                    viewModel.message = "File Upload Successfully!";
                    viewModel.isUploaded = true;
                    if (errorCodes.Count > 0)
                    {
                        viewModel.message += "Error Codes : " + JsonConvert.SerializeObject(errorCodes);
                    }
                }
                catch (Exception ex)
                {
                    viewModel.isUploaded = false;
                    viewModel.message = ex.Message + Environment.NewLine + "Error Json : " + JsonConvert.SerializeObject(errorCodes);
                }

                return View("../Store/ProductFixing",viewModel);
            }
        }

    }
}