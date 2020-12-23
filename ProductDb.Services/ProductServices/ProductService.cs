using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using ProductDb.Common.Cache;
using ProductDb.Common.Enums;
using ProductDb.Common.Extensions;
using ProductDb.Common.GlobalEntity;
using ProductDb.Data.BiggBrandsDb;
using ProductDb.Data.BiggBrandsDb.ProductDocks;
using ProductDb.Data.NopDb;
using ProductDb.Mapping.AutoMapperConfigurations;
using ProductDb.Mapping.BiggBrandDbModelFields;
using ProductDb.Mapping.BiggBrandDbModels;
using ProductDb.Mapping.BiggBrandDbModels.ProductDocksMapping;
using ProductDb.Repositories.BaseRepositories;
using ProductDb.Repositories.UnitOfWorks;
using ProductDb.Services.AttributesServices;
using ProductDb.Services.CategoryServices;
using ProductDb.Services.LanguageServices;
using ProductDb.Services.PictureServices;
using ProductDb.Services.TaxServices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Attribute = ProductDb.Common.GlobalEntity.Attribute;

namespace ProductDb.Services.ProductServices
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly ITaxService taxService;
        private readonly ICacheManager cacheManager;
        private readonly IAttributesService attributesService;
        private readonly ICategoryService categoryService;
        private readonly ILanguageService languageService;
        private readonly IUnitOfWorkNop unitOfWorkNop;
        private readonly IAutoMapperConfiguration autoMapper;
        private readonly IHostingEnvironment hostingEnvironment;
        private readonly IPictureService pictureService;
        private readonly IConfiguration configuration;
        private readonly IMemoryCache memoryCache;

        private readonly IGenericRepository<Language> languageRepo;
        private readonly IGenericRepository<LanguageValues> languageValueRepo;
        private readonly IGenericRepository<ProductGroup> productGroupRepo;
        private readonly IGenericRepository<Product> productRepo;
        private readonly IGenericRepository<ProductAttributeMapping> productAttributeRepo;
        private readonly IGenericRepository<ProductAttributeValue> productAttributeValueRepo;
        private readonly IGenericRepository<ParentProduct> parentProductRepo;
        private readonly IGenericRepository<Category> categoryRepo;
        private readonly IGenericRepository<CategoryAttributeMapping> categoryAttributeRepo;
        private readonly IGenericRepository<Attributes> attributesRepo;
        private readonly IGenericRepository<AttributesValue> attributesValueRepo;
        private readonly IGenericRepository<StoreProductMapping> storeProductMappingRepo;
        private readonly IGenericRepository<WarehouseProductStock> warehouseProductStockRepo;
        private readonly IGenericRepository<ProductDock> productDockRepo;
        private readonly IGenericRepository<Store> storeRepo;
        private readonly IGenericRepository<Pictures> pictureRepo;
        private readonly IGenericRepository<StoreWarehouseMapping> storeWarehouseRepo;
        private readonly IGenericRepository<ProductDockPictures> productDockPicturesRepo;
        private readonly IGenericRepository<LanguageValues> languageValuesRepo;
        private readonly IGenericRepositoryNop<ProductNop> productNopRepo;
        private readonly IGenericRepositoryNop<LocalizedPropertyNop> localizedPropertyRepo;
        private readonly IGenericRepository<ProductVariant> productVariantRepo;
        const string cacheProductKey = "productList";

        public ProductService(IUnitOfWork unitOfWork, IUnitOfWorkNop unitOfWorkNop, IAutoMapperConfiguration autoMapper, IHostingEnvironment hostingEnvironment,
            IPictureService pictureService, IConfiguration configuration, IMemoryCache memoryCache,
            ILanguageService languageService, ICategoryService categoryService, IAttributesService attributesService,
            ICacheManager cacheManager, ITaxService taxService)
        {
            this.taxService = taxService;
            this.cacheManager = cacheManager;
            this.attributesService = attributesService;
            this.categoryService = categoryService;
            this.languageService = languageService;
            this.unitOfWorkNop = unitOfWorkNop;
            this.unitOfWork = unitOfWork;
            this.autoMapper = autoMapper;
            this.hostingEnvironment = hostingEnvironment;
            this.pictureService = pictureService;
            this.configuration = configuration;
            this.memoryCache = memoryCache;
            this.taxService = taxService;

            languageValueRepo = this.unitOfWork.Repository<LanguageValues>();
            productGroupRepo = this.unitOfWork.Repository<ProductGroup>();
            languageRepo = this.unitOfWork.Repository<Language>();
            productNopRepo = this.unitOfWorkNop.Repository<ProductNop>();
            localizedPropertyRepo = this.unitOfWorkNop.Repository<LocalizedPropertyNop>();
            productRepo = this.unitOfWork.Repository<Product>();
            productAttributeRepo = this.unitOfWork.Repository<ProductAttributeMapping>();
            parentProductRepo = this.unitOfWork.Repository<ParentProduct>();
            productAttributeValueRepo = this.unitOfWork.Repository<ProductAttributeValue>();
            categoryRepo = this.unitOfWork.Repository<Category>();
            categoryAttributeRepo = this.unitOfWork.Repository<CategoryAttributeMapping>();
            attributesRepo = this.unitOfWork.Repository<Attributes>();
            attributesValueRepo = this.unitOfWork.Repository<AttributesValue>();
            storeProductMappingRepo = this.unitOfWork.Repository<StoreProductMapping>();
            warehouseProductStockRepo = this.unitOfWork.Repository<WarehouseProductStock>();
            productDockRepo = this.unitOfWork.Repository<ProductDock>();
            productDockPicturesRepo = this.unitOfWork.Repository<ProductDockPictures>();
            languageValuesRepo = this.unitOfWork.Repository<LanguageValues>();
            storeRepo = this.unitOfWork.Repository<Store>();
            pictureRepo = this.unitOfWork.Repository<Pictures>();
            storeWarehouseRepo = this.unitOfWork.Repository<StoreWarehouseMapping>();
            productVariantRepo = this.unitOfWork.Repository<ProductVariant>();
        }
        public ICollection<ProductModel> AllProducts()
        {
            if (!memoryCache.TryGetValue(cacheProductKey, out ICollection<ProductModel> allProductModels))
            {
                var allProducts = productRepo.Filter(x => x.State == (int)State.Active, null, "Pictures").
                    Select(x => new Product { Id = x.Id, Sku = x.Sku, Title = x.Title, Pictures = x.Pictures }).ToList();

                allProductModels = autoMapper.MapCollection<Product, ProductModel>(allProducts).ToList();

                var cacheExpirationOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                    Priority = CacheItemPriority.High
                };
                memoryCache.Set(cacheProductKey, allProductModels, cacheExpirationOptions);
            }

            return allProductModels;
        }

        #region CachedDatas Generic Yapılacak
        public ICollection<Product> AllProductWithDetails()
        {
            var products = new List<Product>();

            if (!cacheManager.TryGetValue(CacheStatics.AllProductsWithDetail, out products))
            {
                products = productRepo.Table().Include(x => x.Brand).Include(x => x.Supplier).Include(x => x.ParentProduct)
                                                   .ThenInclude(x => x.Category)
                                                   .Include(x => x.Currency)
                                                   .Include(x => x.ParentProduct.SupplierPP)
                                                   .Include(x => x.ParentProduct.BrandPP)
                                                   .Include(x => x.VatRate)
                                                   .Include(x => x.Pictures).ToList();

                cacheManager.Set(CacheStatics.AllProductsWithDetail, products, CacheStatics.AllProductsWithDetailCacheTime);
            }

            return products;
        }

        public ICollection<ProductAttributeMappingModel> ProductAttributeMapping()
        {
            var productAttributes = new List<ProductAttributeMappingModel>();

            if (!cacheManager.TryGetValue(CacheStatics.ProductAttributeMappingCache, out productAttributes))
            {
                productAttributes = autoMapper.MapCollection<ProductAttributeMapping, ProductAttributeMappingModel>
                                    (productAttributeRepo.Table()
                                    .Include(a => a.Attributes)).ToList();

                cacheManager.Set(CacheStatics.ProductAttributeMappingCache, productAttributes,
                    CacheStatics.ProductAttributeMappingCacheTime);
            }

            return productAttributes;
        }

        public ICollection<AttributesModel> Attributes()
        {
            var Attributes = new List<AttributesModel>();

            if (!cacheManager.TryGetValue(CacheStatics.Attribute, out Attributes))
            {
                Attributes = autoMapper.MapCollection<Attributes, AttributesModel>
                                    (attributesRepo.Table()).ToList();

                cacheManager.Set(CacheStatics.Attribute, Attributes,
                    CacheStatics.AttributeCacheTime);
            }

            return Attributes;
        }

        public ICollection<AttributesValueModel> AttributeValues()
        {
            var attributeValues = new List<AttributesValueModel>();

            if (!cacheManager.TryGetValue(CacheStatics.AttributeValue, out attributeValues))
            {
                attributeValues = autoMapper.MapCollection<AttributesValue, AttributesValueModel>
                                    (attributesValueRepo.Table()).ToList();

                cacheManager.Set(CacheStatics.AttributeValue, attributeValues,
                    CacheStatics.AttributeValueCacheTime);
            }

            return attributeValues;
        }
        public ICollection<LanguageValuesModel> LanguageValues(int languageId)
        {
            var languageValues = new List<LanguageValuesModel>();

            if (!cacheManager.TryGetValue($"{CacheStatics.LanguageValuesWithTableName}_{languageId}", out languageValues))
            {
                SetLanguageValues(out languageValues, languageId);
            }
            else
            {
                var cachedLanguage = languageValues.FirstOrDefault().LanguageId;
                if (languageId != cachedLanguage)
                    SetLanguageValues(out languageValues, languageId);
            }
            return languageValues;
        }
        private void SetLanguageValues(out List<LanguageValuesModel> languageValues, int languageId)
        {
            languageValues = autoMapper.MapCollection<LanguageValues, LanguageValuesModel>
                                  (languageValuesRepo.Filter(a => a.LanguageId == languageId)).ToList();

            cacheManager.Set($"{CacheStatics.LanguageValuesWithTableName}_{languageId}", languageValues,
                CacheStatics.LanguageValuesWithTableNameCacheTime);
        }

        public ICollection<LanguageModel> Languages()
        {
            var languages = new List<LanguageModel>();

            if (!cacheManager.TryGetValue(CacheStatics.LanguageCache, out languages))
            {
                languages = autoMapper.MapCollection<Language, LanguageModel>
                                    (languageRepo.Table()).ToList();

                cacheManager.Set(CacheStatics.LanguageCache, languages,
                    CacheStatics.LanguageCacheTime);
            }

            return languages;
        }

        public ICollection<ProductAttributeValueModel> ProductAttributeValues(int productId)
        {
            var values = new List<ProductAttributeValueModel>();

            if (!cacheManager.TryGetValue(CacheStatics.ProductAttributeValueCache, out values))
            {
                values = autoMapper.MapCollection<ProductAttributeValue, ProductAttributeValueModel>
                                    (productAttributeValueRepo.Table()).ToList();

                cacheManager.Set(CacheStatics.ProductAttributeValueCache, values,
                    CacheStatics.ProductAttributeValueCacheTime);
            }

            return values.Where(x => x.ProductId == productId).ToList();
        }

        public int ProductCount()
        {
            return productRepo.Count();
        }

        #endregion

        public ICollection<ProductModel> AllProductsWithParentId(int id)
        {
            return autoMapper.MapCollection<Product, ProductModel>(productRepo.Filter(x => x.ParentProductId == id && x.State == (int)State.Active, null, "Pictures")).ToList();
        }

        public ProductModel ProductById(int id)
        {
            return autoMapper.MapObject<Product, ProductModel>(productRepo.GetById(id));
        }

        public ProductModel ProductBySku(string sku,bool isActive = true)
        {
            if (isActive)
                return autoMapper.MapObject<Product, ProductModel>(productRepo.Filter(x => x.Sku == sku && x.State == (int)State.Active,null, "VatRate").FirstOrDefault());      
            else
                return autoMapper.MapObject<Product, ProductModel>(productRepo.Filter(x => x.Sku == sku, null, "VatRate").FirstOrDefault());


        }

        public int ProductStateById(int productId)
        {
            try
            {
                return productRepo.GetById(productId).State.Value;
            }
            catch
            {
                return (int)State.Passive;
            }
        }

        public ProductModel ProductDetailById(int productId, int languageId)
        {
            // Cached Datas
            var product = AllProductWithDetails().Where(a => a.Id == productId).FirstOrDefault();
            var productMappings = ProductAttributeMapping();
            var attributeValues = AttributeValues();
            var languageValues = LanguageValues(languageId);
            var languages = Languages();
            var pattributeValues = ProductAttributeValues(productId);
            var attributes = Attributes();
            // Cache End
            var productDetail = autoMapper.MapObject<Product, ProductModel>(product);

            bool isDefaultLanguage = languages.Any(a => a.IsDefault && a.Id == languageId);

            if (productDetail != null)
            {
                if (!isDefaultLanguage)
                {
                    var now = DateTime.Now;

                    var attrTableName = GetFieldValues.ProductAttributeMappingTable;
                    var attvTableName = GetFieldValues.AttributesValueTable;
                    var catTableName = GetFieldValues.CategoryTable;
                    var productTableName = GetFieldValues.ProductTable;
                    // Category 
                    var category = productDetail.ParentProduct.Category;
                    if (category != null)
                    {
                        var lValue = languageValues.Where(x => x.EntityId == category.Id && x.TableName == catTableName);
                        if (lValue != null)
                        {
                            category.Name = categoryService.CategoryWithParentNamesAndLanguages(category.Id, languageId);
                        }

                    }
                    // Product Attributes Default
                    productDetail.ProductAttributeMappings = productMappings.Where(x => x.ProductId == productId).ToList();
                    // Language Values 
                    var attrMappings = productDetail.ProductAttributeMappings;

                    List<LanguageValuesModel> lValues = new List<LanguageValuesModel>();
                    foreach (var iMap in attrMappings)
                    {
                        LanguageValuesModel newValue = new LanguageValuesModel();
                        if (iMap.AttributeValueId != null)
                        {
                            // attributeValueTable
                            //var attrValue = attributeValues.FirstOrDefault(a => a.Id == iMap.AttributeValueId.Value);
                            // language Value
                            var attrValue = languageValues.Where(a => a.EntityId == iMap.AttributeValueId.Value && a.TableName == attvTableName).FirstOrDefault();

                            if (attrValue != null)
                            {
                                newValue.EntityId = iMap.Id;
                                newValue.FieldName = iMap.Attributes.Name;
                                newValue.LanguageId = languageId;
                                newValue.State = iMap.State;
                                newValue.TableName = attvTableName;
                                newValue.Id = 0;
                                newValue.CreatedDate = now;
                                newValue.Value = attrValue.Value;
                            }
                            else
                            {
                                var default_ = attributeValues.FirstOrDefault(a => a.Id == iMap.AttributeValueId.Value);

                                newValue.Unit = default_.Unit;
                                newValue.EntityId = default_.Id;
                                newValue.FieldName = attributes.FirstOrDefault(x => x.Id == default_.AttributesId) == null ? string.Empty : attributes.FirstOrDefault(x => x.Id == default_.AttributesId).Name;
                                newValue.LanguageId = languageId;
                                newValue.State = iMap.State;
                                newValue.TableName = attvTableName;
                                newValue.Id = 0;
                                newValue.CreatedDate = now;
                                newValue.Value = default_.Value;

                            }
                        }
                        else
                        {
                            // languageValueTable
                            var lValue = languageValues.Where(x => x.EntityId == iMap.Id && x.TableName == attrTableName
                            && x.LanguageId == languageId).FirstOrDefault();
                            if (lValue == null)
                            {
                                newValue.EntityId = iMap.Id;
                                newValue.FieldName = iMap.Attributes.Name;
                                newValue.LanguageId = languageId;
                                newValue.State = iMap.State;
                                newValue.TableName = attrTableName;
                                newValue.Id = 0;
                                newValue.CreatedDate = now;
                                newValue.Value = iMap.RequiredAttributeValue;
                            }
                            else
                            {
                                lValue.Value = lValue.Value ?? iMap.RequiredAttributeValue;
                                newValue = lValue;
                            }
                        }

                        lValues.Add(newValue);
                    }

                    // Product Information Fields
                    var productInformation = languageValues.Where(x => x.EntityId == productId && x.TableName == productTableName);

                    lValues = lValues.Union(productInformation.Where(x => GetFieldValues.ProductFields.Any(k => k == x.FieldName)
                    && x.LanguageId == languageId)).ToList();

                    productDetail.LanguageValues = lValues;
                }
                else
                {
                    productDetail.ProductAttributeValues = pattributeValues.Where(x => x.ProductId == productId).ToList();
                }
            }

            return productDetail;
        }


        public IEnumerable<ProductModel> ProductDetailByStoreWithLanguage(int storeId, int languageId, int? startIndex, int? endIndex)
        {
            var store = storeRepo.Table().FirstOrDefault(x => x.Id == storeId);

            string StoreName = store != null ? store.Name : string.Empty;

            List<ProductModel> products = new List<ProductModel>();
            ICollection<StoreProductMapping> storeProducts;

            if (startIndex.HasValue && endIndex.HasValue)
            {
                storeProducts = storeProductMappingRepo.Filter(a => a.StoreId == storeId && !string.IsNullOrEmpty( a.StoreCategory ) && a.State == (int)State.Active)
                   .Skip(startIndex.Value).Take(endIndex.Value).ToList();

            }
            else
            {
                storeProducts = storeProductMappingRepo.Filter(a => a.StoreId == storeId && !string.IsNullOrEmpty(a.StoreCategory) && a.State == (int)State.Active);
            }


            foreach (var item in storeProducts)
            {
                var model = ProductDetailById(item.ProductId.Value, languageId);

                model.Stock = item.Stock;
                if (item.Price != null)
                    model.Price = item.Price.Value;
                else
                {
                    var price = item.Price;
                    model.Price = price == null ? 0 : price.Value;
                }

                // For Fixing
                model.CatalogCode = item.CatalogCode ?? string.Empty;
                model.IsFixed = item.IsFixed ?? false;
                model.ErpPoint = Convert.ToInt32( item.ErpPoint ?? 0 );
                model.ErpPrice = item.ErpPrice ?? 0;
                model.Point = Convert.ToInt32( item.Point ?? 0 );
                model.IsFixedPoint = item.IsFixedPoint ?? false;

                model.StoreCategory = item.StoreCategory;
                model.StoreProductId = item.StoreProductId;
                products.Add(model);
            }

            products.ForEach(x => x.Store = StoreName);
            return products;
        }
        // mock for testing
        public ProductModel ProductDetailByIdMock(int productId, int languageId,
            List<Product> prodList,
            List<ProductAttributeMappingModel> productMappings,
            List<AttributesValueModel> attributeValues,
            List<ProductAttributeValueModel> _pattributeValues,
            List<AttributesModel> attributes,
            List<LanguageValuesModel> LanguageValues)
        {
            // UnCached Datas
            //var product = AllProductWithDetails().Where(a => a.Id == productId).FirstOrDefault();
            var product = prodList.Where(x => x.Id == productId).FirstOrDefault();
            var languageValues = LanguageValues.Where(x => x.LanguageId == languageId).ToList();
            var languages = Languages();
            var pattributeValues = _pattributeValues.Where(x => x.ProductId == productId).ToList();
            // Cache End
            var productDetail = autoMapper.MapObject<Product, ProductModel>(product);

            bool isDefaultLanguage = languages.Any(a => a.IsDefault && a.Id == languageId);

            if (productDetail != null)
            {
                if (!isDefaultLanguage)
                {
                    var now = DateTime.Now;

                    var attrTableName = GetFieldValues.ProductAttributeMappingTable;
                    var attvTableName = GetFieldValues.AttributesValueTable;
                    var catTableName = GetFieldValues.CategoryTable;
                    var productTableName = GetFieldValues.ProductTable;
                    // Category 
                    var category = productDetail.ParentProduct.Category;
                    if (category != null)
                    {
                        var lValue = languageValues.Where(x => x.EntityId == category.Id && x.TableName == catTableName);
                        if (lValue != null)
                        {
                            category.Name = categoryService.CategoryWithParentNamesAndLanguages(category.Id, languageId);
                        }

                    }
                    // Product Attributes Default
                    productDetail.ProductAttributeMappings = productMappings.Where(x => x.ProductId == productId).ToList();
                    // Language Values 
                    var attrMappings = productDetail.ProductAttributeMappings;

                    List<LanguageValuesModel> lValues = new List<LanguageValuesModel>();
                    foreach (var iMap in attrMappings)
                    {
                        LanguageValuesModel newValue = new LanguageValuesModel();
                        if (iMap.AttributeValueId != null)
                        {
                            // attributeValueTable
                            //var attrValue = attributeValues.FirstOrDefault(a => a.Id == iMap.AttributeValueId.Value);
                            // language Value
                            var attrValue = languageValues.Where(a => a.EntityId == iMap.AttributeValueId.Value && a.TableName == attvTableName).FirstOrDefault();

                            if (attrValue != null)
                            {
                                newValue.EntityId = iMap.Id;
                                newValue.FieldName = iMap.Attributes.Name;
                                newValue.LanguageId = languageId;
                                newValue.State = iMap.State;
                                newValue.TableName = attvTableName;
                                newValue.Id = 0;
                                newValue.CreatedDate = now;
                                newValue.Value = attrValue.Value;
                            }
                            else
                            {
                                var default_ = attributeValues.FirstOrDefault(a => a.Id == iMap.AttributeValueId.Value);

                                newValue.Unit = default_.Unit;
                                newValue.EntityId = default_.Id;
                                newValue.FieldName = attributes.FirstOrDefault(x => x.Id == default_.AttributesId) == null ? string.Empty : attributes.FirstOrDefault(x => x.Id == default_.AttributesId).Name;
                                newValue.LanguageId = languageId;
                                newValue.State = iMap.State;
                                newValue.TableName = attvTableName;
                                newValue.Id = 0;
                                newValue.CreatedDate = now;
                                newValue.Value = default_.Value;

                            }
                        }
                        else
                        {
                            // languageValueTable
                            var lValue = languageValues.Where(x => x.EntityId == iMap.Id && x.TableName == attrTableName
                            && x.LanguageId == languageId).FirstOrDefault();
                            if (lValue == null)
                            {
                                newValue.EntityId = iMap.Id;
                                newValue.FieldName = iMap.Attributes.Name;
                                newValue.LanguageId = languageId;
                                newValue.State = iMap.State;
                                newValue.TableName = attrTableName;
                                newValue.Id = 0;
                                newValue.CreatedDate = now;
                                newValue.Value = iMap.RequiredAttributeValue;
                            }
                            else
                            {
                                lValue.Value = lValue.Value ?? iMap.RequiredAttributeValue;
                                newValue = lValue;
                            }
                        }

                        lValues.Add(newValue);
                    }

                    // Product Information Fields
                    var productInformation = languageValues.Where(x => x.EntityId == productId && x.TableName == productTableName);

                    lValues = lValues.Union(productInformation.Where(x => GetFieldValues.ProductFields.Any(k => k == x.FieldName)
                    && x.LanguageId == languageId)).ToList();

                    productDetail.LanguageValues = lValues;
                }
                else
                {
                    productDetail.ProductAttributeValues = pattributeValues.Where(x => x.ProductId == productId).ToList();
                }
            }

            return productDetail;
        }

        public IEnumerable<ProductModel> ProductDetailByStoreWithLanguageMock(int storeId, int languageId, int? startIndex, int? endIndex)
        {
            var store = storeRepo.Table().FirstOrDefault(x => x.Id == storeId);

            string StoreName = store != null ? store.Name : string.Empty;

            List<ProductModel> products = new List<ProductModel>();
            ICollection<StoreProductMapping> storeProducts;

            if (startIndex.HasValue && endIndex.HasValue)
            {
                storeProducts = storeProductMappingRepo.Filter(a => a.StoreId == storeId)
                   .Skip(startIndex.Value).Take(endIndex.Value).ToList();

            }
            else
            {
                storeProducts = storeProductMappingRepo.Filter(a => a.StoreId == storeId);
            }
            // for mock
            var prodList = productRepo.Table().Include(x => x.ParentProduct)
                                                   .ThenInclude(x => x.Category)
                                                   .Include(x => x.Currency)
                                                   .Include(x => x.ParentProduct.SupplierPP)
                                                   .Include(x => x.ParentProduct.BrandPP)
                                                   .Include(x => x.VatRate)
                                                   .Include(x => x.Pictures).ToList();

            var prodMappings = autoMapper.MapCollection<ProductAttributeMapping, ProductAttributeMappingModel>
                                    (productAttributeRepo.Table()
                                    .Include(a => a.Attributes)).ToList();

            var attributes = autoMapper.MapCollection<AttributesValue, AttributesValueModel>
                                    (attributesValueRepo.Table()).ToList();

            var pattributes = autoMapper.MapCollection<ProductAttributeValue, ProductAttributeValueModel>
                                    (productAttributeValueRepo.Table()).ToList();

            var attr = autoMapper.MapCollection<Attributes, AttributesModel>
                                    (attributesRepo.Table()).ToList();

            var languageValues = autoMapper.MapCollection<LanguageValues, LanguageValuesModel>
                                   (languageValuesRepo.Filter(a => a.LanguageId == languageId)).ToList();

            foreach (var item in storeProducts)
            {
                var model = ProductDetailByIdMock(item.ProductId.Value, languageId, prodList, prodMappings, attributes, pattributes, attr, languageValues);

                model.Stock = item.Stock;
                if (item.Price != null)
                    model.Price = item.Price.Value;
                else
                {
                    var price = item.Price;
                    model.Price = price == null ? 0 : price.Value;
                }
                model.StoreCategory = item.StoreCategory;
                model.StoreProductId = item.StoreProductId;
                products.Add(model);
            }

            products.ForEach(x => x.Store = StoreName);
            return products;
        }


        public IEnumerable<ProductModel> ProductDetailByProductIDsStoreWithLanguage(List<string> SKUs,
            int storeId, int languageId)
        {
            var store = storeRepo.Table().FirstOrDefault(x => x.Id == storeId);

            string StoreName = store != null ? store.Name : string.Empty;

            List<ProductModel> products = new List<ProductModel>();
            ICollection<StoreProductMapping> storeProducts;

            var productIDs = productRepo.Table().Where(x => SKUs.Any(k => x.Sku == k)).ToList();

            storeProducts = storeProductMappingRepo.Filter(a => a.StoreId == storeId).Where(x => productIDs.Any(k => k.Id == x.ProductId)).ToList();


            foreach (var item in storeProducts)
            {
                var model = ProductDetailById(item.ProductId.Value, languageId);

                model.Stock = item.Stock;
                var price = item.Price;
                model.Price = price == null ? 0 : price.Value;
                model.StoreProductId = item.StoreProductId;
                products.Add(model);
            }

            products.ForEach(x => x.Store = StoreName);

            return products;
        }

        public ProductModel AddNewProduct(ProductModel model)
        {
            if (productRepo.Exist(x => x.Sku.ToLowerInvariant() == model.Sku.ToLowerInvariant()))
                return null;

            var entity = autoMapper.MapObject<ProductModel, Product>(model);

            entity.Brand = null;
            entity.Supplier = null;

            var savedEntity = productRepo.Add(entity);

            return autoMapper.MapObject<Product, ProductModel>(savedEntity);

        }

        public ProductModel EditProduct(ProductModel model)
        {
            var entity = autoMapper.MapObject<ProductModel, Product>(model);

            entity.Brand = null;
            entity.Supplier = null;

            var updatedEntity = productRepo.Update(entity);

            return autoMapper.MapObject<Product, ProductModel>(updatedEntity);
        }

        public ICollection<ProductModel> SearchProducts(int categoryId, int brandId, string sku, string title)
        {
            if (!string.IsNullOrWhiteSpace(sku))
                return autoMapper.MapCollection<Product, ProductModel>(productRepo.Filter(x => x.Sku == sku, null, "Pictures,ParentProduct")).ToList();

            else if (categoryId != 0 || brandId != 0)
            {
                var finalCategoryIds = new List<int>();

                if (categoryId != 0)
                {
                    var childCategoryIds = new List<int> { categoryId };
                    finalCategoryIds = finalCategoryIds.Concat(childCategoryIds).ToList();

                    while (childCategoryIds.Count > 0)
                    {
                        childCategoryIds = categoryRepo.Filter(x => childCategoryIds.Contains(x.ParentCategoryId.Value)).Select(x => x.Id).ToList();
                        finalCategoryIds = finalCategoryIds.Concat(childCategoryIds).ToList();
                    }
                }

                var parentEntitiesIds = parentProductRepo.Filter(x => x.BrandId == brandId || finalCategoryIds.Contains(x.CategoryId.Value)).Select(pp => pp.Id).ToList();

                return autoMapper.MapCollection<Product, ProductModel>(productRepo.Filter(x => parentEntitiesIds.Contains(x.ParentProductId.Value), null, "Pictures")).ToList();
            }

            return autoMapper.MapCollection<Product, ProductModel>(productRepo.Filter(x => x.Title.Contains(title), null, "Pictures")).ToList();

        }

        public bool ChangeProductsParent(int parentId, List<int> productIds)
        {
            try
            {
                foreach (var productId in productIds)
                {
                    var product = productRepo.GetById(productId);
                    product.ParentProductId = parentId;
                    productRepo.Update(product);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool EditProductParent(int productId, int parentId)
        {
            var product = productRepo.GetById(productId);

            if (product.ParentProductId.HasValue && product.ParentProductId.Value == parentId)
                return true;

            product.ParentProductId = parentId;

            productRepo.Update(product);

            return true;

        }

        public ICollection<ProductModel> ProductsNotInStore(int storeId)
        {
            var existProductListIds = storeProductMappingRepo.FindAll(x => x.StoreId == storeId && x.State == (int)State.Active).Select(x => x.ProductId).ToList();

            return AllProducts().Where(x => !existProductListIds.Contains(x.Id)).ToList();
        }

        /// <summary>
        /// Deletes all references of Product
        /// </summary>
        /// <param name="productId">Id of the Product</param>
        /// <returns></returns>
        public bool DeleteProduct(int productId, out string exception)
        {
            try
            {
                var product = productRepo.GetById(productId);
                exception = product.Sku;

                //Delete Attributes with Language Values
                var attributesMaps = productAttributeRepo.Filter(x => x.ProductId == productId);
                var deletingAttributes = languageValueRepo.Filter(x => attributesMaps.Select(s => s.Id).Contains(x.EntityId) && x.TableName == GetFieldValues.ProductAttributeMappingTable);

                languageValueRepo.BulkDelete(deletingAttributes.ToList());
                productAttributeRepo.BulkDelete(attributesMaps.ToList());

                //Delete Language Values
                languageValueRepo.BulkDelete(languageValueRepo.Filter(x => x.EntityId == productId && x.TableName == GetFieldValues.ProductTable).ToList());

                //Deleting Pictures
                var pictures = pictureRepo.Filter(x => x.ProductId == productId);
                foreach (var picture in pictures)
                {
                    ImageOperations.DeleteFileInServer(hostingEnvironment, picture.LocalPath);
                    ImageOperations.DeleteFileInCdn($"{configuration.GetValue<string>("PicturePaths:CdnPath")}{picture.CdnPath.Replace(configuration.GetValue<string>("PicturePaths:CdnUrlPath"), "")}", configuration.GetValue<string>("PicturePaths:CdnUserName"), configuration.GetValue<string>("PicturePaths:CdnPassword"));
                }
                pictureRepo.BulkDelete(pictures.ToList());

                //Delete From Stores
                var deletingStoreProductMappings = storeProductMappingRepo.Filter(x => x.ProductId == productId);
                storeProductMappingRepo.BulkDelete(deletingStoreProductMappings.ToList());

                //Delete From Warehouses
                var deletingWarehouseProductMappings = warehouseProductStockRepo.Filter(x => x.ProductId == productId);
                warehouseProductStockRepo.BulkDelete(deletingWarehouseProductMappings.ToList());

                //Delete From Variant
                var deletingVariant = productVariantRepo.Filter(x => x.ProductId == productId || x.BaseId == productId);
                productVariantRepo.BulkDelete(deletingVariant.ToList());

                //Finally Delete Product
                product.State = (int)State.Delete;
                productRepo.Update(product);

                return true;
            }
            catch (Exception ex)
            {
                exception = ex.Message;
                return false;
            }
        }

        public bool SetStateProduct(int productId, int state, out string exception)
        {
            
            try
            {
                var product = productRepo.GetById(productId);
                exception = product.Sku;

                //Change State Attributes with Language Values
                
                var attributesMaps = productAttributeRepo.Filter(x => x.ProductId == productId);
                var statingAttributes = languageValueRepo.Filter(x => attributesMaps.Select(s => s.Id).Contains(x.EntityId) && x.TableName == GetFieldValues.ProductAttributeMappingTable);

                statingAttributes = statingAttributes.Select(c => { c.State = state; return c; }).ToList();
                attributesMaps = attributesMaps.Select(c => { c.State = state; return c; }).ToList();

                languageValueRepo.BulkUpdate(statingAttributes.ToList());
                Thread.Sleep(1000);
                productAttributeRepo.BulkUpdate(attributesMaps.ToList());

                //Change State Language Values
                
                var languageValues = languageValueRepo.Filter(x => x.EntityId == productId && x.TableName == GetFieldValues.ProductTable);
                languageValues = languageValues.Select(c => { c.State = state; return c; }).ToList();

                languageValueRepo.BulkUpdate(languageValues.ToList());
                Thread.Sleep(1000);

                //Change State (Delete) From Stores

                var deletingStoreProductMappings = storeProductMappingRepo.Filter(x => x.ProductId == productId);
                deletingStoreProductMappings = deletingStoreProductMappings.Select(c => { c.State = state; return c; }).ToList();

                storeProductMappingRepo.BulkUpdate(deletingStoreProductMappings.ToList());
                Thread.Sleep(1000);

                //Delete From Variant
                var deletingVariant = productVariantRepo.Filter(x => x.ProductId == productId || x.BaseId == productId);
                deletingVariant = deletingVariant.Select(c => { c.State = state; return c; }).ToList();
                productVariantRepo.BulkUpdate(deletingVariant.ToList());
                Thread.Sleep(1000);

                //Finally Change State of the Product
                product.State = state;
                productRepo.Update(product);

                return true;
            }
            catch (Exception ex)
            {
                exception = ex.Message;
                return false;
            }

        }

        #region Product Attributes

        public ICollection<ProductAttributeMappingModel> ProductAttributesById(int id)
        {
            var productAttributeMappingList = productAttributeRepo.Filter(x => x.ProductId == id && x.State == (int)State.Active,
                null, "Attributes").ToList();

            return autoMapper.MapCollection<ProductAttributeMapping, ProductAttributeMappingModel>(productAttributeMappingList).ToList();
        }

        public ProductAttributeMappingModel ProductAttributeMappingById(int id)
        {
            return autoMapper.MapObject<ProductAttributeMapping, ProductAttributeMappingModel>(productAttributeRepo.Filter(x => x.Id == id, null, "Attributes").FirstOrDefault());
        }

        public ICollection<ProductAttributeMappingModel> ArrangeCategoryProductAttributes(int parentProductId, int? productId)
        {
            var productAttributeMappingList = new List<ProductAttributeMapping>();
            var categoryAttributesMappingList = new List<CategoryAttributeMapping>();
            var categoryId = parentProductRepo.GetById(parentProductId).CategoryId;

            if (productId.HasValue)
                productAttributeMappingList = productAttributeRepo.FindAll(x => x.ProductId == productId.Value && x.RequiredAttributeValue == null).ToList();

            #region Get Category Attributes With Parents Included

            var categoryWithParents = new List<Category>();

            var category = categoryRepo.GetById(categoryId.Value);

            while (category.ParentCategoryId.HasValue)
            {
                categoryWithParents.Add(category);
                category = categoryRepo.GetById(category.ParentCategoryId.Value);
            }

            categoryWithParents.Add(category); //Main category

            var categoryIds = categoryWithParents.Select(p => p.Id).ToList();
            categoryAttributesMappingList = categoryAttributeRepo.Filter(x => categoryIds.Contains(x.CategoryId.Value), null, "Attributes").ToList();

            #endregion

            #region Arrange Product Attributes Via Category Attributes

            //Product Attributes are empty initial creation
            if (productAttributeMappingList == null || productAttributeMappingList.Count == 0)
            {
                foreach (var categoryAttributesMapping in categoryAttributesMappingList)
                {
                    productAttributeMappingList.Add(new ProductAttributeMapping
                    {
                        ProductId = productId ?? 0,
                        Attributes = categoryAttributesMapping.Attributes,
                        AttributesId = categoryAttributesMapping.AttributesId,
                        IsRequired = categoryAttributesMapping.IsRequired
                    });
                }
            }
            //Product Attributes exists checks changes on Category Attributes. If exists update mandotary, if not add.
            else
            {
                foreach (var categoryAttributesMapping in categoryAttributesMappingList)
                {
                    if (productAttributeMappingList.Select(x => x.AttributesId).Contains(categoryAttributesMapping.AttributesId))
                        productAttributeMappingList.FirstOrDefault(x => x.AttributesId == categoryAttributesMapping.AttributesId).IsRequired = categoryAttributesMapping.IsRequired;
                    else
                    {
                        productAttributeMappingList.Add(new ProductAttributeMapping
                        {
                            ProductId = productId ?? 0,
                            Attributes = categoryAttributesMapping.Attributes,
                            AttributesId = categoryAttributesMapping.AttributesId,
                            IsRequired = categoryAttributesMapping.IsRequired
                        });
                    }
                }
            }

            //Attributes are deleted from Category Attributes but still exists on Product Attributes. If productId is null this control is not necessary.
            if (productId.HasValue)
            {
                var categoryAttributesAttributeIds = categoryAttributesMappingList.Select(x => x.AttributesId).ToList();

                foreach (var productAttributeMapping in productAttributeMappingList)
                {
                    if (!categoryAttributesAttributeIds.Contains(productAttributeMapping.AttributesId))
                    {
                        productAttributeMapping.IsRequired = false;
                        productAttributeMapping.Attributes = attributesRepo.GetById(productAttributeMapping.AttributesId.Value);
                    }
                }

            }

            #endregion

            foreach (var productAttributeMap in productAttributeMappingList)
                productAttributeMap.Attributes.AttributesValues = attributesValueRepo.Filter(x => x.AttributesId == productAttributeMap.AttributesId).ToList();

            return autoMapper.MapCollection<ProductAttributeMapping, ProductAttributeMappingModel>(productAttributeMappingList).ToList();
        }

        public ICollection<ProductAttributeMappingModel> ArrangeRequiredProductAttributes(int? productId)
        {
            var requiredAttributeList = attributesRepo.FindAll(x => x.IsRequired && x.State == (int)State.Active).ToList();
            var productAttributeMappingList = new List<ProductAttributeMapping>();

            if (productId.HasValue)
                productAttributeMappingList = productAttributeRepo.Filter(x => x.ProductId == productId && x.State == (int)State.Active && x.RequiredAttributeValue != null, null, "Attributes").ToList();

            if (productAttributeMappingList.Count == 0)
            {
                foreach (var attributes in requiredAttributeList)
                {
                    productAttributeMappingList.Add(new ProductAttributeMapping
                    {
                        ProductId = 0,
                        Attributes = attributes,
                        AttributesId = attributes.Id,
                        IsRequired = attributes.IsRequired
                    });
                }
            }
            else
            {
                foreach (var requiredAttribute in requiredAttributeList)
                {
                    if (!productAttributeMappingList.Select(x => x.AttributesId).ToList().Contains(requiredAttribute.Id))
                    {
                        productAttributeMappingList.Add(new ProductAttributeMapping
                        {
                            ProductId = productId,
                            AttributesId = requiredAttribute.Id,
                            Attributes = attributesRepo.GetById(requiredAttribute.Id),
                            AttributeValueId = null,
                            IsRequired = requiredAttribute.IsRequired
                        });
                    }
                }
            }

            return autoMapper.MapCollection<ProductAttributeMapping, ProductAttributeMappingModel>(productAttributeMappingList).ToList();
        }

        public ProductAttributeMappingModel AddNewProductAttributeMapping(int productId, int attributeId, int? attributeValueId, bool isRequired, string requiredAttributeValue = null)
        {
            var productAttribute = new ProductAttributeMapping
            {
                ProductId = productId,
                AttributesId = attributeId,
                AttributeValueId = attributeValueId,
                IsRequired = isRequired,
                RequiredAttributeValue = requiredAttributeValue
            };

            return autoMapper.MapObject<ProductAttributeMapping, ProductAttributeMappingModel>(productAttributeRepo.Add(productAttribute));
        }

        public int AddRangeProductAttributeMapping(List<ProductAttributeMappingModel> productAttributeMappingsModel, int entityId)
        {
            if (productAttributeMappingsModel == null || productAttributeMappingsModel.Count == 0)
                return -1;

            productAttributeMappingsModel.RemoveAll(x => x.AttributeValueId == null);

            foreach (var productAttributeMapping in productAttributeMappingsModel)
            {
                productAttributeMapping.ProductId = entityId;
                productAttributeMapping.Attributes = null; // Addrange ve unitofwork için burada attribute doluysa attribute tablosuna da yazıyor.
            }

            var entities = autoMapper.MapCollection<ProductAttributeMappingModel, ProductAttributeMapping>(productAttributeMappingsModel).ToList();

            return productAttributeRepo.AddRange(entities);
        }

        public bool EditProductAttributeMapping(List<ProductAttributeMappingModel> productAttributeMappingModels, int entityId)
        {
            var productAttributeMappings = autoMapper.MapCollection<ProductAttributeMappingModel, ProductAttributeMapping>(productAttributeMappingModels);

            foreach (var productAttributeMapping in productAttributeMappings)
            {
                productAttributeMapping.ProductId = entityId;
                productAttributeMapping.Attributes = null;
            }

            if (productAttributeMappings.Any(x => x.Id == 0))
                productAttributeRepo.AddRange(productAttributeMappings.Where(x => x.Id == 0).ToList());

            return productAttributeRepo.UpdateRange(productAttributeMappings.Where(x => x.Id != 0).ToList()) == -1 ? false : true;
        }

        public int DeleteProductAttributeMapping(ProductAttributeMappingModel productAttributeMappingModel)
        {
            var entity = autoMapper.MapObject<ProductAttributeMappingModel, ProductAttributeMapping>(productAttributeMappingModel);

            return productAttributeRepo.Delete(entity);
        }

        public int DeleteProductAttributeMappingWithIdReturnProductId(int productAttributeMappingId)
        {
            var entity = productAttributeRepo.GetById(productAttributeMappingId);

            var productId = entity.ProductId;

            productAttributeRepo.Delete(entity);

            return productId ?? 0;
        }

        #endregion

        #region Product Attributes Value

        public int ArrangeProductAttributeValue(int productId)
        {
            var productAttributeMappings = productAttributeRepo.Filter(x => x.ProductId == productId).ToList();

            var productAttributeValueList = productAttributeValueRepo.Filter(x => x.ProductId == productId).ToList();

            if (productAttributeValueList.Count > 0)
            {
                productAttributeValueRepo.DeleteRange(productAttributeValueList); //Silmek update etmekten kolay. Attribute name bazlı olacak sıkıntı çıkarır.
                productAttributeValueList.Clear();
            }
            foreach (var productAttribute in productAttributeMappings)
            {
                var productAttributeValue = new ProductAttributeValue()
                {
                    ProductId = productId,
                    Attribute = attributesRepo.GetById(productAttribute.AttributesId.Value).Name,
                    AttributeValueId = productAttribute.AttributeValueId
                };

                if (productAttribute.AttributeValueId.HasValue)
                {
                    var attributeValue = attributesValueRepo.GetById(productAttribute.AttributeValueId.Value);
                    productAttributeValue.AttributeValue = attributeValue.Value;
                    productAttributeValue.Unit = attributeValue.Unit;
                }
                else if (!string.IsNullOrWhiteSpace(productAttribute.RequiredAttributeValue))
                    productAttributeValue.AttributeValue = productAttribute.RequiredAttributeValue;
                else
                    continue;

                productAttributeValueList.Add(productAttributeValue);
            }

            return productAttributeValueRepo.AddRange(productAttributeValueList);
        }

        public int DeleteProductAttributeValue(int productId, string attributeName)
        {
            try
            {
                var entity = productAttributeValueRepo.Filter(x => x.ProductId == productId && x.Attribute == attributeName).FirstOrDefault();

                return productAttributeValueRepo.Delete(entity);
            }
            catch
            {
                return -1;
            }

        }

        #endregion

        #region ProductDock

        public void UpdateProductImport(string path, int languageId)
        {
            string[] readLines = File.ReadAllLines(@"C:\SkuList.txt");
            var skuList = readLines.ToList();

            var nopProducts = productNopRepo.Filter(x => skuList.Contains(x.Sku), o => o.OrderBy(p => p.Sku)).ToList();
            var nopLanguageId = languageId;

            if (languageId == 1)
                nopLanguageId = 2;
            else if (languageId == 2)
                nopLanguageId = 1;


            foreach (var nopProduct in nopProducts)
            {
                List<ProductAttributeMapping> attrList = new List<ProductAttributeMapping>();
                List<LanguageValues> lValues = new List<LanguageValues>();
                var nopLanguageValues = localizedPropertyRepo.Filter(x => x.EntityId == nopProduct.Id && x.LocaleKeyGroup == "Product" && x.LanguageId == nopLanguageId).ToList();
                var product = ProductBySku(nopProduct.Sku);
                //var productAttributes = productAttributeRepo.Filter(a => a.ProductId == product.Id && attList.Any(x => x == a.Id));

                product.Title = nopLanguageValues.FirstOrDefault(a => a.LocaleKey == "Name").LocaleValue;
                product.ShortDescription = nopLanguageValues.FirstOrDefault(a => a.LocaleKey == "ShortDescription").LocaleValue;
                product.Description = nopLanguageValues.FirstOrDefault(a => a.LocaleKey == "FullDescription").LocaleValue;

                productRepo.Update(autoMapper.MapObject<ProductModel, Product>(product));

                var attrBullet1 = productAttributeRepo.Filter(a => a.ProductId == product.Id && a.AttributesId.Value == 2).FirstOrDefault();
                var attrBullet2 = productAttributeRepo.Filter(a => a.ProductId == product.Id && a.AttributesId.Value == 4).FirstOrDefault();
                var attrBullet3 = productAttributeRepo.Filter(a => a.ProductId == product.Id && a.AttributesId.Value == 8).FirstOrDefault();
                var attrBullet4 = productAttributeRepo.Filter(a => a.ProductId == product.Id && a.AttributesId.Value == 9).FirstOrDefault();
                var attrBullet5 = productAttributeRepo.Filter(a => a.ProductId == product.Id && a.AttributesId.Value == 10).FirstOrDefault();

                var attrBullet1Value = nopLanguageValues.FirstOrDefault(a => a.LocaleKey == "BulletPoint1").LocaleValue;
                var attrBullet2Value = nopLanguageValues.FirstOrDefault(a => a.LocaleKey == "BulletPoint2").LocaleValue;
                var attrBullet3Value = nopLanguageValues.FirstOrDefault(a => a.LocaleKey == "BulletPoint3").LocaleValue;
                var attrBullet4Value = nopLanguageValues.FirstOrDefault(a => a.LocaleKey == "BulletPoint4").LocaleValue;
                var attrBullet5Value = nopLanguageValues.FirstOrDefault(a => a.LocaleKey == "BulletPoint5").LocaleValue;

                // if language is turkish
                #region TurkishLanguage
                if (languageId == 1)
                {
                    attrBullet1.RequiredAttributeValue = attrBullet1Value;

                    attrList.Add(attrBullet1);

                    attrBullet2.RequiredAttributeValue = attrBullet2Value;

                    attrList.Add(attrBullet2);

                    attrBullet3.RequiredAttributeValue = attrBullet3Value;

                    attrList.Add(attrBullet3);

                    attrBullet4.RequiredAttributeValue = attrBullet4Value;

                    attrList.Add(attrBullet4);

                    attrBullet5.RequiredAttributeValue = attrBullet5Value;

                    attrList.Add(attrBullet5);

                    productAttributeRepo.UpdateRange(attrList);
                }
                #endregion
                else
                {
                    #region AnotherLanguages

                    var lBulletPoint1 = autoMapper.MapObject<LanguageValuesModel, LanguageValues>(languageService.LanguageValuesByEntityIdAndTableName(attrBullet1.Id,
                        "ProductAttributeMapping").FirstOrDefault(a => a.FieldName == "BulletPoint1"
                    && a.LanguageId == languageId));

                    lBulletPoint1.Value = attrBullet1Value;
                    lValues.Add(lBulletPoint1);

                    var lBulletPoint2 = autoMapper.MapObject<LanguageValuesModel, LanguageValues>(languageService.LanguageValuesByEntityIdAndTableName(attrBullet2.Id,
                        "ProductAttributeMapping").FirstOrDefault(a => a.FieldName == "BulletPoint2"
                    && a.LanguageId == languageId));

                    lBulletPoint2.Value = attrBullet2Value;
                    lValues.Add(lBulletPoint2);

                    var lBulletPoint3 = autoMapper.MapObject<LanguageValuesModel, LanguageValues>(languageService.LanguageValuesByEntityIdAndTableName(attrBullet3.Id,
                        "ProductAttributeMapping").FirstOrDefault(a => a.FieldName == "BulletPoint3"
                    && a.LanguageId == languageId));

                    lBulletPoint3.Value = attrBullet3Value;
                    lValues.Add(lBulletPoint3);

                    var lBulletPoint4 = autoMapper.MapObject<LanguageValuesModel, LanguageValues>(languageService.LanguageValuesByEntityIdAndTableName(attrBullet4.Id,
                        "ProductAttributeMapping").FirstOrDefault(a => a.FieldName == "BulletPoint4"
                    && a.LanguageId == languageId));

                    lBulletPoint4.Value = attrBullet4Value;
                    lValues.Add(lBulletPoint4);

                    var lBulletPoint5 = autoMapper.MapObject<LanguageValuesModel, LanguageValues>(languageService.LanguageValuesByEntityIdAndTableName(attrBullet5.Id,
                        "ProductAttributeMapping").FirstOrDefault(a => a.FieldName == "BulletPoint5"
                     && a.LanguageId == languageId));

                    lBulletPoint5.Value = attrBullet5Value;
                    lValues.Add(lBulletPoint5);

                    languageValueRepo.UpdateRange(lValues);

                    #endregion
                }
            }

        }

        public bool DeleteProductDock(int id)
        {
            productDockPicturesRepo.DeleteRange(productDockPicturesRepo.FindAll(x => x.ProductDockId == id).ToList());

            var productDock = productDockRepo.GetById(id);

            var returnValue = productDockRepo.Delete(productDock);

            return returnValue > 0 ? true : false;
        }

        public bool AddNewProductByProductDock(ProductDockModel productDockModel, int? parentId)
        {
            bool flag = false;

            if (productDockModel == null)
                return false;

            Product product = new Product()
            {
                Sku = productDockModel.Sku,
                Barcode = productDockModel.Gtin,
                Name = productDockModel.Name,
                Title = productDockModel.Name,
                Model = productDockModel.Model,
                Description = productDockModel.FullDescription,
                ShortDescription = productDockModel.ShortDescription,
                Desi = productDockModel.Desi,
                BuyingPrice = productDockModel.Cost,
                ParentProductId = parentId,
                PsfPrice = productDockModel.PsfPrice,
                CorporatePrice = productDockModel.CorporatePrice,
                VatRateId = productDockModel.VatRateId,
                CurrencyId = productDockModel.CurrencyId
            };

            try
            {
                var newProduct = productRepo.Add(product);

                warehouseProductStockRepo.Add(new WarehouseProductStock()
                {
                    Sku = newProduct.Sku,
                    Name = newProduct.Title,
                    ProductId = newProduct.Id,
                    WarehouseTypeId = 1,
                    Quantity = 0
                });

                var requiredAttributeList = attributesRepo.FindAll(x => x.IsRequired).ToList();

                flag = CreateRequiredAttributesValue(product.Id, productDockModel);

                flag = DownloadPictures(newProduct.Id, newProduct.Sku, productDockPicturesRepo.Filter(x => x.ProductDockId == productDockModel.Id).Select(s => s.DownloadUrl).ToList());

                if (flag)
                {
                    productDockPicturesRepo.DeleteRange(productDockPicturesRepo.FindAll(x => x.ProductDockId == productDockModel.Id).ToList());

                    var returnValue = productDockRepo.Delete(autoMapper.MapObject<ProductDockModel, ProductDock>(productDockModel));

                    return returnValue > 0 ? true : false;
                }

                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool CreateRequiredAttributesValue(int product_id, ProductDockModel product)
        {
            try
            {
                List<ProductAttributeMapping> productAttributeMapping = new List<ProductAttributeMapping>();

                var bulletPointClearedList = new List<int> { 11, 12, 13, 14, 15 };

                //TODO: Bu ne amk? Düzelt burayı

                productAttributeMapping.Add(AddAttributeToAttributeValueList(2, product.BulletPoint1, product_id, bulletPointClearedList));
                productAttributeMapping.Add(AddAttributeToAttributeValueList(4, product.BulletPoint2, product_id, bulletPointClearedList));
                productAttributeMapping.Add(AddAttributeToAttributeValueList(8, product.BulletPoint3, product_id, bulletPointClearedList));
                productAttributeMapping.Add(AddAttributeToAttributeValueList(9, product.BulletPoint4, product_id, bulletPointClearedList));
                productAttributeMapping.Add(AddAttributeToAttributeValueList(10, product.BulletPoint5, product_id, bulletPointClearedList));
                productAttributeMapping.Add(AddAttributeToAttributeValueList(11, product.Weight.ToString(), product_id, bulletPointClearedList));
                productAttributeMapping.Add(AddAttributeToAttributeValueList(12, product.Weight.ToString(), product_id, bulletPointClearedList));
                productAttributeMapping.Add(AddAttributeToAttributeValueList(13, product.Length.ToString(), product_id, bulletPointClearedList));
                productAttributeMapping.Add(AddAttributeToAttributeValueList(14, product.Width.ToString(), product_id, bulletPointClearedList));
                productAttributeMapping.Add(AddAttributeToAttributeValueList(15, product.Height.ToString(), product_id, bulletPointClearedList));

                var clearProductAttributeMapping = productAttributeMapping.Where(x => x.ProductId != 0).ToList();


                foreach (var item in clearProductAttributeMapping)
                {

                    var paMapping = productAttributeRepo.Add(item);

                    productAttributeValueRepo.Add(new ProductAttributeValue
                    {
                        ProductId = product_id,
                        Attribute = attributesRepo.GetById(paMapping.AttributesId.Value).Name,
                        AttributeValue = paMapping.RequiredAttributeValue
                    });

                    if (bulletPointClearedList.Contains(paMapping.AttributesId.Value))
                        continue;

                    //var productDockLanguageList = productDockLanguagesRepo.FindAll(x => x.ProductDockId == product.Id && x.LanguageId != 1);

                    //foreach (var productDockLanguage in productDockLanguageList)
                    //{
                    //    if (GetFieldValues.ProductDockFields.Contains(productDockLanguage.FieldName))
                    //    {
                    //        languageValuesRepo.Add(new LanguageValues
                    //        {
                    //            EntityId = product_id,
                    //            TableName = GetFieldValues.ProductDockTable,
                    //            //FieldName = productDockLanguage.FieldName == "FullDescription" ? "Description" : productDockLanguage.FieldName,
                    //            LanguageId = productDockLanguage.LanguageId,
                    //            Value = productDockLanguage.FieldValue
                    //        });
                    //    }
                    //    else if (GetFieldValues.ProductDockBulletFields.Contains(productDockLanguage.FieldName))
                    //    {
                    //        languageValuesRepo.Add(new LanguageValues
                    //        {
                    //            EntityId = paMapping.Id,
                    //            TableName = GetFieldValues.ProductDockAttributeTable,
                    //            FieldName = attributesRepo.GetById(paMapping.AttributesId.Value).Name,
                    //            LanguageId = productDockLanguage.LanguageId,
                    //            Value = productDockLanguage.FieldValue
                    //        });
                    //    }
                    //}
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        private ProductAttributeMapping AddAttributeToAttributeValueList(int attributeId, string value, int productId, List<int> bulletPointList)
        {
            if (string.IsNullOrWhiteSpace(value) && !bulletPointList.Contains(attributeId))
                return new ProductAttributeMapping() { ProductId = 0 };

            return new ProductAttributeMapping()
            {
                ProductId = productId,
                AttributesId = attributeId,
                AttributeValueId = null,
                RequiredAttributeValue = value ?? "-",
                IsRequired = true
            };
        }

        private bool DownloadPictures(int productId, string sku, ICollection<string> downloadUrls)
        {
            try
            {
                foreach (var url in downloadUrls)
                {
                    var desiredName = ArrangePictureName(sku, url.Substring(url.LastIndexOf(".")));

                    var downloadedPictureUrl = ImageOperations.DownLoadFile(hostingEnvironment, sku, configuration.GetValue<string>("PicturePaths:LocalPath"), url, desiredName);

                    if (!string.IsNullOrWhiteSpace(downloadedPictureUrl))
                    {

                        ImageOperations.UploadImageToCdn(configuration.GetValue<string>("PicturePaths:CdnPath"),
                            configuration.GetValue<string>("PicturePaths:CdnUserName"),
                            configuration.GetValue<string>("PicturePaths:CdnPassword"), sku, desiredName,
                            $"{hostingEnvironment.WebRootPath}{downloadedPictureUrl}");

                        pictureService.AddNewPictures(new PicturesModel
                        {
                            ProductId = productId,
                            Order = 0,
                            LocalPath = downloadedPictureUrl,
                            CdnPath = $"{configuration.GetValue<string>("PicturePaths:CdnUrlPath")}{downloadedPictureUrl.Replace(configuration.GetValue<string>("PicturePaths:LocalPath"), "/")}"
                        });
                    }
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        private string ArrangePictureName(string sku, string extension)
        {
            try
            {
                var productName = ProductBySku(sku).Barcode;
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

        public bool AddNewProductByProductDock(ProductDockModel productDockModel, string pictureName, int parentId)
        {
            throw new NotImplementedException();
        }

        #endregion

        //sorguların hepsini düzeltmen lazım
        #region Integration

        public List<string> GetProductSkubyId(List<int> productId)
        {
            return productRepo.Table().Where(x => productId.Any(y => y == x.Id)).Select(x => x.Sku).ToList();
        }

        public List<ChannelAdvisorProductViewModel> JsonchannelAdvisorProductVMs(int store_id, List<ProductJsonModel> productJsonModels, List<ProductStockAndPriceJsonModel> productStockAndPriceJsonModels)
        {
            List<ChannelAdvisorProductViewModel> channelAdvisorProductViewModels = new List<ChannelAdvisorProductViewModel>();

            var storeProduct = storeProductMappingRepo.Table()
                .Where(x => x.StoreId == store_id && x.StoreParentProductId == 0
                && x.StoreProductId == 0).ToList();

            var subStoreId = storeProductMappingRepo.Table().Where(x => x.BaseStoreId == store_id).ToList();

            foreach (var item in productJsonModels.Where(x=>x.StoreProductId==0))
            {
                var productQuery = new ChannelAdvisorProductModel
                {
                    Sku = item.Sku,
                    Title = item.Title,
                    Brand = item.Brand,
                    Classification = item.Category.Split(">>").Last(),
                    Description = item.Description,
                    EAN = item.Barcode,
                    Manufacturer = item.Supplier,
                    ShortDescription = item.ShortDescription,
                    BuyItNowPrice = productStockAndPriceJsonModels.FirstOrDefault(x => x.Sku == item.Sku).Price
                };
                List<Attribute> attributes = new List<Attribute>();
                foreach (var attribute in item.Attributes)
                {
                    attributes.Add(new Attribute()
                    {
                        Name = attribute.FieldName.ToUpperInvariant() + "NEW",
                        Value = attribute.Value
                    });
                }

                var browseNode = storeProduct.Where(x => x.ProductId == item.Id).Select(x => new Attribute()
                {
                    Name = x.StoreId + "-Category",
                    Value = x.StoreCategory
                }).ToList();

                var subCategory = subStoreId.Where(x => x.ProductId == item.Id).Select(x => new Attribute()
                {
                    Name = x.StoreId + "-Category",
                    Value = x.StoreCategory
                }).ToList();

                attributes.AddRange(browseNode);
                attributes.AddRange(subCategory);

                productQuery.Attributes = attributes;

                var labelQuery = new Label() { Name = storeRepo.Find(x => x.Id == store_id).Name };

                var imageQuery = item.Pictures
                    .Select(x => new Image()
                    {
                        PlacementName = "ITEMIMAGEURL",
                        UrlPath = new UrlPath() { Url = x.CdnPath }
                    }).ToList();

                var dCQuantitysQuery = new DCQuantity()
                {
                    Value = new Value
                    {
                        UpdateType = "InStock",
                        Updates = new List<Update>()
                        {
                               new Update()
                            {
                                Quantity=productStockAndPriceJsonModels.FirstOrDefault(x=>x.Sku==item.Sku).Stock
                            }
                        }
                    }
                };


                channelAdvisorProductViewModels.Add(new ChannelAdvisorProductViewModel()
                {
                    channelAdvisorProductModel = productQuery,
                    labels = labelQuery,
                    Images = imageQuery,
                    DCQuantitys = dCQuantitysQuery

                });

                if (item.ProductVariants!=null)
                {
                    foreach (var productVariant in item.ProductVariants.Skip(1))
                    {
                        var productQueryVar = new ChannelAdvisorProductModel
                        {
                            Sku = productVariant.Sku,
                            Title = productVariant.Title,
                            Brand = productVariant.Brand,
                            Classification = productVariant.Category.Split(">>").Last(),
                            Description = productVariant.Description,
                            EAN = productVariant.Barcode,
                            Manufacturer = productVariant.Supplier,
                            ShortDescription = productVariant.ShortDescription,
                            BuyItNowPrice = productStockAndPriceJsonModels.FirstOrDefault(x => x.Sku == productVariant.Sku).Price
                        };
                        List<Attribute> attributesVar = new List<Attribute>();
                        foreach (var attribute in productVariant.Attributes)
                        {
                            attributesVar.Add(new Attribute()
                            {
                                Name = attribute.FieldName.ToUpperInvariant() + "NEW",
                                Value = attribute.Value
                            });
                        }

                        var browseNodeVar = storeProduct.Where(x => x.ProductId == productVariant.Id).Select(x => new Attribute()
                        {
                            Name = x.StoreId + "-Category",
                            Value = x.StoreCategory
                        }).ToList();

                        var subCategoryVar = subStoreId.Where(x => x.ProductId == productVariant.Id).Select(x => new Attribute()
                        {
                            Name = x.StoreId + "-Category",
                            Value = x.StoreCategory
                        }).ToList();

                        attributes.AddRange(browseNodeVar);
                        attributes.AddRange(subCategoryVar);

                        productQueryVar.Attributes = attributes;

                        var labelQueryVar = new Label() { Name = storeRepo.Find(x => x.Id == store_id).Name };

                        var imageQueryVar = productVariant.Pictures
                            .Select(x => new Image()
                            {
                                PlacementName = "ITEMIMAGEURL",
                                UrlPath = new UrlPath() { Url = x.CdnPath }
                            }).ToList();

                        var dCQuantitysQueryVar = new DCQuantity()
                        {
                            Value = new Value
                            {
                                UpdateType = "InStock",
                                Updates = new List<Update>()
                        {
                               new Update()
                            {
                                Quantity=productStockAndPriceJsonModels.FirstOrDefault(x=>x.Sku==productVariant.Sku).Stock
                            }
                        }
                            }
                        };


                        channelAdvisorProductViewModels.Add(new ChannelAdvisorProductViewModel()
                        {
                            channelAdvisorProductModel = productQueryVar,
                            labels = labelQueryVar,
                            Images = imageQueryVar,
                            DCQuantitys = dCQuantitysQueryVar

                        });

                    }
                }
            }

            return channelAdvisorProductViewModels;
        }

        public List<ChannelAdvisorProductViewModel> channelAdvisorProductVMParent(int store_id, int language_id, string parentSku, int storeParentId, int take, int skip)
        {
            List<ChannelAdvisorProductViewModel> channelAdvisorProductViewModels = new List<ChannelAdvisorProductViewModel>();

            var parentProductId = parentProductRepo.Table().FirstOrDefault(x => x.Sku == parentSku).Id;

            var storeProduct = storeProductMappingRepo.Table().Where(x => x.StoreId == store_id && x.Product.ParentProductId == parentProductId).Skip(skip).Take(take);

            var subStoreId = storeProductMappingRepo.Table().Where(x => x.BaseStoreId == store_id).ToList();

            var productAttributeMapping = productAttributeRepo.Table()
                .Where(x => storeProduct.Any(y => y.Product.Id == x.ProductId)).Include(x => x.Attributes).Include(x => x.AttributesValue).ToList();

            var productAttributeValues = productAttributeValueRepo.Table()
            .Where(x => storeProduct.Any(y => y.ProductId == x.ProductId) && x.AttributeValueId == null).ToList();

            var categoryAttributeMapping = categoryAttributeRepo.Table().Where(x => !x.IsRequired).ToList();

            var languageValues = languageValuesRepo.Table().Where(x => x.LanguageId == language_id).ToList();

            foreach (var item in storeProduct)
            {

                var productQuery = productRepo.Table().Where(x => x.Id == item.ProductId)
                    .Include(x => x.ParentProduct)
                    .Select(x => new ChannelAdvisorChildModel
                    {
                        Sku = x.Sku,
                        Title = x.Title,
                        Brand = x.ParentProduct.BrandPP.Name,
                        Classification = item.StoreCategory,
                        SupplierName = x.ParentProduct.SupplierPP.Name,
                        Description = x.Description,
                        EAN = x.Barcode,
                        Manufacturer = x.ParentProduct.SupplierPP.Name,
                        ShortDescription = x.ShortDescription,
                        Weight = Convert.ToDecimal(productAttributeValues.FirstOrDefault(y => y.ProductId == x.Id && y.Attribute == "Product Weight").AttributeValue.Replace(".", ",")),
                        Height = Convert.ToDecimal(productAttributeValues.FirstOrDefault(y => y.ProductId == x.Id && y.Attribute == "Product Height").AttributeValue.Replace(".", ",")),
                        Width = Convert.ToDecimal(productAttributeValues.FirstOrDefault(y => y.ProductId == x.Id && y.Attribute == "Product Width").AttributeValue.Replace(".", ",")),
                        Length = Convert.ToDecimal(productAttributeValues.FirstOrDefault(y => y.ProductId == x.Id && y.Attribute == "Product Lenght").AttributeValue.Replace(".", ",")),
                        BuyItNowPrice = item.Price == null ? 0 : item.Price.Value,
                        ParentProductID = storeParentId.ToString(),
                        RelationshipName = string.Join("-", categoryAttributeMapping.Where(y => y.CategoryId == x.ParentProduct.CategoryId).Select(y => y.Attributes.Name).ToArray()),
                        Attributes = productAttributeMapping.Where(y => y.ProductId == x.Id).Select(y => new Attribute()
                        {
                            Name = y.Attributes.Name,
                            Value = y.AttributeValueId == null ?
                            (languageValues.FirstOrDefault(z => z.EntityId == y.Id && z.TableName == "ProductAttributeMapping" && z.FieldName == y.Attributes.Name) == null ?
                            y.RequiredAttributeValue : (languageValues.FirstOrDefault(z => z.EntityId == y.Id && z.TableName == "ProductAttributeMapping" && z.FieldName == y.Attributes.Name).Value == null ? y.RequiredAttributeValue : languageValues.FirstOrDefault(z => z.EntityId == y.Id && z.TableName == "ProductAttributeMapping" && z.FieldName == y.Attributes.Name).Value)) : (languageValues.FirstOrDefault(z => z.EntityId == y.AttributeValueId && z.TableName == "AttributesValue") == null ? y.AttributesValue.Value : languageValues.FirstOrDefault(z => z.EntityId == y.AttributeValueId && z.TableName == "AttributesValue").Value)
                        }).ToList()
                    }).First();

                var category = subStoreId.Where(x => x.ProductId == item.ProductId).Select(x => new Attribute()
                {
                    Name = x.Store.Name + "-Category",
                    Value = x.StoreCategory
                }).ToList();

                productQuery.Attributes.AddRange(category);

                var labelQuery = new Label() { Name = storeRepo.Find(x => x.Id == store_id).Name };

                var imageQuery = pictureRepo.Table().Where(x => x.ProductId == item.ProductId).OrderBy(x => x.Order)
                    .Select(x => new Image()
                    {
                        PlacementName = "ITEMIMAGEURL",
                        UrlPath = new UrlPath() { Url = x.CdnPath }
                    }).ToList();

                var dCQuantitysQuery = storeProductMappingRepo.Table().Where(x => x.StoreId == store_id && x.ProductId == item.ProductId && x.UpdatedDate > DateTime.Now.AddMinutes(-30))
                        .Select(x => new DCQuantity()
                        {
                            Value = new Value
                            {
                                UpdateType = "InStock",
                                Updates = new List<Update>()
                                    {
                                          new Update()
                                          {
                                           Quantity=x.Stock
                                          }
                                    }
                            }
                        }
                        ).First();

                channelAdvisorProductViewModels.Add(new ChannelAdvisorProductViewModel()
                {
                    channelAdvisorChildModel = productQuery,
                    labels = labelQuery,
                    Images = imageQuery,
                    DCQuantitys = dCQuantitysQuery

                });
            }

            return channelAdvisorProductViewModels;
        }

        public List<ChannelAdvisorPriceUpdateModel> ChannelAdvisorPriceUpdateModel(int store_id)
        {
            var queryPrice = storeProductMappingRepo.Table().Where(x=>x.StoreId==store_id)
                .Select(x => new ChannelAdvisorPriceUpdateModel()
                {
                    ProductId = x.StoreProductId,
                    UpdatePrice = new ChannelAdvisorPrice() { BuyItNowPrice = x.Price == null ? 0 : x.Price.Value }
                }).ToList();
            return queryPrice;
        }

        public List<DCQuantityandID> UpdateQuantity(int store_id)
        {
            var query = storeProductMappingRepo.Table().Where(x => x.StoreId == store_id && x.UpdatedDate > DateTime.Now.AddMinutes(-30) && x.StoreProductId>0)
                .Select(x => new DCQuantityandID()
                {
                    ProductID = x.StoreProductId,
                    dCQuantity = new DCQuantity()
                    {
                        Value = new Value
                        {
                            UpdateType = "InStock",
                            Updates = new List<Update>()
                            {
                                new Update()
                                {
                                    Quantity=x.Stock
                                }
                            }
                        }
                    }

                }).ToList();
            return query;
        }

        public List<DCQuantityandID> UpdateQuantityAll(int store_id)
        {
            var query = storeProductMappingRepo.Table().Where(x => x.StoreId == store_id)
                .Select(x => new DCQuantityandID()
                {
                    ProductID = x.StoreProductId,
                    dCQuantity = new DCQuantity()
                    {
                        Value = new Value
                        {
                            UpdateType = "InStock",
                            Updates = new List<Update>()
                            {
                                new Update()
                                {
                                    Quantity=x.Stock
                                }
                            }
                        }
                    }

                }).ToList();
            return query;
        }

        public List<ChannelAdvisorUpdateVM> JsonchannelAdvisorUpdateModels(int store_id, List<ProductJsonModel> productJsonModels, List<ProductStockAndPriceJsonModel> productStockAndPriceJsonModels)
        {
            List<ChannelAdvisorUpdateVM> channelAdvisorUpdateVMs = new List<ChannelAdvisorUpdateVM>();

            var storeProduct = storeProductMappingRepo.Table().Where(x => x.StoreId == store_id && productJsonModels.Any(y => y.Id == x.ProductId) && x.StoreProductId>0).ToList();

            var subStoreId = storeProductMappingRepo.Table().Where(x => x.BaseStoreId == store_id).ToList();

            foreach (var item in productJsonModels)
            {
                var productQuery = new ChannelAdvisorUpdateModel
                {
                    Title = item.Title,
                    Brand = item.Brand,
                    Classification = item.Category.Split(">>").Last(),
                    SupplierName = item.Supplier,
                    Description = item.Description,
                    EAN = item.Barcode,
                    Manufacturer = item.Supplier,
                    BuyItNowPrice = productStockAndPriceJsonModels.FirstOrDefault(x => x.Sku == item.Sku).Price
                };

                var imageQuery = item.Pictures
                .Select(x => new Image()
                {
                    PlacementName = "ITEMIMAGEURL",
                    UrlPath = new UrlPath() { Url = x.CdnPath }
                }).ToList();

                List<AttributeUpdate> attributes = new List<AttributeUpdate>();
                foreach (var attribute in item.Attributes)
                {
                    attributes.Add(new AttributeUpdate()
                    {
                        Name = attribute.FieldName.ToUpperInvariant() + "NEW",
                        Value = attribute.Value,
                        ProductID = item.StoreProductId
                    });
                }

                var browseNode = storeProduct.Where(x => x.ProductId == item.Id).Select(x => new AttributeUpdate()
                {
                    Name = x.StoreId + "-Category",
                    Value = x.StoreCategory,
                    ProductID = item.StoreProductId
                }).ToList();

                var subCategory = subStoreId.Where(x => x.ProductId == item.Id).Select(x => new AttributeUpdate()
                {
                    Name = x.StoreId + "-Category",
                    Value = x.StoreCategory,
                    ProductID = item.StoreProductId
                }).ToList();

                attributes.AddRange(browseNode);
                attributes.AddRange(subCategory);


                channelAdvisorUpdateVMs.Add(new ChannelAdvisorUpdateVM()
                {
                    Value = attributes,
                    Images = imageQuery,
                    productId = storeProduct.First(x => x.ProductId == item.Id).StoreProductId,
                    productUpdateModel = productQuery

                });
            }

            return channelAdvisorUpdateVMs;
        }

        public List<ChannelAdvisorUpdateVM> updateStoreCategory(int store_id)
        {
            List<ChannelAdvisorUpdateVM> channelAdvisorUpdateVMs = new List<ChannelAdvisorUpdateVM>();
            var storeproduct = storeProductMappingRepo.Table().Where(x => x.BaseStoreId == store_id);
            var basestoreproduct = storeProductMappingRepo.Table().Where(x => x.StoreId == store_id && x.StoreProductId>0);

            foreach (var item in storeproduct)
            {
                if (!basestoreproduct.Any(x => x.ProductId == item.ProductId))
                    continue;
                List<AttributeUpdate> attributes = new List<AttributeUpdate>();

                var browseNode = new AttributeUpdate()
                {
                    Name = "17-Category",
                    Value = item.StoreCategory,
                    ProductID = basestoreproduct.First(x => x.ProductId == item.ProductId).StoreProductId
                };

                attributes.Add(browseNode);

                channelAdvisorUpdateVMs.Add(new ChannelAdvisorUpdateVM()
                {
                    Value = attributes,
                    Images = new List<Image>(),
                    productId = basestoreproduct.First(x => x.ProductId == item.ProductId).StoreProductId,
                    productUpdateModel = null

                });
            }

            return channelAdvisorUpdateVMs;
        }

        public List<EntegraXmlModel> EntegraProducts(int store_id, int language_id)
        {
            List<EntegraXmlModel> entegraXmlModels = new List<EntegraXmlModel>();

            var languageValues = languageValuesRepo.Table().Where(x => x.LanguageId == language_id && x.TableName == "Product").ToList();

            var language = languageValuesRepo.Table().Where(x => x.LanguageId == language_id && x.TableName == "Category").ToList();

            entegraXmlModels = storeProductMappingRepo.Table().Where(x => x.StoreId == store_id)
                .Include(x => x.Product)
                .Select(x => new EntegraXmlModel
                {
                    SKU = x.Product.Sku,
                    Name = languageValues.FirstOrDefault(y => y.EntityId == x.ProductId && y.FieldName == "Title") == null ? x.Product.Title :
                    languageValues.FirstOrDefault(y => y.EntityId == x.ProductId && y.FieldName == "Title").Value,
                    Brand = x.Product.ParentProduct.BrandPP.Name,
                    ProductCategory = language.FirstOrDefault(y => y.EntityId == Convert.ToInt32(x.Product.ParentProduct.CategoryId)) == null ? x.Product.ParentProduct.Category.Name : language.FirstOrDefault(y => y.EntityId == Convert.ToInt32(x.Product.ParentProduct.CategoryId)).Value,
                    Supplier = x.Product.ParentProduct.SupplierPP.Name,
                    FullDescription = languageValues.FirstOrDefault(y => y.EntityId == x.ProductId && y.FieldName == "Description") == null ? x.Product.Description : languageValues.FirstOrDefault(y => y.EntityId == x.ProductId && y.FieldName == "Description").Value,
                    BarCode = x.Product.Barcode,
                    ShortDescription = languageValues.FirstOrDefault(y => y.EntityId == x.ProductId && y.FieldName == "ShortDescription") == null ? x.Product.ShortDescription : languageValues.FirstOrDefault(y => y.EntityId == x.ProductId && y.FieldName == "ShortDescription").Value,
                    Price = x.Price == null ? 0 : x.Price.Value,
                    ProductPictures = x.Product.Pictures.OrderBy(y => y.Order).Select(y => y.CdnPath).ToList(),
                    StockQuantity = x.Stock,
                    Desi = x.Product.AbroadDesi.HasValue ? x.Product.AbroadDesi.Value : 0

                }).ToList();

            var newEntegraXmlModel = entegraXmlModels.Where(x => x.Price > 0).ToList();

            return newEntegraXmlModel;
        }

        public List<JoomProductModel> joomProductModels(int store_id, int language_id)
        {
            var storeProduct = storeProductMappingRepo.Table().Where(x => x.StoreId == store_id).ToList();

            var productAttributeValueSize = productAttributeValueRepo.Table()
                .Where(x => storeProduct.Any(y => y.Product.Id == x.ProductId) && x.Attribute.Contains("Size") && x.AttributeValueId != null).ToList();

            var productAttributeValueColor = productAttributeValueRepo.Table()
                .Where(x => storeProduct.Any(y => y.Product.Id == x.ProductId) && x.Attribute == "Color" && x.AttributeValueId != null).ToList();

            var languageValueColor = languageValuesRepo.Table()
                .Where(x => x.TableName == "AttributesValue" && x.LanguageId == language_id && productAttributeValueColor.Any(y => y.AttributeValueId.Value == x.EntityId)).ToList();

            var languageValues = languageValuesRepo.Table().Where(x => x.LanguageId == language_id).ToList();

            var parentProductQuery = productRepo.Table()
                .Where(x => storeProduct.Any(y => y.ProductId == x.Id))
                .Select(x => new JoomProductModel
                {
                    sku = x.Sku,
                    name = languageValues.FirstOrDefault(y => y.EntityId == x.Id && y.TableName == "Product" && y.FieldName == "Title") == null ? x.Title :
                    languageValues.FirstOrDefault(y => y.EntityId == x.Id && y.TableName == "Product" && y.FieldName == "Title").Value,
                    brand = x.ParentProduct.BrandPP.Name,
                    description = languageValues.FirstOrDefault(y => y.EntityId == x.Id && y.TableName == "Product" && y.FieldName == "Description")
                    == null ? x.Description : languageValues.FirstOrDefault(y => y.EntityId == x.Id && y.TableName == "Product" && y.FieldName == "Description").Value,
                    tags = new List<string>() { "BiggBrands" },
                    gtin = x.Barcode,
                    extraImages = x.Pictures.Where(y => y.Order != 0).Count() > 0 ? x.Pictures.Where(y => y.Order != 0).Select(y => y.CdnPath).ToList() : x.Pictures.Where(y => y.Order == 0).Select(y => y.CdnPath).ToList(),
                    mainImage = x.Pictures.Where(y => y.Order == 0).FirstOrDefault().CdnPath,
                    variants =new List<JoomProductVariant>() {  new JoomProductVariant()
                    {
                        gtin = x.Barcode,
                        inventory = storeProduct.FirstOrDefault(z => z.ProductId == x.Id).Stock,
                        price = storeProduct.FirstOrDefault(z => z.ProductId == x.Id).Price.ToString().Replace(",", "."),
                        msrPrice = storeProduct.FirstOrDefault(z => z.ProductId == x.Id).Price.ToString().Replace(",", "."),
                        mainImage = x.Pictures.OrderBy(z => z.Order).FirstOrDefault().CdnPath,
                        sku = x.Sku,
                        size = productAttributeValueSize.Where(z => z.ProductId == x.Id).Count() > 0 ? productAttributeValueSize.FirstOrDefault(z => z.ProductId == x.Id).AttributeValue : null,
                        colors = productAttributeValueColor.Where(z => z.ProductId == x.Id).Count() > 0 ? languageValueColor.Where(k => k.EntityId == productAttributeValueColor.FirstOrDefault(z => z.ProductId ==x.Id).AttributeValueId).FirstOrDefault().Value : null
                    } }
                }).ToList();

            return parentProductQuery;
        }

        public List<JoomUpdateModel> joomUpdateModels(int store_id, int language_id)
        {
            //x.UpdatedDate > DateTime.Now.AddMinutes(-30)
            var storeProduct = storeProductMappingRepo.Table().Where(x => x.StoreId == store_id && x.ProductId == 1379).ToList();

            if (storeProduct.Count == 0)
                return new List<JoomUpdateModel>();

            var productAttributeValueSize = productAttributeValueRepo.Table()
                .Where(x => storeProduct.Any(y => y.Product.Id == x.ProductId) && x.Attribute.Contains("Size") && x.AttributeValueId != null).ToList();

            var productAttributeValueColor = productAttributeValueRepo.Table()
                .Where(x => storeProduct.Any(y => y.Product.Id == x.ProductId) && x.Attribute == "Color" && x.AttributeValueId != null).ToList();

            var languageValueColor = languageValuesRepo.Table()
                .Where(x => x.TableName == "AttributesValue" && x.LanguageId == language_id && productAttributeValueColor.Any(y => y.AttributeValueId.Value == x.EntityId)).ToList();


            var parentProductQuery = productRepo.Table()
                .Where(x => storeProduct.Any(y => y.ProductId == x.Id))
                .Select(x => new JoomUpdateModel
                {
                    gtin = x.Barcode,
                    inventory = storeProduct.FirstOrDefault(z => z.ProductId == x.Id).Stock,
                    price = storeProduct.FirstOrDefault(z => z.ProductId == x.Id).Price.ToString().Replace(",", "."),
                    shipping = "0.00",
                    main_image = x.Pictures.Where(z => z.Order == 0).FirstOrDefault().CdnPath,
                    sku = x.Sku,
                    size = productAttributeValueSize.Where(z => z.ProductId == x.Id).Count() > 0 ? productAttributeValueSize.FirstOrDefault(z => z.ProductId == x.Id).AttributeValue : null,
                    color = productAttributeValueColor.Where(z => z.ProductId == x.Id).Count() > 0 ? languageValueColor.Where(k => k.EntityId == productAttributeValueColor.FirstOrDefault(z => z.ProductId == x.Id).AttributeValueId).FirstOrDefault().Value : null
                }).ToList();

            return parentProductQuery;
        }

        //language mantiğini düzelt
        public List<ProductWithAttributeXMLModel> productWithAttributeXMLs(int store_id, int language_id)
        {
            var storeProduct = storeProductMappingRepo.Table().Where(x => x.StoreId == store_id).Include(x => x.Product);

            var productAttributeValues = productAttributeRepo.Table()
                .Where(x => storeProduct.Any(y => y.Product.Id == x.ProductId)).ToList();

            var languageValues = languageValuesRepo.Table().Where(x => x.LanguageId == language_id).ToList();
            var englishLanguageValues = languageValuesRepo.Table().Where(x => x.LanguageId == 2 && x.TableName == "Category").ToList();

            var productXMLWithAttribute = storeProduct.Select(x => new ProductWithAttributeXMLModel
            {
                SKU = x.Product.Sku,
                Name = languageValues.FirstOrDefault(y => y.EntityId == x.ProductId && y.TableName == "Product" && y.FieldName == "Title") == null ? x.Product.Title :
                      languageValues.FirstOrDefault(y => y.EntityId == x.ProductId && y.TableName == "Product" && y.FieldName == "Title").Value,
                Brand = x.Product.ParentProduct.BrandPP.Name,
                //MainProductCategory = englishLanguageValues.FirstOrDefault(y => y.EntityId == Convert.ToInt32(x.Product.ParentProduct.Category.ParentCategoryId) && y.TableName == "Category") == null ? categories.First(y => y.ParentCategoryId == x.Product.ParentProduct.Category.ParentCategoryId).Name : englishLanguageValues.FirstOrDefault(y => y.EntityId == Convert.ToInt32(x.Product.ParentProduct.Category.ParentCategoryId) && y.TableName == "Category").Value,
                ProductCategory = englishLanguageValues.FirstOrDefault(y => y.EntityId == Convert.ToInt32(x.Product.ParentProduct.CategoryId) && y.TableName == "Category") == null ? x.Product.ParentProduct.Category.Name : englishLanguageValues.FirstOrDefault(y => y.EntityId == Convert.ToInt32(x.Product.ParentProduct.CategoryId) && y.TableName == "Category").Value,
                Supplier = x.Product.ParentProduct.SupplierPP.Name,
                FullDescription = languageValues.FirstOrDefault(y => y.EntityId == x.ProductId && y.TableName == "Product" && y.FieldName == "Description") == null ? x.Product.Description : languageValues.FirstOrDefault(y => y.EntityId == x.ProductId && y.TableName == "Product" && y.FieldName == "Description").Value,
                BarCode = x.Product.Barcode,
                ShortDescription = languageValues.FirstOrDefault(y => y.EntityId == x.ProductId && y.TableName == "Product" && y.FieldName == "ShortDescription") == null ? x.Product.ShortDescription : languageValues.FirstOrDefault(y => y.EntityId == x.ProductId && y.TableName == "Product" && y.FieldName == "ShortDescription").Value,
                Price = x.Price == null ? 0 : x.Price.Value,
                ProductPictures = x.Product.Pictures.Select(y => y.CdnPath).ToList(),
                StockQuantity = x.Stock,
                Desi = x.Product.AbroadDesi.HasValue ? x.Product.AbroadDesi.Value : 0,
                Attributes = productAttributeValues.Where(y => y.ProductId == x.ProductId).Select(y => new Attribute()
                {
                    Name = y.Attributes.Name,
                    Value = y.AttributeValueId == null ?
                        (languageValues.FirstOrDefault(z => z.EntityId == y.Id && z.TableName == "ProductAttributeMapping" && z.FieldName == y.Attributes.Name) == null ?
                        y.RequiredAttributeValue : languageValues.FirstOrDefault(z => z.EntityId == y.Id && z.TableName == "ProductAttributeMapping" && z.FieldName == y.Attributes.Name).Value) : (languageValues.FirstOrDefault(z => z.EntityId == y.AttributeValueId && z.TableName == "AttributesValue") == null ? y.AttributesValue.Value : languageValues.FirstOrDefault(z => z.EntityId == y.AttributeValueId && z.TableName == "AttributesValue").Value)
                }).ToList()
            }).ToList();
            return productXMLWithAttribute.Where(x => x.Price > 0).ToList();
        }

        public List<IntegrationList> integrationLists(string sku, string brand, string category, string store)
        {
            List<IntegrationList> query = new List<IntegrationList>();

            if (!String.IsNullOrEmpty(sku))
            {
                query = storeProductMappingRepo.Table()
                    .Where(x => x.StoreId == Convert.ToInt32(store) && x.Product.Sku == sku)
                    .Include(x => x.Product)
                    .Select(x => new IntegrationList()
                    {
                        ProductId = x.ProductId.Value,
                        Sku = x.Product.Sku,
                        PictureUrl = x.Product.Pictures.FirstOrDefault() == null ? null : x.Product.Pictures.FirstOrDefault().CdnPath,
                        StoreId = x.StoreId.Value,
                        StoreProductId = x.StoreProductId,
                        Title = x.Product.Title

                    }).ToList();

                return query;
            }
            else if (!String.IsNullOrEmpty(brand))
            {
                query = storeProductMappingRepo.Table()
                    .Where(x => x.StoreId == Convert.ToInt32(store) && x.Product.ParentProduct.BrandId == Convert.ToInt32(brand))
                    .Include(x => x.Product)
                    .Select(x => new IntegrationList()
                    {
                        ProductId = x.ProductId.Value,
                        Sku = x.Product.Sku,
                        PictureUrl = x.Product.Pictures.FirstOrDefault() == null ? null : x.Product.Pictures.FirstOrDefault().CdnPath,
                        StoreId = Convert.ToInt32(store),
                        StoreProductId = x.StoreProductId,
                        Title = x.Product.Title

                    }).ToList();
                return query;
            }
            else if (!String.IsNullOrEmpty(category))
            {
                query = storeProductMappingRepo.Table()
                    .Where(x => x.StoreId == Convert.ToInt32(store) && x.Product.ParentProduct.CategoryId == Convert.ToInt32(category))
                    .Include(x => x.Product)
                    .Select(x => new IntegrationList()
                    {
                        ProductId = x.ProductId.Value,
                        Sku = x.Product.Sku,
                        PictureUrl = x.Product.Pictures.FirstOrDefault() == null ? null : x.Product.Pictures.FirstOrDefault().CdnPath,
                        StoreId = x.StoreId.Value,
                        StoreProductId = x.StoreProductId,
                        Title = x.Product.Title

                    }).ToList();
                return query;
            }

            return query;
        }

        public List<int> integrationListId(string sku, string brand, string category, string store)
        {
            List<int> query = new List<int>();

            if (sku != "default")
            {
                query = storeProductMappingRepo.Table()
                    .Where(x => x.StoreId == Convert.ToInt32(store) && x.Product.Sku == sku)
                    .Include(x => x.Product)
                    .Select(x => x.ProductId.Value).ToList();

                return query;
            }
            else if (!String.IsNullOrEmpty(brand))
            {
                query = storeProductMappingRepo.Table()
                    .Where(x => x.StoreId == Convert.ToInt32(store) && x.Product.ParentProduct.BrandId == Convert.ToInt32(brand))
                    .Include(x => x.Product)
                    .Select(x => x.ProductId.Value).ToList();
                return query;
            }
            else if (!String.IsNullOrEmpty(category))
            {
                query = storeProductMappingRepo.Table()
                    .Where(x => x.StoreId == Convert.ToInt32(store) && x.Product.ParentProduct.CategoryId == Convert.ToInt32(category))
                    .Include(x => x.Product)
                    .Select(x => x.ProductId.Value).ToList();
                return query;
            }

            return query;
        }

        public List<ChannelAdvisorProductViewModel> allChannelAdvisorProductVMs(int store_id, int language_id, int skip, int take)
        {
            List<ChannelAdvisorProductViewModel> channelAdvisorProductViewModels = new List<ChannelAdvisorProductViewModel>();

            var storeProduct = storeProductMappingRepo.Table().Where(x => x.StoreId == store_id && x.StoreParentProductId == 0 && x.StoreProductId == 0).Skip(skip).Take(take).ToList();

            var subStoreId = storeProductMappingRepo.Table().Where(x => x.BaseStoreId == store_id && x.StoreProductId == 0).Include(x => x.Store).ToList();

            var productAttributeMapping = productAttributeRepo.Table()
                .Where(x => storeProduct.Any(y => y.Product.Id == x.ProductId)).Include(x => x.Attributes).Include(x => x.AttributesValue).ToList();

            var categoryAttributeMapping = categoryAttributeRepo.Table().Where(x => x.IsRequired).ToList();

            var languageValues = languageValuesRepo.Table().Where(x => x.LanguageId == language_id).ToList();

            var language = languageValuesRepo.Table().Where(x => x.LanguageId == 2 && x.TableName == "Category").ToList();

            foreach (var item in storeProduct)
            {

                var productQuery = productRepo.Table().Where(x => x.Id == item.ProductId)
                    .Include(x => x.ParentProduct)
                    .Select(x => new ChannelAdvisorProductModel
                    {
                        Sku = x.Sku,
                        Title = languageValues.FirstOrDefault(y => y.EntityId == x.Id && y.TableName == "Product" && y.FieldName == "Title") == null ? x.Title :
                            languageValues.FirstOrDefault(y => y.EntityId == x.Id && y.TableName == "Product" && y.FieldName == "Title").Value,
                        Brand = x.ParentProduct.BrandPP.Name,
                        Classification = language.FirstOrDefault(y => y.EntityId == Convert.ToInt32(x.ParentProduct.CategoryId)).Value,
                        SupplierName = x.ParentProduct.SupplierPP.Name,
                        Description = languageValues.FirstOrDefault(y => y.EntityId == x.Id && y.TableName == "Product" && y.FieldName == "Description") == null ? x.Description : languageValues.FirstOrDefault(y => y.EntityId == x.Id && y.TableName == "Product" && y.FieldName == "Description").Value,
                        EAN = x.Barcode,
                        Manufacturer = x.ParentProduct.SupplierPP.Name,
                        ShortDescription = languageValues.FirstOrDefault(y => y.EntityId == x.Id && y.TableName == "Product" && y.FieldName == "ShortDescription") == null ? x.ShortDescription : languageValues.FirstOrDefault(y => y.EntityId == x.Id && y.TableName == "Product" && y.FieldName == "ShortDescription").Value,
                        BuyItNowPrice = item.Price.Value,
                        Attributes = productAttributeMapping.Where(y => y.ProductId == x.Id).Select(y => new Attribute()
                        {
                            Name = y.Attributes.Name.ToUpperInvariant() + "NEW",
                            Value = y.AttributeValueId == null ?
                            (languageValues.FirstOrDefault(z => z.EntityId == y.Id && z.TableName == "ProductAttributeMapping" && z.FieldName == y.Attributes.Name) == null ?
                            y.RequiredAttributeValue : (languageValues.FirstOrDefault(z => z.EntityId == y.Id && z.TableName == "ProductAttributeMapping" && z.FieldName == y.Attributes.Name).Value == null ? y.RequiredAttributeValue : languageValues.FirstOrDefault(z => z.EntityId == y.Id && z.TableName == "ProductAttributeMapping" && z.FieldName == y.Attributes.Name).Value)) : (languageValues.FirstOrDefault(z => z.EntityId == y.AttributeValueId && z.TableName == "AttributesValue") == null ? y.AttributesValue.Value : languageValues.FirstOrDefault(z => z.EntityId == y.AttributeValueId && z.TableName == "AttributesValue").Value)
                        }).ToList()
                    }).First();

                var category = subStoreId.Where(x => x.ProductId == item.ProductId)
                    .Select(x => new Attribute()
                    {
                        Name = x.Store.Name + "-NEW-Category",
                        Value = x.StoreCategory
                    }).ToList();

                productQuery.Attributes.AddRange(category);

                productQuery.Attributes.Add(new Attribute()
                {
                    Name = item.StoreId + "-Category",
                    Value = item.StoreCategory
                });

                var labelQuery = new Label() { Name = storeRepo.Find(x => x.Id == store_id).Name };

                var imageQuery = pictureRepo.Table().Where(x => x.ProductId == item.ProductId).OrderBy(x => x.Order)
                    .Select(x => new Image()
                    {
                        PlacementName = "ITEMIMAGEURL",
                        UrlPath = new UrlPath() { Url = x.CdnPath }
                    }).ToList();

                var dCQuantitysQuery = storeProductMappingRepo.Table().Where(x => x.StoreId == store_id && x.ProductId == item.ProductId)
                        .Select(x => new DCQuantity()
                        {
                            Value = new Value
                            {
                                UpdateType = "InStock",
                                Updates = new List<Update>()
                                    {
                                          new Update()
                                          {
                                           Quantity=x.Stock
                                          }
                                    }
                            }
                        }
                        ).First();

                channelAdvisorProductViewModels.Add(new ChannelAdvisorProductViewModel()
                {
                    channelAdvisorProductModel = productQuery,
                    labels = labelQuery,
                    Images = imageQuery,
                    DCQuantitys = dCQuantitysQuery

                });
            }

            return channelAdvisorProductViewModels;
        }

        #endregion

        public List<StoreProductMappingModel> GetSubStoreProductMappingByStoreId(int storeId)
        {
            return autoMapper.MapCollection<StoreProductMapping, StoreProductMappingModel>(storeProductMappingRepo.Table()
                .Where(x => x.BaseStoreId == storeId)).ToList();
        }

        public async Task<ICollection<ProductModel>> AllProductsAsync(int id)
        {

            var productTableName = GetFieldValues.ProductTable;
            var productFieldList = GetFieldValues.ProductFields;

            var languageValues = languageValueRepo.Filter(x => x.LanguageId == id);
            //return null;
            var datas = await (productRepo.Table().Include(x => x.VatRate)
                                .Include(x => x.Currency)
                                .Include(x => x.ParentProduct)
                                .Include(x => x.Supplier)
                                .Include(x => x.Brand)
                                .Include(x => x.ParentProduct.Category)
                                .Where(x => x.State == (int)State.Active)
                                .Select(x => new ProductModel()
                                {
                                    Id = x.Id,
                                    MetaDescription = x.MetaDescription,
                                    Model = x.Model,
                                    Name = x.Name,
                                    Barcode = x.Barcode,
                                    BuyingPrice = x.BuyingPrice,
                                    AbroadDesi = x.AbroadDesi,
                                    CorporatePrice = x.CorporatePrice,
                                    Description = x.Description,
                                    Gtip = x.Gtip,
                                    Desi = x.Desi,
                                    ShortDescription = x.ShortDescription,
                                    PsfPrice = x.PsfPrice,
                                    Sku = x.Sku,
                                    Category = x.ParentProduct.Category != null ? x.ParentProduct.Category.Name : string.Empty,
                                    Title = x.Title,
                                    CurrName = x.Currency == null ? string.Empty : x.Currency.Name,
                                    CurrencyId = x.CurrencyId,
                                    VatRateAmount = x.VatRate == null ? string.Empty : x.VatRate.Name
                                })).OrderByDescending(x => x.CreatedDate).ToListAsync();

            if (!languageService.isDefaultLanguage(id))
            {
                var productValues = languageValues.Where(x => x.TableName == productTableName).ToList();

                var count = datas.Count;
                var fieldCount = productFieldList.Count;
                for (int i = 0; i < count; i++)
                {
                    for (int y = 0; y < fieldCount; y++)
                    {
                        var lValue = productValues.FirstOrDefault(x => x.FieldName == productValues[y].FieldName);
                        if (lValue != null)
                        {
                            datas[i].GetType().GetProperty(productValues[y].FieldName).SetValue(datas[i], lValue.Value);
                        }
                    }
                }
            }

            return datas;
        }

        public ICollection<ProductGroupModel> ProductGroups()
        {
            return autoMapper.MapCollection<ProductGroup, ProductGroupModel>(productGroupRepo.Filter(x => x.State == (int)State.Active)).ToList();
        }

        public IQueryable<ProductModel> AllProductQueryable()
        {
            return (productRepo.Table().Include(x => x.VatRate)
                                .Include(x => x.Currency)
                                .Include(x => x.PsfCurrency)
                                .Include(x => x.CorporateCurrency)
                                .Include(x => x.DdpCurrency)
                                .Include(x => x.Pictures)
                                .Include(x => x.ParentProduct)
                                .Include(x => x.Supplier)
                                .Include(x => x.Brand)
                                .Include(x => x.ParentProduct.Category)
                                .Include(x => x.ParentProduct.BrandPP)
                                .Where(x => x.State != (int)State.Delete)
                                .Select(x => new ProductModel()
                                {
                                    Id = x.Id,
                                    MetaDescription = x.MetaDescription,
                                    Model = x.Model,
                                    Name = x.Name,
                                    FirstPicturePath = x.Pictures.FirstOrDefault().CdnPath,
                                    Barcode = x.Barcode,
                                    BuyingPrice = x.BuyingPrice,
                                    AbroadDesi = x.AbroadDesi,
                                    BrndName = x.Brand != null ? x.Brand.Name : x.ParentProduct.BrandPP != null ? x.ParentProduct.BrandPP.Name : string.Empty,
                                    CorporatePrice = x.CorporatePrice,
                                    Description = x.Description,
                                    Gtip = x.Gtip,
                                    Desi = x.Desi,
                                    ShortDescription = x.ShortDescription,
                                    PsfPrice = x.PsfPrice,
                                    DdpPrice = x.DdpPrice,
                                    FobPrice = x.FobPrice,
                                    Sku = x.Sku,
                                    CategoryId = x.ParentProduct.CategoryId.Value,
                                    Category = x.ParentProduct.Category != null ? x.ParentProduct.Category.Name : string.Empty,
                                    Title = x.Title,
                                    CurrName = x.Currency == null ? string.Empty : x.Currency.Abbrevation,
                                    PsfCurrName = x.PsfCurrency == null ? string.Empty : x.PsfCurrency.Abbrevation,
                                    CorCurrName = x.CorporateCurrency == null ? string.Empty : x.CorporateCurrency.Abbrevation,
                                    DdpCurrName = x.DdpCurrency == null ? string.Empty : x.DdpCurrency.Abbrevation,
                                    FobCurrName = x.FobCurrency == null ? string.Empty : x.FobCurrency.Abbrevation,
                                    CurrencyId = x.CurrencyId,
                                    VatRateAmount = x.VatRate == null ? string.Empty : x.VatRate.Name,
                                    CreatedDate = x.CreatedDate,
                                    ParentProductId = x.ParentProduct.Id,
                                    ParentProductName = x.ParentProduct == null ? string.Empty : x.ParentProduct.Title,
                                    State = x.State
                                }).OrderByDescending(x => x.CreatedDate));
        }

        public IEnumerable<ProductModel> AllProductsQueryableAsync(int id, int take, int skip, out int total)
        {
            var productTableName = GetFieldValues.ProductTable;
            var productFieldList = GetFieldValues.ProductFields;

            var categoryTableName = GetFieldValues.CategoryTable;
            var categoryFieldList = GetFieldValues.CategoryFields;

            var languageValues = LanguageValues(id);
            // query
            var query = AllProductQueryable();
            // total count
            total = query.Count();
            //return null;
            var datas = query.Skip(skip).Take(take).OrderByDescending(x => x.CreatedDate).ToList();

            if (!languageService.isDefaultLanguage(id))
            {
                var productValues = languageValues.Where(x => x.TableName == productTableName && x.LanguageId == id).ToList();
                var categoryValues = languageValues.Where(x => x.TableName == categoryTableName && x.LanguageId == id).ToList();

                var count = datas.Count();
                var fieldCount = productFieldList.Count;
                for (int i = 0; i < count; i++)
                {
                    for (int y = 0; y < fieldCount; y++)
                    {

                        var lValue = productValues.FirstOrDefault(x => x.FieldName == productFieldList[y] && x.EntityId == datas[i].Id);
                        if (lValue != null)
                        {
                            datas[i].GetType().GetProperty(productFieldList[y]).SetValue(datas[i], lValue.Value);
                        }
                    }
                }

                //For category translation
                for (int i = 0; i < count; i++)
                {
                    for (int k = 0; k < categoryFieldList.Count; k++)
                    {
                        var lValue = categoryValues.FirstOrDefault(x => x.FieldName == categoryFieldList[k] && x.EntityId == datas[i].CategoryId);
                        if (lValue != null)
                        {
                            datas[i].GetType().GetProperty("Category").SetValue(datas[i], lValue.Value);
                        }
                    }
                }
            }

            return datas;
        }

        public IEnumerable<ProductModel> AllProductsQueryableAsync(int id, List<ProductModel> productList)
        {
            var productTableName = GetFieldValues.ProductTable;
            var productFieldList = GetFieldValues.ProductFields;

            var categoryTableName = GetFieldValues.CategoryTable;
            var categoryFieldList = GetFieldValues.CategoryFields;

            var languageValues = LanguageValues(id);

            var datas = productList;

            if (!languageService.isDefaultLanguage(id))
            {
                var productValues = languageValues.Where(x => x.TableName == productTableName && x.LanguageId == id).ToList();
                var categoryValues = languageValues.Where(x => x.TableName == categoryTableName && x.LanguageId == id).ToList();

                var count = datas.Count();
                var fieldCount = productFieldList.Count;
                for (int i = 0; i < count; i++)
                {
                    for (int y = 0; y < fieldCount; y++)
                    {

                        var lValue = productValues.FirstOrDefault(x => x.FieldName == productFieldList[y] && x.EntityId == datas[i].Id);
                        if (lValue != null)
                        {
                            datas[i].GetType().GetProperty(productFieldList[y]).SetValue(datas[i], lValue.Value);
                        }
                    }
                }

                //For category translation
                for (int i = 0; i < count; i++)
                {
                    for (int k = 0; k < categoryFieldList.Count; k++)
                    {
                        var lValue = categoryValues.FirstOrDefault(x => x.FieldName == categoryFieldList[k] && x.EntityId == datas[i].CategoryId);
                        if (lValue != null)
                        {
                            datas[i].GetType().GetProperty("Category").SetValue(datas[i], lValue.Value);
                        }
                    }
                }
            }

            return datas;
        }

        public void AddProductGroup(ProductGroupModel productGroup)
        {
            productGroupRepo.Add(autoMapper.MapObject<ProductGroupModel, ProductGroup>(productGroup));
        }

        public void EditProductGroup(ProductGroupModel productGroup)
        {
            productGroupRepo.Update(autoMapper.MapObject<ProductGroupModel, ProductGroup>(productGroup));
        }

        public void DeleteProductGroup(ProductGroupModel productGroup)
        {
            var data = autoMapper.MapObject<ProductGroupModel, ProductGroup>(productGroup);
            data.State = (int)State.Delete;
            productGroupRepo.Update(data);
        }

        public IEnumerable<ProductModel> ProductStockAndPriceByStoreId(int storeId)
        {
            List<ProductModel> productList = new List<ProductModel>();

            var storeProducts = storeProductMappingRepo.Table()
                            .Include(x => x.Product).Where(x => x.StoreId == storeId).ToList();

            var spLenght = storeProducts.Count;
            for (int i = 0; i < spLenght; i++)
            {
                productList.Add(new ProductModel
                {
                    Sku = storeProducts[i].Product.Sku,
                    Stock = storeProducts[i].Stock,
                    Price = storeProducts[i].Price ?? 0
                });
            }

            return productList;

        }

        public ICollection<StoreProductMappingModel> ProductPricesWithStore(int productId)
        {
            var storeProductMappings = storeProductMappingRepo.Table()
                .Include(x => x.Store).ThenInclude(x => x.Currency)
                .Where(x => x.ProductId == productId)
                .GroupBy(x => x.StoreId).Select(g => g.First()).ToList();

            return autoMapper.MapCollection<StoreProductMapping, StoreProductMappingModel>(storeProductMappings).ToList();
        }


    }
}
