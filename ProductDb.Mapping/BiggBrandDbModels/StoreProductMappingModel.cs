using ProductDb.Common.Entities;

namespace ProductDb.Mapping.BiggBrandDbModels
{
    public class StoreProductMappingModel : EntityBaseModel
    {

        public int? StoreId { get; set; }

        public virtual StoreModel Store { get; set; }

        public int? ProductId { get; set; }

        public virtual ProductModel Product { get; set; }

        public int Stock { get; set; }

        public int StoreProductId { get; set; }

        public int StoreParentProductId { get; set; }

        public int BaseStoreId { get; set; }

        public string StoreCategory { get; set; }

        public bool IsRealStock { get; set; } = true;

        public bool? IsSend { get; set; }
   
        public decimal? Price { get; set; }

        public decimal? Point { get; set; }

        public bool? IsFixed { get; set; } = false;

        public bool? IsFixedPoint { get; set; } = false;

        public decimal? ErpPrice { get; set; }

        public decimal? ErpPoint { get; set; }

        public string CatalogCode { get; set; }

        public string CatalogName { get; set; }

        public string CategoryWithParents { get; set; }

        public string ParentProductSku { get; set; }

        public int? VatValue { get; set; }

    }
}
