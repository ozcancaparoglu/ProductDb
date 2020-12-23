using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProductDb.Services.UploadService
{
    public interface IUploadService
    {
        bool isUploadExcelDocumentCompleted(IFormFile file, string path);
        string GetRootPath();
        string GetUplaodedPath(string path);
    }
}
