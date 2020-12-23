using AutoMapper;
using ProductDb.Data.BiggBrandsDb;
using ProductDb.Data.BiggBrandsDb.ProductDocks;
using ProductDb.Mapping.BiggBrandDbModels;
using ProductDb.Mapping.BiggBrandDbModels.ProductDocksMapping;

namespace ProductDb.Mapping.MapperServiceConfiguration
{
    public class MapperProfile : Profile
    {
        private readonly int depth = 5;

        public MapperProfile()
        {
            CreateMap<Attributes, AttributesModel>().MaxDepth(depth).ReverseMap();
            CreateMap<AttributesValue, AttributesValueModel>().MaxDepth(depth).ReverseMap();

            CreateMap<Brand, BrandModel>().MaxDepth(depth).ReverseMap();
            CreateMap<Category, CategoryModel>().MaxDepth(depth).ReverseMap();
            CreateMap<Currency, CurrencyModel>().MaxDepth(depth).ReverseMap();
            CreateMap<CategoryAttributeMapping, CategoryAttributeMappingModel>().MaxDepth(depth).ReverseMap();

            CreateMap<Language, LanguageModel>().MaxDepth(depth).ReverseMap();
            CreateMap<LanguageValues, LanguageValuesModel>().MaxDepth(depth).ReverseMap();

            CreateMap<ParentProduct, ParentProductModel>().MaxDepth(depth).ReverseMap();
            CreateMap<Product, ProductModel>().MaxDepth(depth).ReverseMap();
            CreateMap<ProductAttributeMapping, ProductAttributeMappingModel>().MaxDepth(depth).ReverseMap();
            CreateMap<ProductAttributeValue, ProductAttributeValueModel>().MaxDepth(depth).ReverseMap();
            CreateMap<Pictures, PicturesModel>().MaxDepth(depth).ReverseMap();

            CreateMap<Supplier, SupplierModel>().MaxDepth(depth).ReverseMap();

            CreateMap<User, UserModel>().MaxDepth(depth).ReverseMap();
            CreateMap<UserRole, UserRoleModel>().MaxDepth(depth).ReverseMap();

            CreateMap<WarehouseQuery, WarehouseQueryModel>().MaxDepth(depth).ReverseMap();
            CreateMap<WarehouseProductStockModel, WarehouseProductStock>().MaxDepth(depth).ReverseMap();
            CreateMap<WarehouseType, WarehouseTypeModel>().MaxDepth(depth).ReverseMap();

            CreateMap<Store, StoreModel>().MaxDepth(depth).ReverseMap();
            CreateMap<StoreWarehouseMapping, StoreWarehouseMappingModel>().MaxDepth(depth).ReverseMap();
            CreateMap<StoreProductMapping, StoreProductMappingModel>().MaxDepth(depth).ReverseMap();
            CreateMap<StoreCategoryMapping, StoreCategoryMappingModel>().MaxDepth(depth).ReverseMap();
            CreateMap<StoreType, StoreTypeModel>().MaxDepth(depth).ReverseMap();

            CreateMap<Permission, PermissionModel>().MaxDepth(depth).ReverseMap();
            CreateMap<RolePermission, RolePermissionModel>().MaxDepth(depth).ReverseMap();

            CreateMap<ProductDock, ProductDockModel>().MaxDepth(depth).ReverseMap();
            CreateMap<ParentProductDock, ParentProductDockModel>().MaxDepth(depth).ReverseMap();

            CreateMap<ProductDockPictures, ProductDockPicturesModel>().MaxDepth(depth).ReverseMap();
            CreateMap<ProductDockCategory, ProductDockCategoryModel>().MaxDepth(depth).ReverseMap();

            CreateMap<VatRate, VatRateModel>().MaxDepth(depth).ReverseMap();
            CreateMap<ProductDockAttribute, ProductDockAttributeModel>().MaxDepth(depth).ReverseMap();

            CreateMap<ProductGroup, ProductGroupModel>().MaxDepth(depth).ReverseMap();
            CreateMap<ProductVariant, ProductVariantModel>().MaxDepth(depth).ReverseMap();

            

            CreateMap<ErpCompany, ErpCompanyModel>().MaxDepth(depth).ReverseMap();

            CreateMap<ProductErpCompanyMapping, ProductErpCompanyMappingModel>().MaxDepth(depth).ReverseMap();

            CreateMap<FormulaGroup, FormulaGroupModel>().MaxDepth(depth).ReverseMap();
            CreateMap<Formula, FormulaModel>().MaxDepth(depth).ReverseMap();
            CreateMap<Margin, MarginModel>().MaxDepth(depth).ReverseMap();
            CreateMap<MarginType, MarginTypeModel>().MaxDepth(depth).ReverseMap();

            CreateMap<Cargo, CargoModel>().MaxDepth(depth).ReverseMap();
            CreateMap<CargoType, CargoTypeModel>().MaxDepth(depth).ReverseMap();
            CreateMap<Transportation, TransportationModel>().MaxDepth(depth).ReverseMap();
            CreateMap<TransportationType, TransportationTypeModel>().MaxDepth(depth).ReverseMap();
            
        }
    }
}
