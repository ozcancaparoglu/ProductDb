using ProductDb.Common.Entities;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProductDb.Mapping.BiggBrandDbModels
{
    public class MarginModel : EntityBaseModel
    {
        public int? StoreId { get; set; }
        public virtual StoreModel Store { get; set; }

        public int? MarginTypeId { get; set; }
        public virtual MarginTypeModel MarginType { get; set; }

        public decimal Profit { get; set; }

        [Required]
        public int EntityId { get; set; }

        public int? SecondEntityId { get; set; }

        // for navigation
        public string Name { get; set; }
        public int BrandId { get; set; }
        public ICollection<BrandModel> Brands { get; set; }
        public int ProductId { get; set; }
        public ProductModel Product { get; set; }
        public int CategoryId { get; set; }
        public ICollection<CategoryModel> Categories { get; set; }
        public string categoryWithParents { get; set; }
        public string SecondName { get; set; }
        public string Sku { get; set; }

    }
}
