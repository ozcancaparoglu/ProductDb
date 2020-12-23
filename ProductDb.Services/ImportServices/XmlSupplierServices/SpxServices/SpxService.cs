using ProductDb.Common.Entities;
using ProductDb.Common.Enums;
using ProductDb.Services.CurrencyServices;
using ProductDb.Services.ProductDockServices;
using ProductDb.Services.ProductServices;
using ProductDb.Services.TaxServices;
using System.Collections.Generic;
using System.Linq;

namespace ProductDb.Services.ImportServices.XmlSupplierServices.SpxServices
{
    public class SpxService : IntegrationOperations, ISupplierService<SpxService>
    {
        public SpxService(ICurrencyService currencyService, IProductDockService productDockService, IProductService productService, ITaxService taxService, IParentProductDockService parentProductDockService) 
            : base(currencyService, productDockService, productService, taxService, parentProductDockService)
        {
        }

        public bool InsertXmlToDb(int? supplierId, int? rootCategoryId, string url, string startNode)
        {
            try
            {
                var response = false;

                var collection = ReadIntegrationAsync<SpxModel>(url, Endpoints.Xml, startNode, false);

                foreach (var spx in collection)
                {
                    response = InsertOrUpdate(PrepareModel(spx, supplierId.Value, rootCategoryId.Value));
                }

                return response;
            }
            catch
            {
                return false;
            }
        }

        private SpxModel PrepareModel(SpxModel model, int supplierId, int rootCategoryId)
        {
            #region Product Fields

            model.SupplierId = supplierId;
            model.RootCategoryId = rootCategoryId;
            
            model.Sku = "SP" + model.Gtin.FirstOrDefault();
            if (model.Sku.Length > 16)
                model.Sku = model.Sku.Substring(0, 15);

            model.Gtip = model.Gtin.FirstOrDefault();
            model.ShortDescription += $" {model.Size.FirstOrDefault()}";
            model.CategoryName = FirstCharacterUpper("SPX" + " - " + model.CategoryName);

            #endregion

            #region Prices, VatRate, Desi & Stock

            var buyingPriceArr = model.BuyingPriceString.Split(' ');
            var sellingPriceArr = model.SellingPriceString.Split(' ');

            model.Cost = decimal.Parse(buyingPriceArr[0]) * 0.8M / ArrangeKDVRatio(decimal.Parse(model.VatRate));
            model.PsfPrice = decimal.Parse(sellingPriceArr[0]);
            model.Desi = 0;

            model.Currency = buyingPriceArr[1];
            model.Stock = model.Stok.FirstOrDefault() == null ? 0 : int.Parse(model.Stok.FirstOrDefault());

            #endregion

            #region Images & Attributes & Variants

            model.Variants = new List<Variants>();

            model.DownloadImageUrls = new List<string> { model.Image };

            for (int i = 1; i < model.Size.Count; i++)
            {
                var attributesAndValues = new List<AttributesAndValues>
                {
                    new AttributesAndValues
                    {
                        AttributeName = "Size",
                        AttributeValue = model.Size[i]
                    }
                };

                var variantSku = $"SP{model.Gtin[i]}";

                if (variantSku.Length > 16)
                    variantSku = variantSku.Substring(0, 15);

                model.Variants.Add(new Variants
                {
                    Sku = variantSku,
                    Barcode = model.Gtin[i],
                    Stock = model.Stok[i] == null ? 0 : int.Parse(model.Stok[i]),
                    Cost = model.Cost,
                    PsfPrice = model.PsfPrice,
                    ShortDescription = string.Join(" ", attributesAndValues.Select(x => x.AttributeValue)),
                    AttributesAndValues = attributesAndValues
                });

            }

            #endregion

            return model;
        }
    }
}
