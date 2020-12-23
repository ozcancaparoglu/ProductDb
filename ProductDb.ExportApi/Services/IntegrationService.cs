using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ProductDb.Common.Cache;
using ProductDb.ExportApi.Common;
using ProductDb.ExportApi.DTOs;
using ProductDb.ExportApi.Models;
using ProductDb.Mapping.BiggBrandDbModelFields;
using ProductDb.Mapping.BiggBrandDbModels;
using ProductDb.Services.AttributesServices;
using ProductDb.Services.BrandServices;
using ProductDb.Services.CategoryServices;
using ProductDb.Services.LanguageServices;
using ProductDb.Services.PictureServices;
using ProductDb.Services.ProductServices;
using ProductDb.Services.ProductVariantServices;
using ProductDb.Services.StoreServices;
using ProductDb.Services.SupplierServices;

namespace ProductDb.ExportApi.Services
{
    public class IntegrationService : IIntegrationService
    {
        private readonly IProductVariantService productVariantService;
        private readonly IStoreService storeService;
        private readonly IRedisCache redisCache;
        private readonly ICacheManager cacheManager;
        private readonly IProductService productService;
        private readonly IAttributesService attributesService;
        private readonly ILanguageService languageService;

        public IntegrationService(IProductService productService,
                                  ICacheManager cacheManager,
                                  IRedisCache redisCache,
                                  IStoreService storeService,
                                  IProductVariantService productVariantService,
                                  IAttributesService attributesService, ILanguageService languageService)
        {
            this.productVariantService = productVariantService;
            this.storeService = storeService;
            this.redisCache = redisCache;
            this.cacheManager = cacheManager;
            this.productService = productService;
            this.attributesService = attributesService;
            this.languageService = languageService;
        }

        public ProductDTO PrepareProductDTO(ProductModel product, int languageId)
        {
            var productDTO = new ProductDTO()
            {
                Id = product.Id,
                AbroadDesi = product.AbroadDesi,
                Alias = product.Alias,
                Barcode = product.Barcode,
                Brand = product.Brand == null ? (product.ParentProduct.BrandPP == null ? string.Empty : product.ParentProduct.BrandPP.Name) : product.Brand.Name,
                BuyingPrice = product.BuyingPrice,
                Category = product.ParentProduct.Category == null ? string.Empty : product.ParentProduct.Category.Name,
                Currency = product.Currency == null ? string.Empty : product.Currency.Abbrevation,
                Desi = product.Desi,
                Gtip = product.Gtip,
                Name = product.Name == null ? product.Title : product.Name,
                MetaDescription = product.MetaDescription,
                MetaKeywords = product.MetaKeywords,
                MetaTitle = product.MetaTitle,
                Pictures = new List<PictureDTO>(),
                Attributes = new List<ProductAttributeDTO>(),
                SearchEngineTerms = product.SearchEngineTerms,
                ShortDescription = product.ShortDescription,
                Description = product.Description,
                Sku = product.Sku,
                Title = product.Title,
                SellingPrice = product.Price,
                Stock = product.Stock,
                SupplierName = product.Supplier != null ? product.Supplier.Name : (product.ParentProduct.SupplierPP == null ? string.Empty : product.ParentProduct.SupplierPP.Name),
                //SupplierName = product.Supplier != null ? product.Supplier.Name : string.Empty,
                Store = product.Store,
                StoreCategory = product.StoreCategory,
                StoreProductId = product.StoreProductId,
                TaxRate = product.VatRate != null ? product.VatRate.Value : 0,
                ManufacturerPartNumber = product.ParentProduct.SupplierPP == null ? string.Empty : product.ParentProduct.SupplierPP.ManufacturerPartNumber,
                PSFPrice = product.PsfPrice,
                ParentProductId = product.ParentProduct == null ? 0 : product.ParentProduct.Id,
                UpdatedDate = product.UpdatedDate ?? product.CreatedDate,
                CatalogCode = product.CatalogCode,
                ErpPoint = product.ErpPoint,
                IsFixed = product.IsFixed,
                ErpPrice = product.ErpPrice,
                Point = product.Point,
                IsFixedPoint = product.IsFixedPoint
            };

            // pictures
            var orderedPic = product.Pictures.OrderBy(o => o.Order);
            foreach (var item in orderedPic)
            {
                productDTO.Pictures.Add(new PictureDTO()
                {
                    //Alt = item.Alt,
                    CdnPath = item.CdnPath,
                    //LocalPath = item.LocalPath,
                    //Sku = item.Sku,
                    Title = item.Title,
                    Order = item.Order
                });
            }

            if (!productService.Languages().Any(x => x.IsDefault && x.Id == languageId))
            {
                // productFields
                var productFields = GetFieldValues.ProductFields;
                // language values
                var lValues = product.LanguageValues;
                // product information
                var productTableName = GetFieldValues.ProductTable;
                var iValues = lValues.Where(a => a.TableName == productTableName).ToList();

                var infoList = new List<LanguageValuesModel>();

                foreach (var item in iValues)
                {
                    productDTO.GetType().GetProperty(item.FieldName).SetValue(productDTO, item.Value);
                    infoList.Add(item);
                }

                foreach (var item in infoList)
                    lValues.Remove(item);

                // attributes
                foreach (var item in lValues)
                {
                    productDTO.Attributes.Add(new ProductAttributeDTO()
                    {
                        //EntityId = item.EntityId,
                        FieldName = item.FieldName,
                        //LanguageId = item.LanguageId,
                        //TableName = item.TableName,
                        Value = item.Value,
                        Unit = item.Unit
                    });
                }
            }
            else
            {
                // attributes
                var attributes = product.ProductAttributeValues;
                foreach (var item in attributes)
                {
                    productDTO.Attributes.Add(new ProductAttributeDTO()
                    {
                        //EntityId = item.Id,
                        FieldName = item.Attribute,
                        Value = item.AttributeValue
                    });
                }
            }

            return productDTO;
        }

        public ProductDTO ProductDetailById(int productId, int languageId)
        {
            return PrepareProductDTO(productService.ProductDetailById(productId, languageId), languageId);
        }

        public async Task<IEnumerable<ProductDTO>> ProductDetailByStoreWithLanguage(int storeId, int languageId, int? startIndex, int? endIndex)
        {
            try
            {
                var products = new List<ProductDTO>();

                bool isCached = false;
                // Redis Caching
                if (startIndex.HasValue && endIndex.HasValue)
                    isCached = redisCache.IsCached($"{CacheStatics.ProductDetailByStoreWithLanguageCache}_{storeId}_{languageId}_{startIndex}_{endIndex}");
                else
                    isCached = redisCache.IsCached($"{CacheStatics.ProductDetailByStoreWithLanguageCache}_{storeId}_{languageId}");

                if (!isCached)
                    return await SetProductDetailByStoreWithLanguage(storeId, languageId, startIndex, endIndex);
                else
                {

                    if (startIndex.HasValue && endIndex.HasValue)
                    {
                        products = await redisCache.GetAsync<List<ProductDTO>>
                                      ($"{CacheStatics.ProductDetailByStoreWithLanguageCache}_{storeId}_{languageId}_{startIndex}_{endIndex}");
                        return products.Skip(startIndex.Value).Take(endIndex.Value);
                    }
                    else
                    {
                        products = await redisCache.GetAsync<List<ProductDTO>>
                                   ($"{CacheStatics.ProductDetailByStoreWithLanguageCache}_{storeId}_{languageId}");

                        return products;
                    }

                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message.ToString());
            }

        }

        public async Task<IEnumerable<ProductDTO>> ProductDetailByStoreWithLanguage(int storeId, int languageId, List<string> SKUs)
        {

            try
            {
                var products = new List<ProductDTO>();
                //// Redis Caching
                //var isCached = redisCache.IsCached($"{CacheStatics.ProductDetailByStoreWithLanguageCache}_{storeId}_{languageId}");
                //if (!isCached)
                //    return await SetProductDetailByStoreWithLanguage(storeId, languageId, SKUs);
                //else
                //{
                //    products = await redisCache.GetAsync<List<ProductDTO>>
                //       ($"{CacheStatics.ProductDetailByStoreWithLanguageCache}_{storeId}_{languageId}");

                //    if (SKUs != null)
                //        products = products.Where(x => SKUs.Any(k => k == x.Sku)).ToList();
                //}

                return await SetProductDetailByStoreWithLanguage(storeId, languageId, SKUs);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message.ToString());
            }

        }

        private async Task<IEnumerable<ProductDTO>> SetProductDetailByStoreWithLanguage(int storeId, int languageId,
            int? startIndex, int? endIndex)
        {
            var products = new List<ProductDTO>();
            var productModels = productService.ProductDetailByStoreWithLanguage(storeId, languageId, startIndex, endIndex);

            foreach (var item in productModels)
                products.Add(PrepareProductDTO(item, languageId));

            products.ForEach(x => x.LanguageId = languageId);

            #region ProductVariants
            // product variants
            var variants = productVariantService.AllProductVariants();
            // varianted DTO
            var variantedDTO = products.Where(x => variants.Any(k => k.ProductId == x.Id)).ToList();
            // base Id added
            foreach (var item in variants.GroupBy(x => x.BaseId))
            {
                var p = products.Where(x => x.Id == item.Key).FirstOrDefault();
                if (p != null)
                {
                    var variantList = item.ToList();
                    p.ProductVariants = new List<ProductDTO>();

                    // copy object
                    ProductDTO baseDTO = (ProductDTO)p.ShallowCopy();
                    baseDTO.ProductVariants = null;
                    p.ProductVariants.Add(baseDTO);

                    p.ProductVariants.AddRange(products.Where(x => variantList.Any(k => k.ProductId == x.Id)).ToList());
                    p.ProductVariantAttr = PrepareProductVariants(variantList.FirstOrDefault().ProductAttributes.Split(","));
                }

            }

            products = products.Except(variantedDTO).ToList();
            #endregion
            if (startIndex.HasValue && endIndex.HasValue)
            {
                await redisCache.SetAsync($"{CacheStatics.ProductDetailByStoreWithLanguageCache}_{ storeId}_{ languageId}_{startIndex}_{endIndex}",
                           products, CacheStatics.ProductDetailByStoreWithLanguageCacheTime);
            }
            else
            {
                await redisCache.SetAsync($"{CacheStatics.ProductDetailByStoreWithLanguageCache}_{ storeId}_{ languageId}",
                             products, CacheStatics.ProductDetailByStoreWithLanguageCacheTime);
            }


            return products;
        }
        private async Task<IEnumerable<ProductDTO>> SetProductDetailByStoreWithLanguage(int storeId, int languageId,
             List<string> SKUs)
        {
            var products = new List<ProductDTO>();
            var productModels = productService.ProductDetailByProductIDsStoreWithLanguage(SKUs, storeId, languageId);

            foreach (var item in productModels)
                products.Add(PrepareProductDTO(item, languageId));

            products.ForEach(x => x.LanguageId = languageId);

            #region ProductVariants
            // product variants
            var variants = productVariantService.AllProductVariants();
            // varianted DTO
            var variantedDTO = products.Where(x => variants.Any(k => k.ProductId == x.Id)).ToList();
            // base Id added
            foreach (var item in variants.GroupBy(x => x.BaseId))
            {
                var p = products.Where(x => x.Id == item.Key).FirstOrDefault();
                if (p != null)
                {
                    var variantList = item.ToList();
                    p.ProductVariants = new List<ProductDTO>();

                    // copy object
                    ProductDTO baseDTO = (ProductDTO)p.ShallowCopy();
                    baseDTO.ProductVariants = null;
                    p.ProductVariants.Add(baseDTO);

                    p.ProductVariants.AddRange(products.Where(x => variantList.Any(k => k.ProductId == x.Id)).ToList());
                    p.ProductVariantAttr = PrepareProductVariants(variantList.FirstOrDefault().ProductAttributes.Split(","));
                }

            }

            products = products.Except(variantedDTO).ToList();

            #endregion

            await redisCache.SetAsync($"{CacheStatics.ProductDetailByStoreWithLanguageCache}_{ storeId}_{ languageId}",
                             products, CacheStatics.ProductDetailByStoreWithLanguageCacheTime);

            return products;
        }

        public void UpdateStoreProduct(string sku, int storeId, int storeProductId)
        {
            storeService.ChangeStoreProduct(sku, storeId, storeProductId, 0);
        }

        public IEnumerable<StoreProductMappingModel> IntegrationSubNodesByStoreId(int storeId)
        {
            return productService.GetSubStoreProductMappingByStoreId(storeId);
        }

        public string[] PrepareProductVariants(string[] ProductAttributes)
        {
            var prLenght = ProductAttributes.Length;
            string[] attNames = new string[prLenght];

            for (int i = 0; i < prLenght; i++)
            {
                var attr = attributesService.AttributeNameById(int.Parse(ProductAttributes[i]));
                attNames[i] = attr;
            }

            return attNames;
        }

        public async Task<IEnumerable<StoreModel>> StoreModels()
        {
            if (!redisCache.IsCached(CacheStatics.StoreCache))
            {
                var storeList = storeService.AllActiveStores();
                await redisCache.SetAsync($"{CacheStatics.StoreCache}", storeList, CacheStatics.StoreCacheTime);

                return storeList;
            }
            return await redisCache.GetAsync<List<StoreModel>>($"{CacheStatics.StoreCache}");
        }

        public IEnumerable<ProductStockAndPriceDTO> ProductStockAndPriceByStoreId(int storeId)
        {
            var list = productService.ProductStockAndPriceByStoreId(storeId);
            return PrepareStockAndPriceByStoreId(list);
        }

        public IEnumerable<ProductStockAndPriceDTO> PrepareStockAndPriceByStoreId(IEnumerable<ProductModel> products)
        {
            List<ProductStockAndPriceDTO> list = new List<ProductStockAndPriceDTO>();
            foreach (var item in products)
            {
                list.Add(new ProductStockAndPriceDTO
                {
                    Price = item.Price,
                    Sku = item.Sku,
                    Stock = item.Stock
                });
            }

            return list;
        }

        public void ClearCache(int storeId, int languageId)
        {
            var keyExists = $"{CacheStatics.ProductDetailByStoreWithLanguageCache}_{storeId}_{languageId}";
            if (redisCache.IsCached(keyExists))
                redisCache.Remove(keyExists);
        }

        public async Task<IEnumerable<ProductDTO>> ProductDetailByStoreWithLanguageMock(int storeId, int languageId, int? startIndex, int? endIndex)
        {
            try
            {
                var products = new List<ProductDTO>();

                bool isCached = false;
                // Redis Caching
                if (startIndex.HasValue && endIndex.HasValue)
                    isCached = redisCache.IsCached($"{CacheStatics.ProductDetailByStoreWithLanguageCache}_{storeId}_{languageId}_{startIndex}_{endIndex}");
                else
                    isCached = redisCache.IsCached($"{CacheStatics.ProductDetailByStoreWithLanguageCache}_{storeId}_{languageId}");

                if (!isCached)
                    return await SetProductDetailByStoreWithLanguageMock(storeId, languageId, startIndex, endIndex);
                else
                {

                    if (startIndex.HasValue && endIndex.HasValue)
                    {
                        products = await redisCache.GetAsync<List<ProductDTO>>
                                      ($"{CacheStatics.ProductDetailByStoreWithLanguageCache}_{storeId}_{languageId}_{startIndex}_{endIndex}");
                        return products.Skip(startIndex.Value).Take(endIndex.Value);
                    }
                    else
                    {
                        products = await redisCache.GetAsync<List<ProductDTO>>
                                   ($"{CacheStatics.ProductDetailByStoreWithLanguageCache}_{storeId}_{languageId}");

                        return products;
                    }

                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message.ToString());
            }
        }
        private async Task<IEnumerable<ProductDTO>> SetProductDetailByStoreWithLanguageMock(int storeId, int languageId, int? startIndex, int? endIndex)
        {
            var products = new List<ProductDTO>();
            var productModels = productService.ProductDetailByStoreWithLanguageMock(storeId, languageId, startIndex, endIndex);

            foreach (var item in productModels)
                products.Add(PrepareProductDTO(item, languageId));

            products.ForEach(x => x.LanguageId = languageId);

            #region ProductVariants
            // product variants
            var variants = productVariantService.AllProductVariants();
            // varianted DTO
            var variantedDTO = products.Where(x => variants.Any(k => k.ProductId == x.Id)).ToList();
            // base Id added
            foreach (var item in variants.GroupBy(x => x.BaseId))
            {
                var p = products.Where(x => x.Id == item.Key).FirstOrDefault();
                if (p != null)
                {
                    var variantList = item.ToList();
                    p.ProductVariants = new List<ProductDTO>();

                    // copy object
                    ProductDTO baseDTO = (ProductDTO)p.ShallowCopy();
                    baseDTO.ProductVariants = null;
                    p.ProductVariants.Add(baseDTO);

                    p.ProductVariants.AddRange(products.Where(x => variantList.Any(k => k.ProductId == x.Id)).ToList());
                    p.ProductVariantAttr = PrepareProductVariants(variantList.FirstOrDefault().ProductAttributes.Split(","));
                }

            }

            products = products.Except(variantedDTO).ToList();
            #endregion
            if (startIndex.HasValue && endIndex.HasValue)
            {
                await redisCache.SetAsync($"{CacheStatics.ProductDetailByStoreWithLanguageCache}_{ storeId}_{ languageId}_{startIndex}_{endIndex}",
                           products, CacheStatics.ProductDetailByStoreWithLanguageCacheTime);
            }
            else
            {
                await redisCache.SetAsync($"{CacheStatics.ProductDetailByStoreWithLanguageCache}_{ storeId}_{ languageId}",
                             products, CacheStatics.ProductDetailByStoreWithLanguageCacheTime);
            }


            return products;
        }

        public LanguageDTO GetStoreLanguage(int id)
        {
            var lang = languageService.LanguageById(id);
            return lang != null ? new LanguageDTO()
            {
                Abbrevation = lang.Abbrevation,
                Name = lang.Name
            } : null;
        }
    }
}
