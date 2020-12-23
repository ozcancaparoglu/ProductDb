using System;
using System.Collections.Generic;
using System.Text;

namespace ProductDb.Common.GlobalEntity
{
    public class IntegrationList
    {
        public int ProductId { get; set; }
        public string PictureUrl { get; set; }
        public string Sku { get; set; }
        public string Title { get; set; }
        public int StoreId { get; set; }
        public int StoreProductId { get; set; }
    }
}
