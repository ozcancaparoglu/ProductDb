using AutoMapper.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using ProductDb.Services.ImportServices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ProductDb.Services.UploadService
{
    public class UploadService : IUploadService
    {
        private readonly IExcelService excelService;
        private readonly IHostingEnvironment hostingEnvironment;

        public UploadService(IHostingEnvironment hostingEnvironment,
                             IExcelService excelService)
        {
            this.excelService = excelService;
            this.hostingEnvironment = hostingEnvironment;
        }

        public string GetRootPath()
        {
            return hostingEnvironment.WebRootPath;
        }

        public string GetUplaodedPath(string fileName)
        {
            return Path.Combine(GetRootPath(),"importedFiles", fileName);
        }

        public bool isUploadExcelDocumentCompleted(IFormFile file, string path)
        {

            bool isCopied = false;

            using (var stream = new FileStream(path, FileMode.Create))
            {
                file.CopyTo(stream);

                isCopied = true;
            }
            return isCopied;
        }
    }
}
