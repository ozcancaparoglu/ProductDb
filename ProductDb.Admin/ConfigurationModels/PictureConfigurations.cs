namespace ProductDb.Admin.ConfigurationModels
{
    public class PictureConfigurations
    {
        public string LocalFolderPath { get; set; }
        public string LocalTempPath { get; set; }
        public string FtpPath { get; set; }
        public string FtpUserName { get; set; }
        public string FtpPassword { get; set; }
        public string CdnPath { get; set; }
    }
}
