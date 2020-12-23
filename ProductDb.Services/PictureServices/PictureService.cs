using Microsoft.Extensions.Configuration;
using ProductDb.Common.Enums;
using ProductDb.Data.BiggBrandsDb;
using ProductDb.Mapping.AutoMapperConfigurations;
using ProductDb.Mapping.BiggBrandDbModels;
using ProductDb.Repositories.BaseRepositories;
using ProductDb.Repositories.UnitOfWorks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProductDb.Services.PictureServices
{
    public class PictureService : IPictureService
    {
        private readonly IConfiguration configuration;
        private readonly IUnitOfWork unitOfWork;
        private readonly IAutoMapperConfiguration autoMapper;
        private readonly IGenericRepository<Pictures> picturesRepo;
        private readonly IGenericRepository<Product> productRepo;

        public PictureService(IUnitOfWork unitOfWork, IAutoMapperConfiguration autoMapper, IConfiguration configuration)
        {
            this.configuration = configuration;
            this.unitOfWork = unitOfWork;
            this.autoMapper = autoMapper;
            picturesRepo = this.unitOfWork.Repository<Pictures>();
            productRepo = this.unitOfWork.Repository<Product>();
        }

        public ICollection<PicturesModel> AllPicturesWithSku(string sku)
        {
            var product = productRepo.Find(x => x.Sku == sku);

            if (product != null)
                return AllPictures(product.Id);

            return new List<PicturesModel>();
        }

        public ICollection<PicturesModel> AllPictures(int id)
        {
            return autoMapper.MapCollection<Pictures, PicturesModel>(picturesRepo.FindAll(x => x.ProductId == id)).ToList();
        }
        public ICollection<PicturesModel> AllActivePictures(int id)
        {
            return autoMapper.MapCollection<Pictures, PicturesModel>(picturesRepo.Filter(x => x.ProductId == id && x.State == (int)State.Active)).ToList();
        }

        public PicturesModel PicturesById(int id)
        {
            return autoMapper.MapObject<Pictures, PicturesModel>(picturesRepo.GetById(id));
        }

        public PicturesModel AddNewPictures(PicturesModel model)
        {
            var entity = autoMapper.MapObject<PicturesModel, Pictures>(model);

            var savedEntity = picturesRepo.Add(entity);

            return autoMapper.MapObject<Pictures, PicturesModel>(savedEntity);
        }

        public PicturesModel EditPictures(PicturesModel model)
        {
            var entity = autoMapper.MapObject<PicturesModel, Pictures>(model);

            var savedEntity = picturesRepo.Update(entity);

            return autoMapper.MapObject<Pictures, PicturesModel>(savedEntity);
        }

        public int DeletePicture(PicturesModel model)
        {
            var entity = autoMapper.MapObject<PicturesModel, Pictures>(model);

            return picturesRepo.Delete(entity);
        }

        public void SetState(int objectId)
        {
            var entity = picturesRepo.GetById(objectId);

            if (entity.State == (int)State.Active)
                entity.State = (int)State.Passive;

            else
                entity.State = (int)State.Active;

            picturesRepo.Update(entity);
        }

        public string ArrangePictureName(string sku, string extension)
        {
            try
            {
                //var productName = productService.ProductBySku(sku).Barcode;
                var productName = productRepo.Table().FirstOrDefault(x => x.Sku == sku).Barcode;
                var fileNames = AllPicturesWithSku(sku);
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

        public PictureConfiguration PictureConfiguration()
        {
            var pictureConfiguration = new PictureConfiguration()
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
    }
}
