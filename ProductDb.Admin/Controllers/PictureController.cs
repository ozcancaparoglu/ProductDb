using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using ProductDb.Admin.ConfigurationModels;
using ProductDb.Admin.Helpers.LocalizationHelpers;
using ProductDb.Admin.PageModels.Picture;
using ProductDb.Common.Extensions;
using ProductDb.Mapping.BiggBrandDbModels;
using ProductDb.Services.LanguageServices;
using ProductDb.Services.PermissionServices;
using ProductDb.Services.PictureServices;
using ProductDb.Services.ProductServices;
using System;
using System.IO;

namespace ProductDb.Admin.Controllers
{
    [MiddlewareFilter(typeof(LocalizationPipeline))]
    [Authorize]
    [Route("{lang}/picture")]
    public class PictureController : BaseController
    {
        private readonly IHostingEnvironment hostingEnvironment;
        private readonly IPictureService pictureService;
        private readonly IConfiguration configuration;
        private readonly IProductService productService;

        public PictureController(ILanguageService languageService, IUserRolePermissionService userRolePermissionService, IHostingEnvironment hostingEnvironment, IPictureService pictureService, IConfiguration configuration, IProductService productService) : base (languageService, userRolePermissionService)
        {
            this.hostingEnvironment = hostingEnvironment;
            this.pictureService = pictureService;
            this.configuration = configuration;
            this.productService = productService;
        }

        [HttpPost]
        [Route("insert")]
        public JsonResult Insert(PictureViewModel model)
        {
            string message = string.Empty;

            var pictureConfiguration = PictureConfiguration();

            foreach (var file in model.Files)
            {
                if (file != null && file.Length > 0)
                {
                    var desiredName = ArrangePictureName(model.Sku, Path.GetExtension(file.FileName));

                    string res = ImageOperations.SaveImage(hostingEnvironment, file, model.Sku, pictureConfiguration.LocalFolderPath, desiredName, null);

                    if (res.Contains("Failed") || res == "Not valid dimensions.")
                    {
                        message += $"Result: {res} File Name: {file.FileName}<br />";
                        continue;
                    }

                    bool isCompleted = ImageOperations.UploadImageToCdn(pictureConfiguration.FtpPath, pictureConfiguration.FtpUserName, pictureConfiguration.FtpPassword,
                        model.Sku, desiredName, $"{hostingEnvironment.WebRootPath}{res}");

                    if (isCompleted)
                    {
                        pictureService.AddNewPictures(new PicturesModel
                        {
                            ProductId = model.Picture.ProductId,
                            LocalPath = res,
                            CdnPath = $"{pictureConfiguration.CdnPath}{res.Replace(pictureConfiguration.LocalFolderPath, "/")}"
                        });
                    }
                    else
                    {
                        ImageOperations.DeleteFile($"{hostingEnvironment.WebRootPath}{res}");
                    }
                }
            }
            return Json(new { result = "Completed", message, url = Url.Action("Edit", "Product", new { id = model.Picture.ProductId, lang = CurrentLanguage }) });
        }

        [Route("edit")]
        public JsonResult Edit(int id, int productId, string pictureAlt, string pictureTitle, int order)
        {
            try
            {
                var model = pictureService.PicturesById(id);

                model.Alt = pictureAlt;
                model.Title = pictureTitle;
                model.Order = order;

                pictureService.EditPictures(model);

                return Json(new { success = true , url = Url.Action("Edit", "Product", new { id = productId, lang = CurrentLanguage }) });
            }
            catch (Exception)
            {
                return Json(new { success = false });
            }
        }

        [Route("delete/{id}")]
        public IActionResult Delete(int id)
        {
            var pictureConfiguration = PictureConfiguration();

            var picture = pictureService.PicturesById(id);

            var productId = picture.ProductId;

            ImageOperations.DeleteFileInServer(hostingEnvironment, picture.LocalPath);

            ImageOperations.DeleteFileInCdn($"{pictureConfiguration.FtpPath}{picture.CdnPath.Replace(pictureConfiguration.CdnPath, "")}", pictureConfiguration.FtpUserName, pictureConfiguration.FtpPassword);

            pictureService.DeletePicture(picture);

            return RedirectToAction("Edit", "Product", new { lang = CurrentLanguage, id = productId });
        }

        #region Methods

        private string ArrangePictureName(string sku, string extension)
        {
            try
            {
                var productName = productService.ProductBySku(sku).Barcode;
                var fileNames = pictureService.AllPicturesWithSku(sku);
                var lastFileIndex = 1;

                if (fileNames.Count > 0)
                {
                    foreach (var fileName in fileNames)
                    {
                        int pFrom = fileName.CdnPath.IndexOf("_") + "_".Length;
                        int pTo = fileName.CdnPath.LastIndexOf(".");

                        var swap = int.Parse(fileName.CdnPath.Substring(pFrom, pTo - pFrom));

                        if (swap > lastFileIndex)
                            lastFileIndex = swap;
                    }

                    lastFileIndex += 1;
                }

                return $"{productName}_{lastFileIndex}{extension}";
            }
            catch
            {
                var random = new Random();
                var exceptionNumber = random.Next(1000);

                return $"{sku}_failed_{exceptionNumber}{extension}";
            }
        }

        private PictureConfigurations PictureConfiguration()
        {
            var pictureConfiguration = new PictureConfigurations()
            {
                LocalFolderPath = configuration.GetValue<string>("PicturePaths:LocalPath"),
                LocalTempPath = configuration.GetValue<string>("PicturePaths:LocalTempPath"),
                FtpPath = configuration.GetValue<string>("PicturePaths:CdnPath"),
                FtpUserName = configuration.GetValue<string>("PicturePaths:CdnUserName"),
                FtpPassword = configuration.GetValue<string>("PicturePaths:CdnPassword"),
                CdnPath = configuration.GetValue<string>("PicturePaths:CdnUrlPath")
            };

            return pictureConfiguration;
        }

        #endregion


    }
}