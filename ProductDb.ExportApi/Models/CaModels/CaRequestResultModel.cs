using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductDb.ExportApi.Models.CaModels
{
    public class CaRequestResultModel
    {
        public string odatacontext { get; set; }
        public int ID { get; set; }
        public int ProfileID { get; set; }
        public DateTime CreateDateUtc { get; set; }
        public DateTime UpdateDateUtc { get; set; }
        public DateTime QuantityUpdateDateUtc { get; set; }
        public bool IsAvailableInStore { get; set; }
        public bool IsBlocked { get; set; }
        public bool IsExternalQuantityBlocked { get; set; }
        public object BlockComment { get; set; }
        public object BlockedDateUtc { get; set; }
        public object ReceivedDateUtc { get; set; }
        public object LastSaleDateUtc { get; set; }
        public object ASIN { get; set; }
        public string Brand { get; set; }
        public string Condition { get; set; }
        public string Description { get; set; }
        public string EAN { get; set; }
        public object FlagDescription { get; set; }
        public string Flag { get; set; }
        public object HarmonizedCode { get; set; }
        public object ISBN { get; set; }
        public string Manufacturer { get; set; }
        public object MPN { get; set; }
        public string ShortDescription { get; set; }
        public string Sku { get; set; }
        public string Subtitle { get; set; }
        public object TaxProductCode { get; set; }
        public string Title { get; set; }
        public object UPC { get; set; }
        public object WarehouseLocation { get; set; }
        public string Warranty { get; set; }
        public object MultipackQuantity { get; set; }
        public object Height { get; set; }
        public object Length { get; set; }
        public object Width { get; set; }
        public double Weight { get; set; }
        public object Cost { get; set; }
        public object Margin { get; set; }
        public object RetailPrice { get; set; }
        public object StartingPrice { get; set; }
        public object ReservePrice { get; set; }
        public object BuyItNowPrice { get; set; }
        public object StorePrice { get; set; }
        public object SecondChancePrice { get; set; }
        public object MinPrice { get; set; }
        public object MaxPrice { get; set; }
        public object EstimatedShippingCost { get; set; }
        public string SupplierName { get; set; }
        public object SupplierCode { get; set; }
        public object SupplierPO { get; set; }
        public string Classification { get; set; }
        public object IsDisplayInStore { get; set; }
        public object StoreTitle { get; set; }
        public object StoreDescription { get; set; }
        public string BundleType { get; set; }
        public int TotalAvailableQuantity { get; set; }
        public int OpenAllocatedQuantity { get; set; }
        public int OpenAllocatedQuantityPooled { get; set; }
        public int PendingCheckoutQuantity { get; set; }
        public int PendingCheckoutQuantityPooled { get; set; }
        public int PendingPaymentQuantity { get; set; }
        public int PendingPaymentQuantityPooled { get; set; }
        public int PendingShipmentQuantity { get; set; }
        public int PendingShipmentQuantityPooled { get; set; }
        public int TotalQuantity { get; set; }
        public int TotalQuantityPooled { get; set; }
        public bool IsParent { get; set; }
        public bool IsInRelationship { get; set; }
        public object CopyToChildren { get; set; }
        public object ParentProductID { get; set; }
        public object RelationshipName { get; set; }
        public object USP { get; set; }
        public object CategoryPath { get; set; }
        public object CategoryCode { get; set; }
    }
}
