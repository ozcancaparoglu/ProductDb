using System;
using System.Collections.Generic;
using System.Text;

namespace ProductDb.Services.PictureServices
{
    public class PictureConfiguration
    {
        public string LocalFolderPath { get; set; }
        public string LocalTempPath { get; set; }
        public string FtpPath { get; set; }
        public string FtpUserName { get; set; }
        public string FtpPassword { get; set; }
        public string CdnPath { get; set; }
    }
}
