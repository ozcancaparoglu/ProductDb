
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using ProductDb.Common.Enums;
using ProductDb.Common.Extensions;
using ProductDb.Data.BiggBrandsDb;
using ProductDb.Data.BiggBrandsDb.ProductDocks;
using ProductDb.Data.NopDb;
using ProductDb.Logging;
using ProductDb.Mapping.AutoMapperConfigurations;
using ProductDb.Mapping.BiggBrandDbModels;
using ProductDb.Mapping.BiggBrandDbModels.ProductDocksMapping;
using ProductDb.Repositories.BaseRepositories;
using ProductDb.Repositories.UnitOfWorks;
using ProductDb.Services.AttributesServices;
using ProductDb.Services.BrandServices;
using ProductDb.Services.CategoryServices;
using ProductDb.Services.LanguageServices;
using ProductDb.Services.PictureServices;
using ProductDb.Services.ProductServices;
using ProductDb.Services.TaxServices;
using ProductDb.Services.WarehouseServices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ProductDb.Services.ProductDockServices
{
    public class ProductDockService : IProductDockService
    {
        private readonly ILoggerManager loggerManager;
        private readonly IHostingEnvironment hostingEnvironment;
        private readonly IProductDockPictureService productDockPictureService;
        private readonly IPictureService pictureService;
        private readonly IAttributesService attributesService;
        private readonly IWarehouseService warehouseService;
        private readonly IParentProductService parentProductService;
        private readonly IBrandService brandService;
        private readonly IConfiguration configuration;
        private readonly IUnitOfWork unitOfWork;
        private readonly IUnitOfWorkNop unitOfWorkNop;
        private readonly IAutoMapperConfiguration autoMapper;

        private readonly IGenericRepository<ProductDock> productDockRepo;
        private readonly IGenericRepository<ProductDockPictures> productDockPicturesRepo;

        private readonly IGenericRepositoryNop<ProductNop> productNopRepo;
        private readonly IGenericRepositoryNop<ProductPictureMappingNop> productPictureMappingNopRepo;
        private readonly IGenericRepositoryNop<PictureNop> pictureNopRepo;
        private readonly IGenericRepositoryNop<LocalizedPropertyNop> localizedPropertyNopRepo;
        private readonly IGenericRepository<ProductDockAttribute> productDockAttributeRepo;
        private readonly IGenericRepository<ProductDockCategory> productDockCategoryRepo;

        private readonly ILanguageService languageService;
        private readonly IProductService productService;
        private readonly ITaxService taxService;
        private readonly IParentProductDockService parentProductDockService;
        private readonly IGenericRepository<AttributesValue> attributeRepo;
        private readonly IGenericRepository<ParentProductDock> parentProductDockRepo;
        private readonly IGenericRepository<ProductAttributeMapping> productAttributeMappingRepo;
        private readonly IGenericRepository<ProductAttributeValue> productAttributeValueRepo;

        public ProductDockService(IUnitOfWork unitOfWork, IUnitOfWorkNop unitOfWorkNop, IAutoMapperConfiguration autoMapper,
            ILanguageService languageService, IProductService productService, ITaxService taxService,
            IParentProductDockService parentProductDockService, IConfiguration configuration, IBrandService brandService,
            IParentProductService parentProductService, IWarehouseService warehouseService, IAttributesService attributesService, IPictureService pictureService,
            IProductDockPictureService productDockPictureService, IHostingEnvironment hostingEnvironment, ILoggerManager loggerManager)
        {
            this.loggerManager = loggerManager;
            this.hostingEnvironment = hostingEnvironment;
            this.productDockPictureService = productDockPictureService;
            this.pictureService = pictureService;
            this.attributesService = attributesService;
            this.warehouseService = warehouseService;
            this.parentProductService = parentProductService;
            this.brandService = brandService;
            this.configuration = configuration;
            this.unitOfWork = unitOfWork;
            this.unitOfWorkNop = unitOfWorkNop;
            this.autoMapper = autoMapper;
            this.languageService = languageService;
            this.productService = productService;
            this.taxService = taxService;
            this.parentProductDockService = parentProductDockService;

            attributeRepo = this.unitOfWork.Repository<AttributesValue>();
            parentProductDockRepo = this.unitOfWork.Repository<ParentProductDock>();
            productDockRepo = this.unitOfWork.Repository<ProductDock>();
            productDockPicturesRepo = this.unitOfWork.Repository<ProductDockPictures>();
            productDockCategoryRepo = this.unitOfWork.Repository<ProductDockCategory>();
            productNopRepo = this.unitOfWorkNop.Repository<ProductNop>();
            productPictureMappingNopRepo = this.unitOfWorkNop.Repository<ProductPictureMappingNop>();
            pictureNopRepo = this.unitOfWorkNop.Repository<PictureNop>();
            localizedPropertyNopRepo = this.unitOfWorkNop.Repository<LocalizedPropertyNop>();
            productDockAttributeRepo = this.unitOfWork.Repository<ProductDockAttribute>();
            productAttributeMappingRepo = this.unitOfWork.Repository<ProductAttributeMapping>();
            productAttributeValueRepo = this.unitOfWork.Repository<ProductAttributeValue>();
        }

        public ICollection<ProductDockModel> AllProducts(string sku, int skip, int take)
        {
            var productDockEntitys = productDockRepo.Filter(x => x.Sku.Contains(sku), o => o.OrderBy(p => p.Id), "", 1, take);

            var productDockDTO = autoMapper.MapCollection<ProductDock, ProductDockModel>(productDockEntitys);
            return productDockDTO.ToList();
        }

        public ICollection<ProductDockModel> AllProducts(int skip, int take)
        {
            var productDockEntitys = productDockRepo.Table().OrderBy(a => a.CreatedDate).Skip(skip).Take(take);

            var productDockDTO = autoMapper.MapCollection<ProductDock, ProductDockModel>(productDockEntitys);

            return productDockDTO.ToList();

        }

        public ProductDockModel GetProductDockBySku(string sku)
        {
            var productDockEntity = productDockRepo.Find(x => x.Sku == sku);

            if (productDockEntity != null)
                return autoMapper.MapObject<ProductDock, ProductDockModel>(productDockEntity);

            return null;
        }

        public ProductDockModel GetProductDockbyId(int Id)
        {
            var productDockEntity = productDockRepo.GetById(Id);

            var productDockDTO = autoMapper.MapObject<ProductDock, ProductDockModel>(productDockEntity);

            return productDockDTO;
        }

        public ProductDockModel ProductDockbyId(int Id)
        {
            var productDockEntity = productDockRepo.Filter(x => x.Id == Id, null, "ParentProductDock,VatRate,Pictures,ProductDockCategory,Attributes").FirstOrDefault();

            var productDockDTO = autoMapper.MapObject<ProductDock, ProductDockModel>(productDockEntity);

            productDockDTO.CostString = productDockDTO.Cost.ToString();
            productDockDTO.PsfPriceString = Convert.ToString(productDockDTO.PsfPrice == null ? 0 : productDockDTO.PsfPrice);
            productDockDTO.DesiString = Convert.ToString(productDockDTO.Desi == null ? 0 : productDockDTO.Desi);

            return productDockDTO;
        }

        public ICollection<ProductDockCategoryModel> ProductDockCategoryByParentId(int parentCategoryId)
        {
            return autoMapper.MapCollection<ProductDockCategory, ProductDockCategoryModel>(
                productDockCategoryRepo.Filter(x => x.ParentCategoryId == parentCategoryId)).ToList();
        }

        public ProductDockCategoryModel AddNewProductDockCategory(ProductDockCategoryModel model)
        {
            var entity = autoMapper.MapObject<ProductDockCategoryModel, ProductDockCategory>(model);

            var savedEntity = productDockCategoryRepo.Add(entity);

            return autoMapper.MapObject<ProductDockCategory, ProductDockCategoryModel>(savedEntity);
        }

        public ProductDockCategoryModel EditProductDockCategory(ProductDockCategoryModel model)
        {
            var entity = autoMapper.MapObject<ProductDockCategoryModel, ProductDockCategory>(model);

            var updatedEntity = productDockCategoryRepo.Update(entity);

            return autoMapper.MapObject<ProductDockCategory, ProductDockCategoryModel>(updatedEntity);
        }

        public ICollection<ProductDockCategoryModel> ProductDockCategoriesBySupplierId(int supplierId)
        {
            return autoMapper.MapCollection<ProductDockCategory, ProductDockCategoryModel>(
                productDockCategoryRepo.Filter(x => x.SupplierId == supplierId)).ToList();
        }

        public ICollection<ProductDockModel> ProductDocksBySupplierAndCategoryId(int supplierId, int? categoryId)
        {
            List<ProductDockModel> list = null;
            if (categoryId.HasValue)
            {
                list = autoMapper.MapCollection<ProductDock, ProductDockModel>
                (productDockRepo.Filter(x => x.SupplierId == supplierId && x.ProductDockCategoryId == categoryId, null, "Pictures,ParentProductDock")).ToList();
            }
            else
            {
                list = autoMapper.MapCollection<ProductDock, ProductDockModel>(productDockRepo.Filter(x => x.SupplierId == supplierId, null, "Pictures,ParentProductDock")).ToList();
            }

            return list;
        }

        public ProductDockModel AddNewProductDock(ProductDockModel model)
        {
            var entity = autoMapper.MapObject<ProductDockModel, ProductDock>(model);

            var savedEntity = productDockRepo.Add(entity);

            return autoMapper.MapObject<ProductDock, ProductDockModel>(savedEntity);

        }

        public int AddNewProductDockPictures(int productDockId, ICollection<ProductDockPicturesModel> modelList)
        {
            var productDockPictures = productDockPicturesRepo.FindAll(x => x.ProductDockId == productDockId);

            if (productDockPictures != null || productDockPictures.Count > 0)
                productDockPicturesRepo.DeleteRange(productDockPictures.ToList());

            return productDockPicturesRepo.AddRange(autoMapper.MapCollection<ProductDockPicturesModel, ProductDockPictures>(modelList).ToList());
        }

        public int AddNewProductDockAttributes(int productDockId, ICollection<ProductDockAttributeModel> modelList)
        {
            var productDockAttributes = productDockAttributeRepo.FindAll(x => x.ProductDockId == productDockId);

            if (productDockAttributes != null || productDockAttributes.Count > 0)
                productDockAttributeRepo.DeleteRange(productDockAttributes.ToList());

            return productDockAttributeRepo.AddRange(autoMapper.MapCollection<ProductDockAttributeModel, ProductDockAttribute>(modelList).ToList());
        }

        public ProductDockModel Edit(ProductDockModel productDock)
        {
            if(!string.IsNullOrWhiteSpace(productDock.CostString))
                productDock.Cost = decimal.Parse(productDock.CostString);
            if (!string.IsNullOrWhiteSpace(productDock.DesiString))
                productDock.Desi = decimal.Parse(productDock.DesiString);
            if (!string.IsNullOrWhiteSpace(productDock.PsfPriceString))
                productDock.PsfPrice = decimal.Parse(productDock.PsfPriceString);

            var model = autoMapper.MapObject<ProductDockModel, ProductDock>(productDock);
            productDockRepo.Update(model);

            return productDock;
        }

        public ICollection<ProductDockModel> NotVariantedProductDocks(int id)
        {
            var productDock = autoMapper.MapCollection<ProductDock, ProductDockModel>(productDockRepo.Filter(x => x.Id != id && x.ParentProductDockId == null)).ToList();
            return productDock;

        }

        public void SaveProductVariant(List<ProductDockModel> productDocks, int parentProductDockId)
        {
            productDocks.Add(autoMapper.MapObject<ProductDock, ProductDockModel>(productDockRepo.Find(x => x.Id == parentProductDockId)));

            var datas = productDockRepo.Table().Where(x => productDocks.Any(k => k.Id == x.Id)).ToList();

            var parentId = parentProductDockId;
            var parent = productDockRepo.Find(x => x.Id == parentId);

            var now = DateTime.Now;

            var parentData = parentProductDockRepo.Add(new ParentProductDock()
            {
                Sku = $"{parent.Sku}_PP",
                Title = parent.Name,
                ProductDockCategoryId = parent.ProductDockCategoryId,
                SupplierId = parent.SupplierId,
                CreatedDate = now,
                State = (int)State.Active
            });

            datas.ForEach(x => x.ProductDockCategoryId = parent.ProductDockCategoryId);
            datas.ForEach(x => x.ParentProductDockId = parentData.Id);

            productDockRepo.UpdateRange(datas);

            //productDockRepo.Add()
        }

        public void ChangeProductParent(int id, int parentId)
        {
            var productDock = GetProductDockbyId(id);

            var isParent = parentProductDockService.ParentProductDockBySku($"{productDock.Sku}_PP");

            if (isParent != null)
            {
                var parentProductDock = parentProductDockService.ParentProductDockById(productDock.ParentProductDockId.Value);
                parentProductDock.Products = null;

                var variants = ProductDockVariantById(parentProductDock.Id);

                var variantParent = variants.FirstOrDefault(x => string.Format("{0}_PP", x.Sku) == parentProductDock.Sku);
                variants.Remove(variantParent);

                if (variants.Count > 0)
                {
                    var firstVariant = variants.FirstOrDefault();

                    parentProductDock.Title = firstVariant.Name;
                    parentProductDock.Sku = $"{firstVariant.Sku}_PP";

                    parentProductDockRepo.Update(autoMapper.MapObject<ParentProductDockModel, ParentProductDock>(parentProductDock));
                }
                else
                {
                    parentProductDockRepo.Delete(autoMapper.MapObject<ParentProductDockModel, ParentProductDock>(parentProductDock));
                }

                if (parentId != 0)
                {
                    productDock.ParentProductDockId = parentId;
                    productDockRepo.Update(autoMapper.MapObject<ProductDockModel, ProductDock>(productDock));
                }
                else
                {
                    variantParent.ParentProductDockId = null;
                    variantParent.ParentProductDock = null;
                    productDockRepo.Update(autoMapper.MapObject<ProductDockModel, ProductDock>(variantParent));
                }
            }
            else
            {
                if (parentId == 0)
                    productDock.ParentProductDockId = null;
                else
                    productDock.ParentProductDockId = parentId;

                productDockRepo.Update(autoMapper.MapObject<ProductDockModel, ProductDock>(productDock));
            }

        }

        public ICollection<ProductDockModel> ProductDockVariantById(int parentId)
        {
            return autoMapper.MapCollection<ProductDock, ProductDockModel>(productDockRepo.Filter(x => x.ParentProductDockId == parentId)).ToList();
        }

        public ICollection<ProductDockModel> ProductDockByIDs(List<int> IDs)
        {
            return autoMapper.MapCollection<ProductDock, ProductDockModel>(productDockRepo.Filter(x => IDs.Any(k => k == x.Id))).ToList();
        }

        public string FormattedParentSKU(string sku)
        {
            string prefix = configuration["SKU:ParentPrefix"];
            return $"{sku}_{prefix}";
        }

        public ICollection<ProductDockCategoryModel> ProductDockCategories()
        {
            return autoMapper.MapCollection<ProductDockCategory, ProductDockCategoryModel>(productDockCategoryRepo.GetAll()).ToList();
        }

        public ProductDockCategoryModel ProductDockCategoryById(int Id)
        {
            return autoMapper.MapObject<ProductDockCategory, ProductDockCategoryModel>(productDockCategoryRepo.Filter(x => x.Id == Id).FirstOrDefault());
        }


        public void SaveAllProductDock(int selecetedCatId, int productDockCategoryId, int supplierId)
        {
            List<ProductDockModel> productDocks;

            if (productDockCategoryId != 0)
            {
                productDocks = autoMapper.MapCollection<ProductDock, ProductDockModel>(productDockRepo.
                   Filter(x => x.ProductDockCategoryId == productDockCategoryId && x.SupplierId == supplierId)).ToList();
            }
            else
            {
                productDocks = autoMapper.MapCollection<ProductDock, ProductDockModel>(productDockRepo.
                  Filter(x => x.SupplierId == supplierId)).ToList();
            }

            DateTime date = DateTime.Now;

            var pictureConfiguration = pictureService.PictureConfiguration();
            string message = string.Empty;

            foreach (var item in productDocks)
            {
                var brandId = 0;
                var parentId = 0;
                var brandIsDefined = brandService.IsDefined(item.Brand);

                if (brandIsDefined == null)
                {
                    brandId = brandService.AddNewBrand(new Mapping.BiggBrandDbModels.BrandModel()
                    {
                        CreatedDate = date,
                        Name = item.Brand,
                        Prefix = "",
                        ProcessedBy = 1,
                        State = (int)State.Active
                    }).Id;
                }
                else
                {
                    brandId = brandIsDefined.Id;

                    if (item.ParentProductDockId == null)
                    {
                        var parentIsDefined = parentProductDockService.ParentProductDockBySku($"{item.Sku}_PP");
                        if (parentIsDefined == null)
                        {
                            parentId = parentProductService.AddNewParentProduct(new ParentProductModel()
                            {
                                BrandId = brandId,
                                CategoryId = selecetedCatId,
                                ProcessedBy = 1,
                                CreatedDate = date,
                                Description = item.FullDescription,
                                ShortDescription = item.ShortDescription,
                                Sku = $"{item.Sku}_PP",
                                State = (int)State.Active,
                                SupplierId = supplierId,
                                Title = item.Name
                            }).Id;
                        }
                        else
                            parentId = parentIsDefined.Id;
                    }
                    else
                    {
                        var parentProductkDock = parentProductDockService.ParentProductDockById(item.ParentProductDockId.Value);

                        var parentIsDefined = parentProductDockService.ParentProductDockBySku(parentProductkDock.Sku);

                        if (parentIsDefined == null)
                        {
                            parentId = parentProductService.AddNewParentProduct(new Mapping.BiggBrandDbModels.ParentProductModel()
                            {
                                BrandId = brandId,
                                CategoryId = selecetedCatId,
                                ProcessedBy = 1,
                                CreatedDate = date,
                                Description = parentProductkDock.Title,
                                ShortDescription = parentProductkDock.Title,
                                Sku = parentProductkDock.Sku,
                                State = (int)State.Active,
                                SupplierId = supplierId,
                                Title = parentProductkDock.Title
                            }).Id;
                        }
                        else
                            parentId = parentIsDefined.Id;

                    }

                    var productIsDefined = productService.ProductBySku(item.Sku);

                    if (productIsDefined == null)
                    {
                        // product insert
                        var product = productService.AddNewProduct(new Mapping.BiggBrandDbModels.ProductModel()
                        {
                            AbroadDesi = item.Desi,
                            Barcode = item.Gtin,
                            BuyingPrice = item.Cost,
                            CurrencyId = item.CurrencyId,
                            CreatedDate = date,
                            Description = item.FullDescription,
                            Desi = item.Desi,
                            Gtip = item.Gtin,
                            MetaDescription = item.MetaDescription,
                            MetaKeywords = item.MetaKeywords,
                            MetaTitle = item.MetaTitle,
                            Model = item.Model,
                            Name = item.Name,
                            ParentProductId = parentId,
                            ProcessedBy = 1,
                            Title = item.Name,
                            ShortDescription = item.ShortDescription,
                            Stock = item.Stock,
                            Sku = item.Sku,
                            State = (int)State.Active,
                            VatRateId = item.VatRateId,
                            SupplierUniqueId = item.SupplierUniqueId
                        });

                        // Attribute
                        var attributes = ProductDockAttributesByProductDockId(item.Id);

                        var hasAttributes = attributeRepo.Table().Where(x => attributes.Any(k => k.Value.ToLowerInvariant() == x.Value.ToLowerInvariant()));

                        foreach (var hAttr in hasAttributes)
                        {
                            // product mapping
                            productAttributeMappingRepo.Add(new ProductAttributeMapping()
                            {
                                AttributesId = hAttr.AttributesId,
                                AttributeValueId = hAttr.Id,
                                ProductId = product.Id,
                                RequiredAttributeValue = null,
                                IsRequired = false,
                                CreatedDate = date,
                                UpdatedDate = date,
                                State = (int)State.Active,
                                ProcessedBy = 1
                            });
                            var attValue = attributesService.AttributesValueById(hAttr.Id);
                            // attibute value
                            productAttributeValueRepo.Add(new ProductAttributeValue()
                            {
                                ProductId = product.Id,
                                Attribute = attributesService.AttributesById(hAttr.AttributesId.Value) == null ? string.Empty :
                                            attributesService.AttributesById(hAttr.AttributesId.Value).Name,
                                AttributeValue = attValue == null ? string.Empty :
                                            attValue.Value,
                                Unit = null,
                                CreatedDate = date,
                                UpdatedDate = date,
                                State = (int)State.Active,
                                ProcessedBy = 1,
                                AttributeValueId = attValue.Id
                            });
                        }

                        // warehouse type 
                        warehouseService.AddNewProductStock(new Mapping.BiggBrandDbModels.WarehouseProductStockModel()
                        {
                            ProductId = product.Id,
                            Name = product.Name,
                            Sku = product.Sku,
                            WarehouseTypeId = 5,
                            Quantity = product.Stock,
                            CreatedDate = date,
                            State = (int)State.Active,
                            ProcessedBy = 1
                        });

                        // download picture
                        var picture = productDockPictureService.ProductDockPictureByProductDockId(item.Id);
                        if (picture != null)
                        {
                            var desiredName = pictureService.ArrangePictureName(item.Sku, Path.GetFileName(picture.DownloadUrl));

                            string res = ImageOperations.SaveImage(hostingEnvironment, picture.DownloadUrl,
                                item.Sku, pictureConfiguration.LocalFolderPath, desiredName, null);

                            if (res == "Failed" || res == "Not valid dimensions.")
                            {
                                message += $"Not Downloaded Picture Information: SKU :  {item.Sku} Result : {res} File Name: {desiredName}" + Environment.NewLine;
                                loggerManager.LogError(message);
                                continue;
                            }

                            bool isCompleted = ImageOperations.UploadImageToCdn(pictureConfiguration.FtpPath, pictureConfiguration.FtpUserName, pictureConfiguration.FtpPassword,
                                     item.Sku, desiredName, $"{hostingEnvironment.WebRootPath}{res}");

                            if (isCompleted)
                            {
                                pictureService.AddNewPictures(new PicturesModel
                                {
                                    ProductId = product.Id,
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
                }

            }

        }

        public ICollection<ProductDockAttributeModel> ProductDockAttributesByProductDockId(int id)
        {
            return autoMapper.MapCollection<ProductDockAttribute, ProductDockAttributeModel>
                (productDockAttributeRepo.Filter(x => x.ProductDockId == id)).ToList();
        }

    }
}
