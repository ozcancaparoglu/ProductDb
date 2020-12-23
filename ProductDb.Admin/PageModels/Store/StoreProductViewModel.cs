using Microsoft.AspNetCore.Http;
using ProductDb.Mapping.BiggBrandDbModels;
using System.Collections.Generic;

namespace ProductDb.Admin.PageModels.Store
{
    public class StoreProductViewModel
    {
        public int StoreId { get; set; }
        public int TargetStoreId { get; set; }
        public StoreModel Store { get; set; }
        public List<StoreModel> Stores { get; set; }
        public List<StoreProductMappingModel> StoreProducts { get; set; }
        // Variantable Navigation
        public List<AttributesModel> Attributes { get; set; }
        public int AttributeId { get; set; }
        // File Uploading
        public IFormFile file { get; set; }
        public string message { get; set; }
        public bool isUploaded { get; set; }
        public int StoreCopyType { get; set; }
        public string StoreCopyTypeName { get; set; }
    }
}
