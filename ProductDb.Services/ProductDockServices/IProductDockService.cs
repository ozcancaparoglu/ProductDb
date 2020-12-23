using ProductDb.Mapping.BiggBrandDbModels.ProductDocksMapping;
using System.Collections.Generic;

namespace ProductDb.Services.ProductDockServices
{
    public interface IProductDockService
    {
        ICollection<ProductDockModel> AllProducts(string sku, int skip, int take);
        ProductDockModel GetProductDockBySku(string sku);
        ProductDockModel GetProductDockbyId(int Id);
        ICollection<ProductDockModel> AllProducts(int skip, int take);
        ICollection<ProductDockCategoryModel> ProductDockCategoriesBySupplierId(int supplierId = 0);
        ICollection<ProductDockModel> ProductDocksBySupplierAndCategoryId(int supplierId, int? categoryId);
        ProductDockModel ProductDockbyId(int Id);
        ProductDockModel AddNewProductDock(ProductDockModel model);
        ProductDockModel Edit(ProductDockModel productDock);
        ICollection<ProductDockModel> NotVariantedProductDocks(int id);
        void SaveProductVariant(List<ProductDockModel> productDocks, int parentProductDockId);
        void ChangeProductParent(int id, int parentId);
        ICollection<ProductDockModel> ProductDockVariantById(int parentId);
        ICollection<ProductDockModel> ProductDockByIDs(List<int> IDs);
        string FormattedParentSKU(string sku);
        ICollection<ProductDockCategoryModel> ProductDockCategoryByParentId(int parentCategoryId);
        ProductDockCategoryModel AddNewProductDockCategory(ProductDockCategoryModel model);
        ProductDockCategoryModel EditProductDockCategory(ProductDockCategoryModel model);
        ICollection<ProductDockCategoryModel> ProductDockCategories();
        int AddNewProductDockPictures(int productDockId, ICollection<ProductDockPicturesModel> modelList);
        int AddNewProductDockAttributes(int productDockId, ICollection<ProductDockAttributeModel> modelList);
        void SaveAllProductDock(int selecetedCatId, int productDockCategoryId, int supplierId);
        ProductDockCategoryModel ProductDockCategoryById(int Id);
        ICollection<ProductDockAttributeModel> ProductDockAttributesByProductDockId(int id);
    }
}