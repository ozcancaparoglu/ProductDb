using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductDb.ExportApi.Models.CaModels
{
    public class CaParentProductModel
    {

        public int ProfileID { get; set; }
        public string ParentProductID { get; set; }
        public string Sku { get; set; }
        public string Title { get; set; }
        public string Brand { get; set; }
        public string SupplierName { get; set; }
        public string Manufacturer { get; set; }
        //public int ParentProductID { get; set; }
        public string Description { get; set; }
        public string ShortDescription { get; set; }
        public string EAN { get; set; }
        public decimal Height { get; set; }
        public decimal Length { get; set; }
        public decimal Width { get; set; }
        public decimal Weight { get; set; }
        //kaçıncı katman olduğunu bilmem lazım producttan çekince TL olarak geliyor
        public decimal Cost { get; set; }
        public decimal RetailPrice { get; set; }
        public decimal BuyItNowPrice { get; set; }
        public string Classification { get; set; }
        public string Condition { get; set; } = "New";
        public string Warranty { get; set; } = "2 Year";
        public bool IsParent { get; set; } = false;
        public bool IsInRelationship { get; set; } = true;
        public string RelationshipName { get; set; }
        public List<Attribute> Attributes { get; set; }
    }

   
}
