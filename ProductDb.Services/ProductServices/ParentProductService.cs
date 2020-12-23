using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using ProductDb.Common.Enums;
using ProductDb.Common.GlobalEntity;
using ProductDb.Data.BiggBrandsDb;
using ProductDb.Mapping.AutoMapperConfigurations;
using ProductDb.Mapping.BiggBrandDbModels;
using ProductDb.Repositories.BaseRepositories;
using ProductDb.Repositories.UnitOfWorks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ProductDb.Services.ProductServices
{
    public class ParentProductService : IParentProductService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IAutoMapperConfiguration autoMapper;
        private readonly IMemoryCache memoryCache;

        private readonly IGenericRepository<ParentProduct> parentProductRepo;
        private readonly IGenericRepository<Product> productRepo;
        private readonly IGenericRepository<ProductAttributeMapping> productAttributeRepo;
        private readonly IGenericRepository<ProductAttributeValue> productAttributeValueRepo;
        private readonly IGenericRepository<Category> categoryRepo;
        private readonly IGenericRepository<CategoryAttributeMapping> categoryAttributeRepo;
        private readonly IGenericRepository<StoreProductMapping> storeProductMappingRepo;
        private readonly IGenericRepository<WarehouseProductStock> warehouseProductStockRepo;
        private readonly IGenericRepository<StoreWarehouseMapping> storeWarehouseRepo;
        private readonly IGenericRepository<LanguageValues> languageValuesRepo;
        private readonly IGenericRepository<Attributes> attributesRepo;
        private readonly IGenericRepository<ProductVariant> productVariantRepo;
        private readonly Expression<Func<ParentProduct, bool>> defaultParentProductFilter = null;

        const string cacheKey = "parentProductListKey";

        public ParentProductService(IUnitOfWork unitOfWork, IAutoMapperConfiguration autoMapper, IMemoryCache memoryCache)
        {
            this.unitOfWork = unitOfWork;
            this.autoMapper = autoMapper;
            this.memoryCache = memoryCache;

            productVariantRepo = this.unitOfWork.Repository<ProductVariant>();
            productRepo = this.unitOfWork.Repository<Product>();
            productAttributeRepo = this.unitOfWork.Repository<ProductAttributeMapping>();
            parentProductRepo = this.unitOfWork.Repository<ParentProduct>();
            productAttributeValueRepo = this.unitOfWork.Repository<ProductAttributeValue>();
            categoryRepo = this.unitOfWork.Repository<Category>();
            categoryAttributeRepo = this.unitOfWork.Repository<CategoryAttributeMapping>();
            storeProductMappingRepo = this.unitOfWork.Repository<StoreProductMapping>();
            warehouseProductStockRepo = this.unitOfWork.Repository<WarehouseProductStock>();
            languageValuesRepo = this.unitOfWork.Repository<LanguageValues>();
            storeWarehouseRepo = this.unitOfWork.Repository<StoreWarehouseMapping>();
            parentProductRepo = this.unitOfWork.Repository<ParentProduct>();
            attributesRepo = this.unitOfWork.Repository<Attributes>();
            defaultParentProductFilter = entity => entity.State == (int)State.Active;

        }

        public IQueryable<ParentProductModel> AllParentProductQueryable(int categoryId = 0)
        {
            var query = AllParentProducts().AsQueryable();

            if (categoryId != 0)
                query = query.Where(x => x.CategoryId == categoryId);

            return query;
        }

        public IEnumerable<ParentProductModel> AllParentProductsQueryableAsync(int categoryId, int take, int skip, out int total)
        {
            // query
            var query = AllParentProductQueryable(categoryId);
            // total count
            total = query.Count();
            //return null;
            var datas = query.Skip(skip).Take(take).OrderByDescending(x => x.CreatedDate).ToList();

            return datas;
        }

        public ICollection<ParentProductModel> AllParentProducts()
        {
            if (!memoryCache.TryGetValue(cacheKey, out ICollection<ParentProductModel> parentProducts))
            {

                var parentProductsEntity = parentProductRepo.Table()
                    .Include(x => x.Products)
                    .Include(x => x.Category)
                    .Select(s => new ParentProduct
                    {
                        Id = s.Id,
                        Sku = s.Sku,
                        Title = s.Title,
                        CategoryId = s.CategoryId,
                        Products = s.Products,
                        State = s.State
                    });

                parentProducts = autoMapper.MapCollection<ParentProduct, ParentProductModel>(parentProductsEntity).ToList();
                foreach (var parentProduct in parentProducts)
                {
                    parentProduct.SkuList = string.Join(", ", parentProduct.Products.Select(x => x.Sku));
                }

                var cacheExpirationOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                    Priority = CacheItemPriority.High
                };
                memoryCache.Set(cacheKey, parentProducts, cacheExpirationOptions);
            }

            return parentProducts;
        }

        public ICollection<ParentProductModel> AllActiveParentProducts()
        {
            return autoMapper.MapCollection<ParentProduct, ParentProductModel>(parentProductRepo.FindAll(defaultParentProductFilter)).ToList();
        }

        public ParentProductModel ParentProductById(int id)
        {
            return autoMapper.MapObject<ParentProduct, ParentProductModel>(parentProductRepo.Filter(x => x.Id == id, null, "Category,SupplierPP,BrandPP").FirstOrDefault());
        }

        public ParentProductModel ParentProductBySku(string sku)
        {
            return autoMapper.MapObject<ParentProduct, ParentProductModel>(parentProductRepo.Filter(x => x.Sku.Equals(sku), null, "Category,SupplierPP,BrandPP").FirstOrDefault());
        }

        public ParentProductModel AddNewParentProduct(ParentProductModel model)
        {
            var entity = autoMapper.MapObject<ParentProductModel, ParentProduct>(model);

            if (parentProductRepo.Exist(x => x.Sku.ToLowerInvariant() == entity.Sku.ToLowerInvariant()))
                return ParentProductBySku(entity.Sku);

            var savedEntity = parentProductRepo.Add(entity);

            ClearCache(cacheKey);

            return autoMapper.MapObject<ParentProduct, ParentProductModel>(savedEntity);

        }

        public ParentProductModel EditParentProduct(ParentProductModel model)
        {
            var entity = autoMapper.MapObject<ParentProductModel, ParentProduct>(model);

            var updatedEntity = parentProductRepo.Update(entity);

            ClearCache(cacheKey);

            return autoMapper.MapObject<ParentProduct, ParentProductModel>(updatedEntity);
        }

        public int DeleteParentProductById(int id)
        {
            if (productRepo.Exist(x => x.ParentProductId == id))
                return -1;
            try
            {
                var entity = parentProductRepo.GetById(id);

                return parentProductRepo.Delete(entity);
            }
            catch { return -1; }

        }

        public int ParentProductsCountWithCategory(int categoryId)
        {
           return parentProductRepo.Filter(x => x.CategoryId == categoryId).Count();
        }

        #region integration

        public List<ChannelAdvisorParentProductModel> channelAdvisorParentProductModels(int store_id, int language_id, List<int> productIds)
        {
            var storeProduct = storeProductMappingRepo.Table().Where(x => x.StoreId == store_id && productIds.Any(y => y == x.ProductId));

            var productQuery = productRepo.Table()
                 .Where(x => storeProduct.Any(y => y.ProductId == x.Id));

            var categoryAttributeMapping = categoryAttributeRepo.Table().Where(x => !x.IsRequired).Include(x => x.Attributes).ToList();


            var query = parentProductRepo.Table()
                .Where(x => productQuery.Any(y => y.ParentProductId == x.Id))
                    .Select(x => new ChannelAdvisorParentProductModel
                    {
                        Sku = x.Sku,
                        Title = x.Title,
                        Brand = x.BrandPP.Name,
                        Description = x.Description,
                        Manufacturer = x.SupplierPP.Name,
                        ShortDescription = x.ShortDescription,
                        BuyItNowPrice = storeProduct.FirstOrDefault(y => y.Id == productQuery.FirstOrDefault(z => z.ParentProductId == x.Id).Id).Price.Value,
                        RelationshipName = string.Join("-", categoryAttributeMapping.Where(y => y.CategoryId == x.CategoryId).Select(y => y.Attributes.Name).ToArray())

                    }).ToList();

            return query;
        }
        #endregion 

        public ParentProductModel SearchParentProduct(int id, string sku, string title, int brandId)
        {
            throw new NotImplementedException();
        }

        public void ClearCache(string cacheKey)
        {
            memoryCache.Remove(cacheKey);
        }


    }
}
