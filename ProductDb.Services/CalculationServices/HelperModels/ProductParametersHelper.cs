namespace ProductDb.Services.CalculationServices.HelperModels
{
    public class ProductParametersHelper
    {
        public int ProductId { get; set; }
        public decimal BuyingPrice { get; set; }
        public int BuyingPriceCurrencyId { get; set; }
        public decimal MarketPrice { get; set; }
        public int MarketPriceCurrencyId { get; set; }
        public decimal CorporatePrice { get; set; }
        public int CorporatePriceCurrencyId { get; set; }
        public decimal Desi { get; set; }
        public decimal AbroadDesi { get; set; }
        public int VatNumber { get; set; }
        public decimal Margin { get; set; }
        public int CategoryId { get; set; }
        public int BrandId { get; set; }
        public string SellingPrice { get; set; }
        public string PointPrice { get; set; }
        public int CurrencyId { get; set; }
        public string ShippingWeight { get; set; }
        public int? StoreVatNumber { get; set; }

    }
}
