using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace ProductDb.Common.Extensions
{
    public class ImageOperations
    {
        /// <summary>
        /// Resizes and rotates an image, keeping the original aspect ratio. Does not dispose the original
        /// Image instance.
        /// </summary>
        /// <param name="image">Image instance</param>
        /// <param name="width">desired width</param>
        /// <param name="height">desired height</param>
        /// <param name="rotateFlipType">desired RotateFlipType</param>
        /// <returns>new resized/rotated Image instance</returns>
        private static Image Resize(Image image, int width, int height, RotateFlipType rotateFlipType)
        {
            // clone the Image instance, since we don't want to resize the original Image instance
            var rotatedImage = image.Clone() as Image;
            rotatedImage.RotateFlip(rotateFlipType);
            var newSize = CalculateResizedDimensions(rotatedImage, width, height);

            var resizedImage = new Bitmap(newSize.Width, newSize.Height, PixelFormat.Format32bppArgb);
            resizedImage.SetResolution(72, 72);

            using (var graphics = Graphics.FromImage(resizedImage))
            {
                // set parameters to create a high-quality thumbnail
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.AntiAlias;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                // use an image attribute in order to remove the black/gray border around image after resize
                // (most obvious on white images), see this post for more information:
                // http://www.codeproject.com/KB/GDI-plus/imgresizoutperfgdiplus.aspx
                using (var attribute = new ImageAttributes())
                {
                    attribute.SetWrapMode(WrapMode.TileFlipXY);

                    // draws the resized image to the bitmap
                    graphics.DrawImage(rotatedImage, new Rectangle(new Point(0, 0), newSize), 0, 0, rotatedImage.Width, rotatedImage.Height, GraphicsUnit.Pixel, attribute);
                }
            }

            return resizedImage;
        }

        /// <summary>
        /// Calculates resized dimensions for an image, preserving the aspect ratio.
        /// </summary>
        /// <param name="image">Image instance</param>
        /// <param name="desiredWidth">desired width</param>
        /// <param name="desiredHeight">desired height</param>
        /// <returns>Size instance with the resized dimensions</returns>
        private static Size CalculateResizedDimensions(Image image, int desiredWidth, int desiredHeight)
        {
            var widthScale = (double)desiredWidth / image.Width;
            var heightScale = (double)desiredHeight / image.Height;

            // scale to whichever ratio is smaller, this works for both scaling up and scaling down
            var scale = widthScale < heightScale ? widthScale : heightScale;

            return new Size
            {
                Width = (int)(scale * image.Width),
                Height = (int)(scale * image.Height)
            };
        }

        /// <summary>
        /// Calculates image dimensions if desiredWidth and desiredHeight are provided.
        /// </summary>
        /// <param name="image"></param>
        /// <param name="desiredWidth"></param>
        /// <param name="desiredHeight"></param>
        /// <returns></returns>
        private static bool ValidateDimensions(Image image, int desiredWidth, int desiredHeight)
        {
            if (image.Width < desiredWidth || image.Height < desiredHeight)
                return false;

            return true;
        }

        /// <summary>
        /// Changes file name to criterion of seo 
        /// </summary>
        /// <param name="incomingText"></param>
        /// <returns></returns>
        private static string ToSeoUrl(string incomingText)
        {
            var extension = Path.GetExtension(incomingText);
            var fileName = Path.GetFileNameWithoutExtension(incomingText);

            fileName = fileName.Replace("ş", "s");
            fileName = fileName.Replace("Ş", "s");
            fileName = fileName.Replace("İ", "i");
            fileName = fileName.Replace("I", "i");
            fileName = fileName.Replace("ı", "i");
            fileName = fileName.Replace("ö", "o");
            fileName = fileName.Replace("Ö", "o");
            fileName = fileName.Replace("ü", "u");
            fileName = fileName.Replace("Ü", "u");
            fileName = fileName.Replace("Ç", "c");
            fileName = fileName.Replace("ç", "c");
            fileName = fileName.Replace("ğ", "g");
            fileName = fileName.Replace("Ğ", "g");
            fileName = fileName.Replace(" ", "-");
            fileName = fileName.Replace("---", "-");
            fileName = fileName.Replace("--", "-");
            fileName = fileName.Replace("?", "");
            fileName = fileName.Replace("/", "");
            fileName = fileName.Replace(".", "");
            fileName = fileName.Replace("'", "");
            fileName = fileName.Replace("#", "");
            fileName = fileName.Replace("%", "");
            fileName = fileName.Replace("&", "");
            fileName = fileName.Replace("*", "");
            fileName = fileName.Replace("!", "");
            fileName = fileName.Replace("@", "");
            fileName = fileName.Replace("+", "");

            fileName = fileName.ToLowerInvariant();
            fileName = fileName.Trim();

            string encodedUrl = (fileName ?? "").ToLowerInvariant();

            encodedUrl = Regex.Replace(encodedUrl, @"&+", "and");

            encodedUrl = encodedUrl.Replace("'", "");

            //encodedUrl = Regex.Replace(encodedUrl, @"[^a-z0-9]", "-");

            encodedUrl = Regex.Replace(encodedUrl, @"-+", "-");

            encodedUrl = encodedUrl.Trim('-');

            return $"{encodedUrl}{extension}";
        }

        private static bool OpenDirectoryIfNotExistsInCdn(string ftpUrl, string login, string password, string sku)
        {
            try
            {
                #region Get List Of Directories

                var request = (FtpWebRequest)WebRequest.Create($"ftp://{ftpUrl}");
                request.Credentials = new NetworkCredential(login, password);
                request.UseBinary = true;

                request.Method = WebRequestMethods.Ftp.ListDirectory;
                var response = request.GetResponse();
                var responseStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(responseStream);
                string names = reader.ReadToEnd();

                reader.Close();
                response.Close();

                var directoryNames = names.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();

                #endregion

                #region Open Directory If Not Exists

                if (!directoryNames.Contains(sku))
                {
                    var directoryRequest = (FtpWebRequest)WebRequest.Create($"ftp://{ftpUrl}/{sku}");
                    directoryRequest.Credentials = new NetworkCredential(login, password);
                    directoryRequest.Method = WebRequestMethods.Ftp.MakeDirectory;
                    WebResponse directoryResponse = directoryRequest.GetResponse();
                }

                #endregion

                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool CheckIfFileExistsOnCdn(string checkPath, string login, string password)
        {
            var request = (FtpWebRequest)WebRequest.Create(checkPath);
            request.Credentials = new NetworkCredential(login, password);
            request.Method = WebRequestMethods.Ftp.GetFileSize;
            try
            {
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                return true;
            }
            catch (WebException ex)
            {
                FtpWebResponse response = (FtpWebResponse)ex.Response;
                if (response.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable)
                    return false;
            }
            return false;
        }

        public static bool DeleteFileInCdn(string deletePath, string login, string password)
        {
            try
            {
                var request = (FtpWebRequest)WebRequest.Create($"ftp://{deletePath}");
                request.Credentials = new NetworkCredential(login, password);
                request.Method = WebRequestMethods.Ftp.DeleteFile;
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                response.Close();
                return true;
            }
            catch (WebException ex)
            {
                FtpWebResponse response = (FtpWebResponse)ex.Response;
                if (response.StatusCode == FtpStatusCode.ActionNotTakenFilenameNotAllowed)
                    return false;
            }

            return false;

        }

        public static bool DeleteFileInServer(IHostingEnvironment environment, string deletePath)
        {
            try
            {
                if (File.Exists($"{environment.WebRootPath}{deletePath}"))
                {
                    File.Delete($"{environment.WebRootPath}{deletePath}");
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }

        }

        public static string DownLoadFile(IHostingEnvironment environment, string sku, string localDirectory, string downloadUrl, string desiredName)
        {
            try
            {
                string folderPath = $"{environment.WebRootPath}{localDirectory}{sku}";

                string imagePath = $"{localDirectory}{sku}/{ToSeoUrl(desiredName)}";

                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                if (File.Exists(imagePath))
                    File.Delete(imagePath);

                var savePath = environment.WebRootPath + imagePath;

                using (var client = new WebClient())
                {
                    client.DownloadFile(downloadUrl, savePath);
                }

                return imagePath;
            }
            catch
            {
                return string.Empty;
            }

        }

        public static string SaveImage(IHostingEnvironment environment, IFormFile file, string sku, string localDirectory, string desiredName = "", string previousPath = "")
        {
            using (var image = Image.FromStream(file.OpenReadStream()))
            {
                if (!ValidateDimensions(image, 999, 999))
                    return "Not valid dimensions.";

                try
                {
                    var resizedImage = Resize(image, 1000, 1000, RotateFlipType.RotateNoneFlipNone);

                    string folderPath = $"{environment.WebRootPath}{localDirectory}{sku}";

                    string imagePath = $"{localDirectory}{sku}/{ToSeoUrl(desiredName)}";

                    if (!string.IsNullOrWhiteSpace(previousPath) && File.Exists(previousPath))
                        File.Delete(previousPath);

                    if (!Directory.Exists(folderPath))
                        Directory.CreateDirectory(folderPath);

                    if (File.Exists(imagePath))
                        File.Delete(imagePath);

                    var savePath = environment.WebRootPath + imagePath;

                    image.Save(savePath);

                    return imagePath;
                }
                catch (Exception ex)
                {
                    return $"Failed{ex.Message}-{ex.InnerException}";
                }
            }
        }
        public static string SaveImage(IHostingEnvironment environment, string filePath, string sku, string localDirectory, string desiredName = "", string previousPath = "")
        {
            using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(filePath)))
            {
                using (var image = Image.FromStream(ms))
                {
                    if (!ValidateDimensions(image, 999, 999))
                        return "Not valid dimensions.";

                    try
                    {
                        var resizedImage = Resize(image, 1000, 1000, RotateFlipType.RotateNoneFlipNone);

                        string folderPath = $"{environment.WebRootPath}{localDirectory}{sku}";

                        string imagePath = $"{localDirectory}{sku}/{ToSeoUrl(desiredName)}";

                        if (!string.IsNullOrWhiteSpace(previousPath) && File.Exists(previousPath))
                            File.Delete(previousPath);

                        if (!Directory.Exists(folderPath))
                            Directory.CreateDirectory(folderPath);

                        if (File.Exists(imagePath))
                            File.Delete(imagePath);

                        var savePath = environment.WebRootPath + imagePath;

                        image.Save(savePath);

                        return imagePath;
                    }
                    catch
                    {
                        return "Failed";
                    }
                }

            }
        }


        public static bool UploadImageToCdn(string ftpUrl, string login, string password, string sku, string fileName, string localFilePath)
        {
            if (CheckIfFileExistsOnCdn($"ftp://{ftpUrl}/{sku}/{fileName}", login, password))
                DeleteFileInCdn($"ftp://{ftpUrl}/{sku}/{fileName}", login, password);

            if (OpenDirectoryIfNotExistsInCdn(ftpUrl, login, password, sku))
            {
                using (var client = new WebClient())
                {
                    client.Credentials = new NetworkCredential(login, password);
                    client.UploadFile($"ftp://{ftpUrl}/{sku}/{ToSeoUrl(fileName)}", WebRequestMethods.Ftp.UploadFile, localFilePath);
                }
                return true;
            }
            return false;
        }

        public static void DeleteFile(string filePath)
        {
            if (File.Exists(filePath))
                File.Delete(filePath);
        }


    }
}
