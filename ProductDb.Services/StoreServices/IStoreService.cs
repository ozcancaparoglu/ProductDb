using ProductDb.Common.Enums;
using ProductDb.Common.GlobalEntity;
using ProductDb.Mapping.BiggBrandDbModels;
using System.Collections.Generic;
using System.Linq;

namespace ProductDb.Services.StoreServices
{
    public interface IStoreService
    {
        IQueryable<StoreModel> AllStoresQuerable(out int total);
        ICollection<StoreModel> AllStores();
        ICollection<StoreModel> AllActiveStores();
        StoreModel StoreById(int id);
        StoreModel AddNewStore(StoreModel model);
        StoreModel EditStore(StoreModel model);
        int StoreCount();
        StoreModel StoreWithCurrency(int id);

        #region StoreWarehouse

        int AddNewStoreWarehouses(List<StoreWarehouseMappingModel> model);
        ICollection<StoreWarehouseMappingModel> StoreWarehouses(int id);
        int EditStoreWarehouses(List<StoreWarehouseMappingModel> model, int id);
        StoreTypeModel SetStoreTypeState(int id, int state);

        #endregion

        #region StoreType

        StoreTypeModel StoreTypeById(int id);
        ICollection<StoreTypeModel> AllStoreTypes();
        StoreTypeModel AddNewStoreType(StoreTypeModel model);
        StoreTypeModel EditStoreType(StoreTypeModel model);

        #endregion

        #region StoreProducts
        bool SyncProductCategories(int storeId, out string exceptionMessage);
        void AddNewStoreProductMappingRange(int storeId, List<int> productIds);
        void RemoveFromStoreProductMappingRange(int storeId, List<int> productIds);
        void AddNewStoreProductMappingRangeCopy(int storeId, List<int> productIds, int userId);
        void AddNewStoreCategoryRangeCopy(int storeId, int targetStoreId, int userId);
        void AddNewStoreMarginRangeCopy(int storeId, int targetStoreId, int userId);
        void AddNewStoreCargoRangeCopy(int storeId, int targetStoreId, int userId);
        void AddNewStoreTransportationRangeCopy(int storeId, int targetStoreId, int userId);
        ICollection<StoreProductMappingModel> StoreProducts(int storeId);
        ICollection<StoreProductMappingModel> StoreProducts(int storeId,List<int> AttributeIDs);
        ICollection<StoreProductMappingModel> StoreProducts(int storeId,int take,int skip);
        int DeleteStoreProduct(int id);
        void UpdateStoreProducts();
        void ChangeStoreProduct(string sku,int storeId,int storeProductId, int storeParentProductId);
        void ChangeStoreProduct(int storeId, int storeProductId);
        bool AddStoreProduct(List<ProductInfo> productInfos, int storeId, int productCount);
        StoreProductMappingModel UpdateStoreProductMapping(StoreProductMappingModel storeProduct);
        List<StoreCategoryMappingModel> StoreCategoryByStoreId(int id);
        void AddStoreCategory(StoreCategoryMappingModel storeCategory);
        void EditStoreCategory(StoreCategoryMappingModel storeCategory);
        void DeleteStoreCategory(StoreCategoryMappingModel storeCategory);
        void UpdateStoreProductCategory(StoreCategoryMappingModel storeCategory, StoreCategoryState categoryState);
        Dictionary<int, string> StoreProductCategoryERPId(int storeId, List<int> productIDs);
        #endregion
    }
}