using ProductDb.Common.GlobalEntity;
using ProductDb.Mapping.BiggBrandDbModels;
using System.Collections.Generic;
using System.Linq;

namespace ProductDb.Services.ProductServices
{
    public interface IParentProductService
    {
        IQueryable<ParentProductModel> AllParentProductQueryable(int categoryId = 0);
        IEnumerable<ParentProductModel> AllParentProductsQueryableAsync(int categoryId, int take, int skip, out int total);
        ICollection<ParentProductModel> AllParentProducts();
        ICollection<ParentProductModel> AllActiveParentProducts();
        ParentProductModel ParentProductById(int id);
        ParentProductModel ParentProductBySku(string sku);
        ParentProductModel AddNewParentProduct(ParentProductModel model);
        ParentProductModel EditParentProduct(ParentProductModel model);
        int DeleteParentProductById(int id);
        int ParentProductsCountWithCategory(int categoryId);
        List<ChannelAdvisorParentProductModel> channelAdvisorParentProductModels(int store_id, int language_id,List<int> productIds );
        void ClearCache(string cacheKey);
    }
}