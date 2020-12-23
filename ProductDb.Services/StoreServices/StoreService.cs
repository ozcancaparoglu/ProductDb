using Microsoft.EntityFrameworkCore;
using ProductDb.Common.Cache;
using ProductDb.Common.Enums;
using ProductDb.Common.GlobalEntity;
using ProductDb.Data.BiggBrandsDb;
using ProductDb.Mapping.AutoMapperConfigurations;
using ProductDb.Mapping.BiggBrandDbModels;
using ProductDb.Repositories.BaseRepositories;
using ProductDb.Repositories.UnitOfWorks;
using ProductDb.Services.CategoryServices;
using ProductDb.Services.ProductServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace ProductDb.Services.StoreServices
{
    public class StoreService : IStoreService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IAutoMapperConfiguration autoMapper;
        private readonly ICacheManager cacheManager;
        private readonly ICategoryService categoryService;
        private readonly IProductService productService;

        private readonly IGenericRepository<Product> productRepo;
        private readonly IGenericRepository<Store> storeRepo;
        private readonly IGenericRepository<StoreProductMapping> storeProductRepo;
        private readonly IGenericRepository<StoreWarehouseMapping> storeWarehouseRepo;
        private readonly IGenericRepository<WarehouseProductStock> warehouseProductRepo;
        private readonly IGenericRepository<ParentProduct> parentRepo;
        private readonly IGenericRepository<ProductAttributeMapping> productMappingRepo;
        private readonly IGenericRepository<StoreCategoryMapping> storeCategoryMappingRepo;
        private readonly IGenericRepository<Margin> marginRepo;
        private readonly IGenericRepository<Cargo> cargoRepo;
        private readonly IGenericRepository<Transportation> transportationRepo;
        private readonly IGenericRepository<StoreType> storeTypeRepo;


        public StoreService(IUnitOfWork unitOfWork, IAutoMapperConfiguration autoMapper, ICacheManager cacheManager, ICategoryService categoryService, IProductService productService)
        {
            this.unitOfWork = unitOfWork;
            this.autoMapper = autoMapper;
            this.cacheManager = cacheManager;
            this.categoryService = categoryService;
            this.productService = productService;

            storeRepo = this.unitOfWork.Repository<Store>();
            storeProductRepo = this.unitOfWork.Repository<StoreProductMapping>();
            storeWarehouseRepo = this.unitOfWork.Repository<StoreWarehouseMapping>();
            warehouseProductRepo = this.unitOfWork.Repository<WarehouseProductStock>();
            productRepo = this.unitOfWork.Repository<Product>();
            parentRepo = this.unitOfWork.Repository<ParentProduct>();
            productMappingRepo = this.unitOfWork.Repository<ProductAttributeMapping>();
            storeCategoryMappingRepo = this.unitOfWork.Repository<StoreCategoryMapping>();
            marginRepo = this.unitOfWork.Repository<Margin>();
            cargoRepo = this.unitOfWork.Repository<Cargo>();
            transportationRepo = this.unitOfWork.Repository<Transportation>();
            storeTypeRepo = this.unitOfWork.Repository<StoreType>();
        }

        public IQueryable<StoreModel> AllStoresQuerable(out int total)
        {
            var query = (storeRepo.Table().Include("StoreType").Where(x => x.State == (int)State.Active)
                .Select(x => new StoreModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    StoreTypeString = x.StoreType == null ? string.Empty : x.StoreType.Name
                }).OrderByDescending(x => x.CreatedDate));

            total = query.Count();

            return query;
        }

        public ICollection<StoreModel> AllStores()
        {
            return autoMapper.MapCollection<Store, StoreModel>(storeRepo.GetAll()).OrderBy(x => x.Name).ToList();
        }

        public ICollection<StoreModel> AllActiveStores()
        {
            return autoMapper.MapCollection<Store, StoreModel>(storeRepo.FindAll(x => x.State == (int)State.Active)).OrderBy(x => x.Name).ToList();
        }

        public StoreModel StoreById(int id)
        {
            return autoMapper.MapObject<Store, StoreModel>(storeRepo.GetById(id));
        }

        public StoreModel AddNewStore(StoreModel model)
        {
            var entity = autoMapper.MapObject<StoreModel, Store>(model);

            var savedEntity = storeRepo.Add(entity);

            return autoMapper.MapObject<Store, StoreModel>(savedEntity);

        }

        public StoreModel EditStore(StoreModel model)
        {
            var entity = autoMapper.MapObject<StoreModel, Store>(model);

            var updatedEntity = storeRepo.Update(entity);

            return autoMapper.MapObject<Store, StoreModel>(updatedEntity);
        }

        public int StoreCount()
        {
            return storeRepo.Count();
        }

        public StoreModel StoreWithCurrency(int id)
        {
            return autoMapper.MapObject<Store, StoreModel>(storeRepo.Filter(x => x.Id == id, null, "Currency").FirstOrDefault());
        }

        #region StoreWarehouse

        public int AddNewStoreWarehouses(List<StoreWarehouseMappingModel> model)
        {
            var entities = autoMapper.MapCollection<StoreWarehouseMappingModel, StoreWarehouseMapping>(model).ToList();

            return storeWarehouseRepo.AddRange(entities);
        }

        public ICollection<StoreWarehouseMappingModel> StoreWarehouses(int id)
        {
            return autoMapper.MapCollection<StoreWarehouseMapping, StoreWarehouseMappingModel>(storeWarehouseRepo.FindAll(x => x.StoreId == id)).ToList();
        }

        public int EditStoreWarehouses(List<StoreWarehouseMappingModel> model, int id)
        {
            //TODO: Refactor
            var entities = autoMapper.MapCollection<StoreWarehouseMappingModel, StoreWarehouseMapping>(model).ToList();

            var dbEntities = storeWarehouseRepo.FindAll(x => x.StoreId == id).ToList();

            storeWarehouseRepo.DeleteRange(dbEntities);

            return storeWarehouseRepo.AddRange(entities);
        }

        #endregion

        #region StoreType

        public StoreTypeModel StoreTypeById(int id)
        {
            return autoMapper.MapObject<StoreType, StoreTypeModel>(storeTypeRepo.GetById(id));
        }

        public ICollection<StoreTypeModel> AllStoreTypes()
        {
            return autoMapper.MapCollection<StoreType, StoreTypeModel>(storeTypeRepo.GetAll()).OrderBy(x => x.Name).ToList();
        }
        public StoreTypeModel AddNewStoreType(StoreTypeModel model)
        {
            var entity = autoMapper.MapObject<StoreTypeModel, StoreType>(model);

            var savedEntity = storeTypeRepo.Add(entity);

            return autoMapper.MapObject<StoreType, StoreTypeModel>(savedEntity);

        }

        public StoreTypeModel EditStoreType(StoreTypeModel model)
        {
            var entity = autoMapper.MapObject<StoreTypeModel, StoreType>(model);

            var updatedEntity = storeTypeRepo.Update(entity);

            return autoMapper.MapObject<StoreType, StoreTypeModel>(updatedEntity);
        }

        public StoreTypeModel SetStoreTypeState(int id, int state)
        {
            var entity = storeTypeRepo.GetById(id);
            
            entity.State = state;
            
            return autoMapper.MapObject<StoreType, StoreTypeModel>(storeTypeRepo.Update(entity));
        }

        #endregion

        #region StoreProducts

        public void AddNewStoreProductMappingRange(int storeId, List<int> productIds)
        {
            var storeProductMappingList = new List<StoreProductMapping>();

            var existingsProducts = storeProductRepo.Filter(x => x.StoreId == storeId && productIds.Contains(x.ProductId.Value)).Select(s => s.ProductId.Value).ToList();

            foreach (var productId in productIds.Except(existingsProducts))
            {
                storeProductMappingList.Add(new StoreProductMapping
                {
                    StoreId = storeId,
                    ProductId = productId,
                    State = productService.ProductStateById(productId)
                });
            }
            
            storeProductRepo.BulkInsertOrUpdate(storeProductMappingList);

            Thread.Sleep(1000);

            // Store Category Update
            var _storeCategories = StoreProductCategoryERPId(storeId, productIds);
            var storeProductCategory = new List<StoreProductMapping>();
            foreach (var item in _storeCategories)
            {
                var storeProduct = storeProductRepo.Table().FirstOrDefault(x => x.ProductId == item.Key && x.StoreId == storeId);
                if (storeProduct != null)
                {
                    storeProduct.StoreCategory = item.Value;
                    storeProductCategory.Add(storeProduct);
                }
            }

            storeProductRepo.BulkUpdate(storeProductCategory);

            Thread.Sleep(1000);

        }

        public bool SyncProductCategories(int storeId, out string exceptionMessage)
        {
            try
            {
                exceptionMessage = string.Empty;
                storeProductRepo.ExecuteSqlCommand($"sp_UpdateStoreCategoryByStoreId {storeId}");
                return true;
            }
            catch (Exception ex)
            {
                exceptionMessage = ex.Message;
                return false;
            }
            
        }

        public void RemoveFromStoreProductMappingRange(int storeId, List<int> productIds)
        {
            var existingsProducts = storeProductRepo.Filter(x => x.StoreId == storeId && productIds.Contains(x.ProductId.Value)).ToList();

            storeProductRepo.BulkDelete(existingsProducts);

            Thread.Sleep(1000);
        }


        public void AddNewStoreProductMappingRangeCopy(int storeId, List<int> productIds, int userId)
        {
            var date = DateTime.Now;
            var storeProductsToBeInserted = new List<StoreProductMapping>();
            var storeProductIdsAlreadyInserted = storeProductRepo.Filter(x => productIds.Contains(x.ProductId.Value) && x.StoreId == storeId).Select(s => s.ProductId.Value);
            productIds = productIds.Except(storeProductIdsAlreadyInserted).ToList();

            foreach (var productId in productIds)
            {
                storeProductsToBeInserted.Add(new StoreProductMapping
                {
                    StoreId = storeId,
                    ProductId = productId,
                    CreatedDate = date,
                    StoreProductId = 0,
                    StoreParentProductId = 0,
                    BaseStoreId = 0,
                    IsRealStock = true,
                    State = productService.ProductStateById(productId),
                    ProcessedBy = userId
                });
            }
            storeProductRepo.BulkInsert(storeProductsToBeInserted);

            Thread.Sleep(500);

            var query = storeProductRepo.ExecuteSqlCommand($"sp_UpdateStoreCategoryByStoreId {storeId}");
        }

        public void AddNewStoreCategoryRangeCopy(int storeId, int targetStoreId, int userId)
        {
            var existingCategoryMappings = storeCategoryMappingRepo.Filter(x => x.StoreId == storeId).ToList();

            if (existingCategoryMappings.Count > 0)
            {
                var targetStoreCategoryMappings = storeCategoryMappingRepo.Filter(x => x.StoreId == targetStoreId);
                storeCategoryMappingRepo.BulkDelete(targetStoreCategoryMappings.ToList());

                var date = DateTime.Now;

                var targetCategoryMappings = new List<StoreCategoryMapping>();

                foreach (var item in existingCategoryMappings)
                {
                    item.Id = 0;
                    item.StoreId = targetStoreId;
                    item.CreatedDate = date;
                    item.ProcessedBy = userId;
                    targetCategoryMappings.Add(item);
                }

                storeCategoryMappingRepo.BulkInsert(targetCategoryMappings);
            }
        }


        public void AddNewStoreMarginRangeCopy(int storeId, int targetStoreId, int userId)
        {
            var existingMargins = marginRepo.Filter(x => x.StoreId == storeId).ToList();

            if (existingMargins.Count > 0)
            {
                var targetStoreMargins = marginRepo.Filter(x => x.StoreId == targetStoreId);
                marginRepo.BulkDelete(targetStoreMargins.ToList());

                var date = DateTime.Now;

                var targetMargins = new List<Margin>();

                foreach (var item in existingMargins)
                {
                    item.Id = 0;
                    item.StoreId = targetStoreId;
                    item.CreatedDate = date;
                    item.ProcessedBy = userId;
                    targetMargins.Add(item);
                }

                marginRepo.BulkInsert(targetMargins);
            }
        }

        public void AddNewStoreCargoRangeCopy(int storeId, int targetStoreId, int userId)
        {
            var existingCargo = cargoRepo.Filter(x => x.StoreId == storeId).ToList();

            if (existingCargo.Count > 0)
            {
                var targetStoreCargo = cargoRepo.Filter(x => x.StoreId == targetStoreId);
                cargoRepo.BulkDelete(targetStoreCargo.ToList());

                var date = DateTime.Now;

                var targetCargo = new List<Cargo>();

                foreach (var item in existingCargo)
                {
                    item.Id = 0;
                    item.StoreId = targetStoreId;
                    item.CreatedDate = date;
                    item.ProcessedBy = userId;
                    targetCargo.Add(item);
                }

                cargoRepo.BulkInsert(targetCargo);
            }
        }

        public void AddNewStoreTransportationRangeCopy(int storeId, int targetStoreId, int userId)
        {
            var existingTransportation = transportationRepo.Filter(x => x.StoreId == storeId).ToList();

            if (existingTransportation.Count > 0)
            {
                var targetStoreTransportation = transportationRepo.Filter(x => x.StoreId == targetStoreId);
                transportationRepo.BulkDelete(targetStoreTransportation.ToList());

                var date = DateTime.Now;

                var targetTransportation = new List<Transportation>();

                foreach (var item in existingTransportation)
                {
                    item.Id = 0;
                    item.StoreId = targetStoreId;
                    item.CreatedDate = date;
                    item.ProcessedBy = userId;
                    targetTransportation.Add(item);
                }

                transportationRepo.BulkInsert(targetTransportation); 
            }
        }

        public ICollection<StoreProductMappingModel> StoreProducts(int storeId)
        {
            var _list = storeProductRepo.Table().Where(x => x.StoreId == storeId).Include(x => x.Product)
                .ThenInclude(x => x.ParentProduct.Category).Include(x => x.Store.Currency).Include(x => x.Product.VatRate).ToList();
            var _returnObject = autoMapper.MapCollection<StoreProductMapping, StoreProductMappingModel>(_list).ToList();

            // category with parents
            _returnObject.ForEach(x => x.CategoryWithParents = categoryService.CategoryWithParentNames(x.Product.ParentProduct.CategoryId.Value));
            _returnObject.ForEach(x => x.ParentProductSku = x.Product.ParentProduct.Sku);
            _returnObject.ForEach(x => x.Product.ParentProduct = null);

            return _returnObject;
        }

        public int DeleteStoreProduct(int id)
        {
            var entity = storeProductRepo.GetById(id);
            return storeProductRepo.Delete(entity);
        }

        public void UpdateStoreProducts()
        {
            var storeWarehouseMapping = storeWarehouseRepo.Filter(x => x.State == (int)State.Active).ToList();
            var storeProducts = storeProductRepo.Filter(x => x.State == (int)State.Active).ToList();
            var warehouseProductStocks = warehouseProductRepo.Filter(x => x.State == (int)State.Active).ToList();

            if (storeProducts != null)
            {
                foreach (var item in storeProducts)
                {
                    var warehouseTypes = storeWarehouseMapping.Where(x => x.StoreId == item.StoreId).Select(x => x.WarehouseTypeId).ToList();
                    var stock = warehouseProductStocks.Where(x => warehouseTypes.Contains(x.WarehouseTypeId) && x.ProductId == item.ProductId).Select(s => s.Quantity).Sum();
                    if (item.Stock != Convert.ToInt32(stock))
                        item.UpdatedDate = DateTime.Now;
                    item.Stock = Convert.ToInt32(stock);
                }

                storeProductRepo.BulkUpdate(storeProducts);
            }
        }

        public void ChangeStoreProduct(string sku, int storeId, int storeProductId, int storeParentProductId)
        {
            var productId = productRepo.Find(x => x.Sku == sku).Id;

            var storeproduct = storeProductRepo.Find(x => x.ProductId == productId && x.StoreId == storeId);

            storeproduct.StoreProductId = storeProductId;
            storeproduct.StoreParentProductId = storeParentProductId;

            var result = storeProductRepo.Update(storeproduct);
        }

        public void ChangeStoreProduct(int productId, int storeId, int storeProductId, int storeParentProductId)
        {

            var storeproduct = storeProductRepo.Find(x => x.ProductId == productId && x.StoreId == storeId);

            var subStoreId = storeProductRepo.Table().FirstOrDefault(x => x.BaseStoreId == storeId).StoreId;

            if (subStoreId != null)
            {
                var substoreproduct = storeProductRepo.Find(x => x.ProductId == productId && x.StoreId == subStoreId);

                substoreproduct.StoreProductId = storeProductId;
                substoreproduct.StoreParentProductId = storeParentProductId;

                var subresult = storeProductRepo.Update(storeproduct);
            }

            storeproduct.StoreProductId = storeProductId;
            storeproduct.StoreParentProductId = storeParentProductId;

            var result = storeProductRepo.Update(storeproduct);
        }

        public void ChangeStoreProduct(int storeId, int storeProductId)
        {
            var productId = storeProductRepo.Find(x => x.StoreProductId == storeProductId).ProductId;

            var storeproduct = storeProductRepo.Find(x => x.ProductId == productId && x.StoreId == storeId);

            storeproduct.StoreProductId = storeProductId;

            var result = storeProductRepo.Update(storeproduct);
        }

        public bool AddStoreProduct(List<ProductInfo> productInfos, int storeId, int productCount)
        {
            var storeProduct = storeProductRepo.Table().Where(x => x.StoreId == storeId && x.StoreProductId == 0);

            var productId = productRepo.Table().Where(x => productInfos.Any(y => y.Sku == x.Sku) && storeProduct.Any(y => y.ProductId == x.Id)).ToList();

            List<StoreProductMapping> storeProductMappingList = new List<StoreProductMapping>();

            foreach (var item in productId)
            {
                var product = storeProduct.FirstOrDefault(x => x.ProductId == item.Id);

                if (product == null)
                    continue;

                product.StoreProductId = productInfos.FirstOrDefault(x => x.Sku == item.Sku).ID;

                storeProductRepo.Update(product);
            }
            return true;
        }

        public ICollection<StoreProductMappingModel> StoreProducts(int storeId, int take, int skip)
        {

            var _list = storeProductRepo.Table().Where(x => x.StoreId == storeId).Include(x => x.Product).Skip(skip).Take(take);

            var _returnObject = autoMapper.MapCollection<StoreProductMapping, StoreProductMappingModel>(_list).ToList();

            return _returnObject;
        }

        public ICollection<StoreProductMappingModel> StoreProducts(int storeId, List<int> AttributeIDs)
        {
            var _list = storeProductRepo.Table().Where(x => x.StoreId == storeId).Include(x => x.Product)
                        .ThenInclude(x => x.ProductAttributeMappings).ToList();

            List<StoreProductMapping> list = new List<StoreProductMapping>();

            for (int i = 0; i < _list.Count; i++)
            {
                var data = _list[i].Product.ProductAttributeMappings.Select(x => AttributeIDs.Any(k => k == x.AttributesId));

            }

            _list = _list.Where(x => x.Product.ProductAttributeMappings.Any(k => AttributeIDs.Any(m => m == k.AttributesId))).ToList();
            var _returnObject = autoMapper.MapCollection<StoreProductMapping, StoreProductMappingModel>(_list).ToList();

            return _returnObject;
        }

        public StoreProductMappingModel UpdateStoreProductMapping(StoreProductMappingModel storeProduct)
        {
            var prod = storeProduct.Product;
            storeProduct.Product = null;
            storeProductRepo.Update(autoMapper.MapObject<StoreProductMappingModel, StoreProductMapping>(storeProduct));
            storeProduct.Product = prod;
            return storeProduct;
        }

        public List<StoreCategoryMappingModel> StoreCategoryByStoreId(int id)
        {
            var allCats = categoryService.AllActiveCategoriesWithParentNames().ToList();

            var datas = storeCategoryMappingRepo.Filter(x => x.StoreId == id, null, "Store,Category");
            var dataModel = autoMapper.MapCollection<StoreCategoryMapping, StoreCategoryMappingModel>(datas).ToList();
            return dataModel.Select(x => new StoreCategoryMappingModel
            {
                Id = x.Id,
                Category = allCats.Where(k => k.Id == x.CategoryId).FirstOrDefault(),
                CategoryId = x.CategoryId,
                ErpCategoryId = x.ErpCategoryId,
                StoreId = x.StoreId
            }).ToList();
        }

        public void AddStoreCategory(StoreCategoryMappingModel storeCategory)
        {
            var data = storeCategoryMappingRepo.Filter(x => x.StoreId == storeCategory.StoreId && x.CategoryId == storeCategory.CategoryId).FirstOrDefault();
            if (data == null)
            {
                storeCategoryMappingRepo.Add(autoMapper.MapObject<StoreCategoryMappingModel, StoreCategoryMapping>(storeCategory));
                UpdateStoreProductCategory(storeCategory);
            }
        }

        public void EditStoreCategory(StoreCategoryMappingModel storeCategory)
        {
            UpdateStoreProductCategory(storeCategory, StoreCategoryState.Update);
            storeCategoryMappingRepo.Update(autoMapper.MapObject<StoreCategoryMappingModel, StoreCategoryMapping>(storeCategory));
        }

        public void DeleteStoreCategory(StoreCategoryMappingModel storeCategory)
        {
            UpdateStoreProductCategory(storeCategory, StoreCategoryState.Delete);
            storeCategoryMappingRepo.Delete(autoMapper.MapObject<StoreCategoryMappingModel, StoreCategoryMapping>(storeCategory));

        }

        public void UpdateStoreProductCategory(StoreCategoryMappingModel storeCategory, StoreCategoryState categoryState = StoreCategoryState.Add)
        {
            var baseCategory = storeCategoryMappingRepo.Filter(x => x.StoreId == storeCategory.StoreId && x.CategoryId
            == storeCategory.CategoryId).FirstOrDefault();

            if (baseCategory != null)
            {
                var products = storeProductRepo.Table().Where(x => x.StoreId == storeCategory.StoreId).Include(x => x.Product)
                            .ThenInclude(k => k.ParentProduct).ThenInclude(m => m.Category).ToList();

                products = products.Where(m => m.Product.ParentProduct.CategoryId == storeCategory.CategoryId).ToList();

                var count = products.Count;
                var catId = storeCategory.ErpCategoryId.ToString();

                List<StoreProductMapping> updatedlist = new List<StoreProductMapping>();

                for (int i = 0; i < count; i++)
                {
                    var _storeCategory = products[i].StoreCategory;

                    if (_storeCategory != null)
                    {
                        var categories = _storeCategory.Split(',');
                        var catCount = categories.Length;
                        var index = Array.IndexOf(categories, baseCategory.ErpCategoryId.ToString());
                        if (index > -1)
                        {
                            switch (categoryState)
                            {
                                case StoreCategoryState.Delete:

                                    categories = categories.Where(x => x != categories[index]).ToArray();
                                    if (categories.Count() == 0)
                                    {
                                        Array.Resize(ref categories, categories.Length + 1);
                                        categories[categories.Length - 1] = "0";
                                    }
                                    break;
                                case StoreCategoryState.Update:
                                    categories[index] = catId;
                                    break;
                            }
                        }
                        var newCategories = string.Join(",", categories);
                        products[i].StoreCategory = newCategories;
                    }
                    else
                    {
                        products[i].StoreCategory = $"{catId}";
                    }
                    updatedlist.Add(products[i]);
                }
                storeProductRepo.BulkUpdate(updatedlist);
            }
        }

        public Dictionary<int, string> StoreProductCategoryERPId(int storeId, List<int> productIDs)
        {
            Dictionary<int, string> keyValuePairs = new Dictionary<int, string>();

            var storeCategoryList = storeCategoryMappingRepo.Filter(x => x.StoreId == storeId).ToList();

            foreach (var productId in productIDs)
            {
                var productCategory = productRepo.Filter(x => x.Id == productId, null, "ParentProduct").FirstOrDefault();
                var erpCategory = storeCategoryList.Where(k => k.CategoryId == productCategory.ParentProduct.CategoryId).FirstOrDefault();

                keyValuePairs.Add(productId, erpCategory == null ? "0" : $"{erpCategory.ErpCategoryId}");
            }
            return keyValuePairs;
        }

        public ICollection<StoreProductMappingModel> StoreProductStocks(int storeId)
        {
            return autoMapper.MapCollection<StoreProductMapping, StoreProductMappingModel>(storeProductRepo.Filter(x => x.StoreId == storeId, null, "Product")).ToList();

        }

        #endregion


    }
}
