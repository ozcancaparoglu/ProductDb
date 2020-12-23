using ProductDb.Common.GlobalEntity;
using ProductDb.Data.BiggBrandsDb;
using ProductDb.Mapping.BiggBrandDbModels;
using ProductDb.Mapping.BiggBrandDbModels.ProductDocksMapping;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductDb.Services.ProductServices
{
    public interface IProductService
    {
        ICollection<ProductModel> AllProducts();
        Task<ICollection<ProductModel>> AllProductsAsync(int id);
        IEnumerable<ProductModel> AllProductsQueryableAsync(int id, int take, int skip, out int total);
        IEnumerable<ProductModel> AllProductsQueryableAsync(int id, List<ProductModel> productList);
        IQueryable<ProductModel> AllProductQueryable();
        ProductModel ProductById(int id);
        ProductModel ProductBySku(string sku,bool isActive = true);
        int ProductStateById(int productId);
        ProductModel AddNewProduct(ProductModel model);
        ProductModel EditProduct(ProductModel model);
        bool DeleteProduct(int productId, out string exception);
        bool SetStateProduct(int productId, int state, out string exception);
        IEnumerable<ProductModel> ProductStockAndPriceByStoreId(int storeId);
        int ProductCount();
        ICollection<ProductModel> SearchProducts(int categoryId, int brandId, string sku, string title);
        bool ChangeProductsParent(int parentId, List<int> productIds);
        bool EditProductParent(int productId, int parentId);
        ICollection<ProductModel> ProductsNotInStore(int storeId);
        ProductModel ProductDetailById(int productId, int languageId);
        IEnumerable<ProductModel> ProductDetailByStoreWithLanguage(int storeId, int languageId, int? startIndex, int? endIndex);
        // Mock for tesing
        ProductModel ProductDetailByIdMock(int productId, int languageId,
            List<Product> prodList,
            List<ProductAttributeMappingModel> productMappings,
            List<AttributesValueModel> attributeValues,
            List<ProductAttributeValueModel> _pattributeValues,
            List<AttributesModel> attributes,
            List<LanguageValuesModel> LanguageValues);
        IEnumerable<ProductModel> ProductDetailByStoreWithLanguageMock(int storeId, int languageId, int? startIndex, int? endIndex);
        IEnumerable<ProductModel> ProductDetailByProductIDsStoreWithLanguage(List<string> productIDs, int storeId, int languageId);
        //Yapacağınız işe sokayım region'a alın servisteki gibi
        bool AddNewProductByProductDock(ProductDockModel productDockModel, string pictureName, int parentId);

        #region Integration
        //List<ChannelAdvisorProductViewModel> channelAdvisorProductVMs(int store_id, int language_id, List<int> productIds);
        List<ChannelAdvisorPriceUpdateModel> ChannelAdvisorPriceUpdateModel(int store_id);
        List<DCQuantityandID> UpdateQuantity(int store_id);
        List<EntegraXmlModel> EntegraProducts(int store_id, int language_id);
        List<JoomProductModel> joomProductModels(int store_id, int language_id);
        List<JoomUpdateModel> joomUpdateModels(int store_id, int language_id);
        List<ProductWithAttributeXMLModel> productWithAttributeXMLs(int store_id, int language_id);
        //List<ChannelAdvisorUpdateVM> channelAdvisorUpdateModels(int store_id, int language_id, int CAProductId, List<int> productIds);
        List<ChannelAdvisorProductViewModel> channelAdvisorProductVMParent(int store_id, int language_id, string parentSku, int storeParentId, int take, int skip);
        List<DCQuantityandID> UpdateQuantityAll(int store_id);
        List<IntegrationList> integrationLists(string sku, string brand, string category, string store);
        List<int> integrationListId(string sku, string brand, string category, string store);
        List<ChannelAdvisorProductViewModel> allChannelAdvisorProductVMs(int store_id, int language_id, int skip, int take);
        List<ChannelAdvisorProductViewModel> JsonchannelAdvisorProductVMs(int store_id, List<ProductJsonModel> productJsonModels, List<ProductStockAndPriceJsonModel> productStockAndPriceJsonModels);
        List<ChannelAdvisorUpdateVM> JsonchannelAdvisorUpdateModels(int store_id, List<ProductJsonModel> productJsonModels, List<ProductStockAndPriceJsonModel> productStockAndPriceJsonModels);
        List<string> GetProductSkubyId(List<int> productId);
        List<ChannelAdvisorUpdateVM> updateStoreCategory(int store_id);
        #endregion

        #region ForExportApi
        List<StoreProductMappingModel> GetSubStoreProductMappingByStoreId(int storeId);
        #endregion

        #region Product Cached
        ICollection<Product> AllProductWithDetails();
        ICollection<ProductAttributeMappingModel> ProductAttributeMapping();
        ICollection<AttributesValueModel> AttributeValues();
        ICollection<LanguageValuesModel> LanguageValues(int languageId);
        ICollection<LanguageModel> Languages();
        ICollection<ProductAttributeValueModel> ProductAttributeValues(int productId);
        #endregion

        #region Product Attributes

        ProductAttributeMappingModel ProductAttributeMappingById(int id);
        ICollection<ProductAttributeMappingModel> ArrangeCategoryProductAttributes(int parentProductId, int? productId);
        ICollection<ProductAttributeMappingModel> ArrangeRequiredProductAttributes(int? productId);
        ProductAttributeMappingModel AddNewProductAttributeMapping(int productId, int attributeId, int? attributeValueId, bool isRequired, string requiredAttributeValue = null);
        int AddRangeProductAttributeMapping(List<ProductAttributeMappingModel> productAttributeMappingsModel, int entityId);
        bool EditProductAttributeMapping(List<ProductAttributeMappingModel> productAttributeMappingModels, int entityId);
        int DeleteProductAttributeMapping(ProductAttributeMappingModel productAttributeMappingModel);
        int DeleteProductAttributeMappingWithIdReturnProductId(int productAttributeMappingId);

        void UpdateProductImport(string path, int languageId);
        ICollection<ProductGroupModel> ProductGroups();

        #endregion

        #region Product Attributes Value

        int ArrangeProductAttributeValue(int productId);
        int DeleteProductAttributeValue(int productId, string attributeName);

        #endregion

        #region Prouduct Dock
        bool AddNewProductByProductDock(ProductDockModel productDockModel, int? parentId);
        bool DeleteProductDock(int id);
        #endregion
        void AddProductGroup(ProductGroupModel productGroup);
        void EditProductGroup(ProductGroupModel productGroup);
        void DeleteProductGroup(ProductGroupModel productGroup);
        ICollection<StoreProductMappingModel> ProductPricesWithStore(int productId);
    }
}