using ProductDb.Common.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ProductDb.Data.BiggBrandsDb
{
    public class StoreCategoryMapping: EntityBase
    {

        public int StoreId { get; set; }
        [ForeignKey("StoreId")]
        public Store Store { get; set; }
        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public Category Category { get; set; }
        public int  ErpCategoryId { get; set; }
    }
}
