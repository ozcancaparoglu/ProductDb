namespace ProductDb.Common.Cache
{
    public class CacheStatics
    {
        /// <summary>
        ///  Invoice Cache 
        /// </summary>
        public static string InvoiceCacheKey = "invoicelist";
        public static int InvoiceCacheTime = 60;

        /// <summary>
        /// Department Cache
        /// </summary>
        public static string DepartmentCacheKey = "DepartmentList";
        public static int DepartmentCacheTime = 60;
        /// <summary>
        /// Division Cache
        /// </summary>
        public static string DivisionCacheKey = "DivisionList";
        public static int DivisionCacheTime = 60;

        /// <summary>
        /// Project List Cache
        /// </summary>
        public static string ProjectListCacheKey = "ProjectList";
        public static int ProjectListCacheTime = 60;

        /// <summary>
        /// Store Warehouse Mappings
        /// </summary>
        public static string StoreWarehouseMappingKey = "StoreWarehouseMapping";
        public static int StoreWarehouseMappingCacheTime = 120;

        /// <summary>
        /// Store Products Cache
        /// </summary>
        public static string StoreProductsKey = "StoreProducts";
        public static int StoreProductsCacheTime = 120;

        /// <summary>
        /// Warehouse Products Cache
        /// </summary>
        public static string WarehouseProductsKey = "WarehouseProducts";
        public static int WarehouseProductsCacheTime = 120;

        /// <summary>
        ///  AllProductsWithDetail Products Cache
        /// </summary>
        public static string AllProductsWithDetail = "AllProductsWithDetail";
        public static int AllProductsWithDetailCacheTime = 120;

        /// <summary>
        ///  LanguageValuesWithTableName Cache
        /// </summary>
        public static string LanguageValuesWithTableName = "LanguageValuesWithTableName";
        public static int LanguageValuesWithTableNameCacheTime = 120;

        /// <summary>
        ///  Product AttributeMapping Cache
        /// </summary>
        public static string ProductAttributeMappingCache = "ProductAttributeMapping";
        public static int ProductAttributeMappingCacheTime = 120;

        /// <summary>
        ///  Product AttributeValue Cache
        /// </summary>
        public static string ProductAttributeValueCache = "ProductAttributeValue";
        public static int ProductAttributeValueCacheTime = 120;

        /// <summary>
        ///  ProductDetailByStoreWithLanguage Cache
        /// </summary>
        public static string ProductDetailByStoreWithLanguageCache = "ProductDetailByStoreWithLanguage";
        public static int ProductDetailByStoreWithLanguageCacheTime = 1440;


        /// <summary>
        ///  Attribute Value Cache
        /// </summary>
        public static string AttributeValue = "AttributeValue";
        public static int AttributeValueCacheTime = 120;

        /// <summary>
        ///  Attribute Cache
        /// </summary>
        public static string Attribute = "Attribute";
        public static int AttributeCacheTime = 120;

        /// <summary>
        ///  Language Cache
        /// </summary>
        public static string LanguageCache = "LanguageCache";
        public static int LanguageCacheTime = 120;

        /// <summary>
        ///  Logo Product Stock Cache
        /// </summary>
        public static string LogoProductStockCache = "LogoProductStockCache";
        public static int LogoProductStockCacheTime = 30;

        /// <summary>
        /// Store Cache
        /// </summary>
        public static string StoreCache = "StoreCache";
        public static int StoreCacheTime = 2;

        /// <summary>
        /// Role Cache
        /// </summary>
        public static string RolesCache = "PermissionCache";
        public static int RoleCacheTime = 30;

        /// <summary>
        /// Role Cache
        /// </summary>
        public static string ProductVariantCache = "ProductVariant";
        public static int ProductVariantCacheTime = 2;
    }
}
