using System.Collections.Generic;

namespace ProductDb.Mapping.BiggBrandDbModelFields
{
    public static class GetFieldValues
    {
        //CATEGORY
        public static readonly List<string> CategoryFields = new List<string> { "Name" };
        public static readonly string CategoryTable = "Category";

        //ATTRIBUTE VALUES
        public static readonly List<string> AttributesValueFields = new List<string> { "Value" };
        public static readonly string AttributesValueTable = "AttributesValue";

        //PARENT PRODUCT
        public static readonly List<string> ParentProductFields = new List<string> { "Title", "Description", "ShortDescription" };
        public static readonly string ParentProductTable = "ParentProduct";

        //PRODUCT
        public static readonly List<string> ProductFields = new List<string> { "Name", "Title", "Description", "ShortDescription", "Alias", "MetaKeywords", "MetaTitle", "MetaDescription", "SearchEngineTerms" };
        public static readonly string ProductTable = "Product";

        public static readonly List<string> ProductAttributeMappingFields = new List<string>();
        public static readonly string ProductAttributeMappingTable = "ProductAttributeMapping";

        //BRAND
        public static readonly List<string> BrandFields = new List<string> { "Name" };
        public static readonly string BrandTable = "Brand";

        //PRODUCT DOCK
        public static readonly List<string> ProductDockFields = new List<string> { "Name", "FullDescription", "ShortDescription", "MetaKeywords", "MetaTitle", "MetaDescription", "SearchEngineTerms" };
        public static readonly List<string> ProductDockBulletFields = new List<string> { "BulletPoint1", "BulletPoint2", "BulletPoint3", "BulletPoint4", "BulletPoint5"};
        public static readonly string ProductDockTable = "Product";
        public static readonly string ProductDockAttributeTable = "ProductAttributeMapping";


    }
}
