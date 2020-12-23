using ProductDb.ExportApi.Common;
using ProductDb.ExportApi.DTOs;
using ProductDb.ExportApi.Models;
using ProductDb.Mapping.BiggBrandDbModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductDb.ExportApi.Services
{
    public interface IIntegrationService
    {
        Task<IEnumerable<ProductDTO>> ProductDetailByStoreWithLanguage(int storeId, int languageId, int? startIndex, int? endIndex);
        // for mock tesing
        Task<IEnumerable<ProductDTO>> ProductDetailByStoreWithLanguageMock(int storeId, int languageId, int? startIndex, int? endIndex);
        Task<IEnumerable<ProductDTO>> ProductDetailByStoreWithLanguage(int storeId, int languageId, List<string> SKUs);
        ProductDTO ProductDetailById(int productId, int languageId);
        ProductDTO PrepareProductDTO(ProductModel product, int languageId);
        void UpdateStoreProduct(string sku, int storeId, int storeProductId);
        IEnumerable<StoreProductMappingModel> IntegrationSubNodesByStoreId(int storeId);
        string[] PrepareProductVariants(string[] ProductAttributes);
        Task<IEnumerable<StoreModel>> StoreModels();
        IEnumerable<ProductStockAndPriceDTO> ProductStockAndPriceByStoreId(int storeId);
        IEnumerable<ProductStockAndPriceDTO> PrepareStockAndPriceByStoreId(IEnumerable<ProductModel> products);
        LanguageDTO GetStoreLanguage(int id);
        void ClearCache(int storeId,int languageId);
    }
}
