using ProductDb.Mapping.BiggBrandDbModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProductDb.Services.ProductVariantServices
{
    public interface IProductVariantService
    {
        
        void Add(List<ProductVariantModel> productVariant);
        // new 
        IList<ProductVariantModel> GetProductsVariant(int id);
        string GetProductsVariantsAttribute(int id);
        IList<ProductVariantModel> GetProductsVariant(int id, List<int> IDs);
        IEnumerable<ProductVariantModel> AllProductVariant();
        IList<ProductModel> GetParentProductsVariantById(int id, List<int> IDs);
        IList<ProductModel> GetParentProductsNotVariantById(int id, List<int> IDs);
        ProductVariantModel ProductVariantByBaseId(int id);
        string PrepareAttributes(List<int> IDs);
        void ChangeProductVariant(int baseId,int productId);
        IEnumerable<ProductModel> AllProductVariantWithProduct();
        IEnumerable<ProductVariantModel> AllProductVariants();
        void ClearProductVariants(int id);
        ProductVariantModel GetProductVariantByProductId(int productId);
    }
}
