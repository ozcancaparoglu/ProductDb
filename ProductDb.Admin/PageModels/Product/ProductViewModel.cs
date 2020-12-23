using ProductDb.Mapping.BiggBrandDbModels;
using System.Collections.Generic;

namespace ProductDb.Admin.PageModels.Product
{
    public class ProductViewModel
    {
        public ProductModel Product { get; set; }
        public List<ProductModel> Products { get; set; }
        public ParentProductModel ParentProduct { get; set; }
        public List<ParentProductModel> ParentProductList { get; set; }
        public List<ProductAttributeMappingModel> ProductCategoryAttributes { get; set; }
        public List<ProductAttributeMappingModel> ProductRequiredAttributes { get; set; }
        public List<PicturesModel> ProductPictures { get; set; }
        public List<BrandModel> Brands { get; set; }
        public List<CurrencyModel> Currencies { get; set; }
        public List<VatRateModel> VatRates { get; set; }
        public List<ProductGroupModel> ProductGroups { get; set; }
        public string CategoryName { get; set; }
        // Get CompanyList For ERP insert
        public List<ErpCompanyModel> ErpCompanies { get; set; }
        public List<ProductErpCompanyMappingModel> ErpSelectedCompanies { get; set; }
        public List<SupplierModel> Suppliers { get; set; }

    }
}
