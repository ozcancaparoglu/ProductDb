using Newtonsoft.Json;
using ProductDb.Common.Entities;
using ProductDb.Common.Enums;
using ProductDb.Common.Helpers;
using ProductDb.Mapping.BiggBrandDbModels.ProductDocksMapping;
using ProductDb.Services.CurrencyServices;
using ProductDb.Services.ProductDockServices;
using ProductDb.Services.ProductServices;
using ProductDb.Services.TaxServices;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace ProductDb.Services.ImportServices
{
    public abstract class IntegrationOperations
    {
        private readonly ICurrencyService currencyService;
        private readonly IProductDockService productDockService;
        private readonly IProductService productService;
        private readonly ITaxService taxService;
        private readonly IParentProductDockService parentProductDockService;


        public IntegrationOperations(ICurrencyService currencyService, IProductDockService productDockService, IProductService productService, ITaxService taxService, IParentProductDockService parentProductDockService)
        {
            this.currencyService = currencyService;
            this.productDockService = productDockService;
            this.productService = productService;
            this.taxService = taxService;
            this.parentProductDockService = parentProductDockService;
        }

        #region Xml Operations

        private readonly XmlDocument xmlDoc = new XmlDocument();

        private XmlDocument ReadXmlFile(string xmlUrl)
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(xmlUrl);
            req.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            var res = (HttpWebResponse)req.GetResponse();
            Stream stream = res.GetResponseStream();
            StreamReader prodReader = new StreamReader(stream);

            xmlDoc.Load(prodReader);
            return xmlDoc;
        }

        private T ConvertNode<T>(XmlNode node, bool replaceTurkishChars) where T : IntegrationBase
        {
            XmlSerializer ser = new XmlSerializer(typeof(T));

            string xml;

            if (replaceTurkishChars)
                xml = Convertions.ReplaceTurkishCharacters(node.OuterXml);
            else
                xml = node.OuterXml;

            using (StringReader sr = new StringReader(xml))
            {
                return (T)ser.Deserialize(sr);
            }
        }

        private List<T> XmlConvertToObjectAsync<T>(string url, string startFromNode, bool replaceTurkishCharacters = true) where T : IntegrationBase
        {
            var xmlDoc = ReadXmlFile(url);

            XmlNodeList nodeList = xmlDoc.SelectNodes(startFromNode);

            var collection = new List<T>();

            foreach (XmlNode node in nodeList)
            {
                collection.Add(ConvertNode<T>(node, replaceTurkishCharacters));
            }

            return collection;
        }

        #endregion

        #region Json Operations

        private async Task<List<T>> ReadJsonAsync<T>(string jsonUrl) where T : IntegrationBase
        {
            using (var httpClient = new HttpClient())
            {
                var json = await httpClient.GetStringAsync(jsonUrl);
                return JsonConvert.DeserializeObject<List<T>>(json);
            }
        }

        #endregion

        #region Common Operations

        private int ArrangeCategory(string categoryName, int rootCategoryId)
        {
            var productDockCategories = productDockService.ProductDockCategoryByParentId(rootCategoryId);

            if (productDockCategories != null && productDockCategories.Count > 0 && productDockCategories.Any(x => x.Name == categoryName))
                return productDockCategories.FirstOrDefault(x => x.Name == categoryName).Id;

            try
            {
                var productDockCategory = productDockService.AddNewProductDockCategory(new ProductDockCategoryModel
                {
                    Name = categoryName,
                    ParentCategoryId = rootCategoryId
                });

                return productDockCategory.Id;
            }
            catch { return -1; }
        }

        private List<ProductDockPicturesModel> ArrangePictures(int productDockId, List<string> downloadUrls)
        {
            var productDockPictures = new List<ProductDockPicturesModel>();

            foreach (var url in downloadUrls)
            {
                productDockPictures.Add(new ProductDockPicturesModel
                {
                    ProductDockId = productDockId,
                    DownloadUrl = url
                });
            }

            return productDockPictures;
        }

        private List<ProductDockAttributeModel> ArrangeAttributes(int productDockId, List<AttributesAndValues> attributesAndValues)
        {
            var productDockAttributes = new List<ProductDockAttributeModel>();

            foreach (var attributes in attributesAndValues)
            {
                productDockAttributes.Add(new ProductDockAttributeModel
                {
                    ProductDockId = productDockId,
                    Name = attributes.AttributeName,
                    Value = attributes.AttributeValue
                });
            }

            return productDockAttributes;
        }

        private int CheckCurrency(string currencyUnit)
        {

            switch (currencyUnit.ToUpperInvariant())
            {
                case "YTL":
                case "TL":
                case "TRL":
                    currencyUnit = "TR";
                    break;

                case "EURO":
                    currencyUnit = "EUR";
                    break;

                default:
                    currencyUnit = currencyUnit.ToUpperInvariant();
                    break;
            }

            var currencies = currencyService.AllActiveCurrencies();

            var currency = currencies.FirstOrDefault(x => x.Abbrevation == currencyUnit.ToUpperInvariant());

            return currency == null ? 1 : currency.Id;

        }

        private int CheckVatRate(string vatRate)
        {
            var tax = taxService.AllTaxRate().FirstOrDefault(x => x.Name.Contains(vatRate));
            return tax == null ? 4 : tax.Id;
        }

        private ProductDockModel PrepareDockModel<T>(ProductDockModel productDock, T integrationModel) where T : IntegrationBase
        {
            return new ProductDockModel
            {
                Sku = integrationModel.Sku,
                SupplierId =  integrationModel.SupplierId,
                Name = productDock == null ? integrationModel.Name : productDock.Name,
                Model = productDock == null ? integrationModel.Model : productDock.Model,
                ShortDescription = productDock == null || string.IsNullOrWhiteSpace(productDock.ShortDescription) ? integrationModel.ShortDescription : productDock.ShortDescription,
                FullDescription = productDock == null || string.IsNullOrWhiteSpace(productDock.FullDescription) ? integrationModel.FullDescription : productDock.FullDescription,
                Gtin = productDock == null ? integrationModel.Gtip : productDock.Gtin,
                Brand = integrationModel.Brand,
                ProductDockCategoryId = productDock == null ? ArrangeCategory(integrationModel.CategoryName, integrationModel.RootCategoryId) : productDock.ProductDockCategoryId,
                ParentProductDockId = productDock == null ? integrationModel.ParentProductDockId : productDock.ParentProductDockId,
                Cost = integrationModel.Cost ?? 0,
                PsfPrice = integrationModel.PsfPrice ?? 0,
                CurrencyId = CheckCurrency(integrationModel.Currency),
                Desi = integrationModel.Desi ?? productDock.Desi,
                Stock = integrationModel.Stock,
                Height = integrationModel.Height ?? 0,
                Width = integrationModel.Width ?? 0,
                Length = integrationModel.Length ?? 0,
                Weight = integrationModel.Weight ?? 0,
                VatRateId = CheckVatRate(integrationModel.VatRate),
                MetaDescription = productDock == null || string.IsNullOrWhiteSpace(productDock.MetaDescription) ? integrationModel.MetaDescription : productDock.MetaDescription,
                MetaTitle = productDock == null || string.IsNullOrWhiteSpace(productDock.MetaTitle) ? integrationModel.MetaTitle : productDock.MetaTitle,
                MetaKeywords = productDock == null || string.IsNullOrWhiteSpace(productDock.MetaKeywords) ? integrationModel.MetaKeywords : productDock.MetaKeywords,

                BulletPoint1 = productDock == null || string.IsNullOrWhiteSpace(productDock.BulletPoint1) ? "-" : productDock.BulletPoint1,
                BulletPoint2 = productDock == null || string.IsNullOrWhiteSpace(productDock.BulletPoint2) ? "-" : productDock.BulletPoint2,
                BulletPoint3 = productDock == null || string.IsNullOrWhiteSpace(productDock.BulletPoint3) ? "-" : productDock.BulletPoint3,
                BulletPoint4 = productDock == null || string.IsNullOrWhiteSpace(productDock.BulletPoint4) ? "-" : productDock.BulletPoint4,
                BulletPoint5 = productDock == null || string.IsNullOrWhiteSpace(productDock.BulletPoint5) ? "-" : productDock.BulletPoint5
            };
        }

        /// <summary>
        /// Arrange Kdv ratio as multiplier. Example (for %8 it returns 1.08) 
        /// </summary>
        /// <param name="kdv"></param>
        /// <returns></returns>
        public decimal ArrangeKDVRatio(decimal kdv)
        {
            return (kdv / 100) + 1;
        }

        /// <summary>
        /// Makes First Character to Uppercase
        /// </summary>
        /// <param name="str">Value that will processed</param>
        /// <returns></returns>
        public string FirstCharacterUpper(string str)
        {
            CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
            TextInfo textInfo = cultureInfo.TextInfo;
            return textInfo.ToTitleCase(str);
        }

        public List<T> ReadIntegrationAsync<T>(string url, Endpoints endpoint, string startNode = "", bool replaceTurkishCharacters = true) where T : IntegrationBase
        {
            if (endpoint == Endpoints.Xml && !string.IsNullOrWhiteSpace(startNode))
                return XmlConvertToObjectAsync<T>(url, startNode, replaceTurkishCharacters);
            else if (endpoint == Endpoints.Json)
                return ReadJsonAsync<T>(url).Result;

            return null;
        }

        //TODO : Refactor et bokum gibi oldu.
        public bool InsertOrUpdate<T>(T integrationModel) where T : IntegrationBase
        {
            try
            {
                var product = productService.ProductBySku(integrationModel.Sku);

                if (product != null)
                {
                    product.BuyingPrice = integrationModel.Cost;
                    product.PsfPrice = integrationModel.PsfPrice;
                    product.VatRateId = CheckVatRate(integrationModel.VatRate);
                    product.CurrencyId = CheckCurrency(integrationModel.Currency);

                    productService.EditProduct(product);
                }
                else
                {
                    var productDock = productDockService.GetProductDockBySku(integrationModel.Sku);
                    var productDockModel = PrepareDockModel(productDock, integrationModel);

                    if (productDock == null)
                    {   
                        var productDockSavedEntity = productDockService.AddNewProductDock(productDockModel);

                        if (integrationModel.DownloadImageUrls != null && integrationModel.DownloadImageUrls.Count > 0)
                            productDockService.AddNewProductDockPictures(productDockSavedEntity.Id, ArrangePictures(productDockSavedEntity.Id, integrationModel.DownloadImageUrls.ToList()));

                        if (integrationModel.AttributesAndValues != null && integrationModel.AttributesAndValues.Count > 0)
                            productDockService.AddNewProductDockAttributes(productDockSavedEntity.Id, ArrangeAttributes(productDockSavedEntity.Id, integrationModel.AttributesAndValues.ToList()));

                        if (integrationModel.Variants != null && integrationModel.Variants.Count > 0)
                        {
                            var parentProductDock = parentProductDockService.AddNewParentProductDock(new ParentProductDockModel
                            {
                                Sku = $"{productDockSavedEntity.Sku}_PP",
                                Title = $"{productDockSavedEntity.Name}",
                                ProductDockCategoryId = productDockSavedEntity.ProductDockCategoryId,
                                SupplierId = productDockSavedEntity.SupplierId
                            });

                            productDockSavedEntity.ParentProductDockId = parentProductDock.Id;

                            productDockService.Edit(productDockSavedEntity);

                            foreach (var variant in integrationModel.Variants)
                            {
                                integrationModel.Sku = variant.Sku;
                                integrationModel.Gtip = variant.Barcode;
                                integrationModel.Stock = variant.Stock == null ? 0 : variant.Stock.Value;
                                integrationModel.PsfPrice = variant.PsfPrice;
                                integrationModel.ParentProductDockId = parentProductDock.Id;
                                integrationModel.ShortDescription = variant.ShortDescription;
                                var savedVariant = productDockService.AddNewProductDock(PrepareDockModel(productDock, integrationModel));

                                if (variant.DownloadImageUrls != null && variant.DownloadImageUrls.Count > 0)
                                    productDockService.AddNewProductDockPictures(savedVariant.Id, ArrangePictures(savedVariant.Id, variant.DownloadImageUrls.ToList()));

                                if (variant.AttributesAndValues != null && variant.AttributesAndValues.Count > 0)
                                    productDockService.AddNewProductDockAttributes(savedVariant.Id, ArrangeAttributes(savedVariant.Id, variant.AttributesAndValues.ToList()));
                            }
                        }
                    }
                    else
                    {
                        productDockModel.Id = productDock.Id;
                        productDockService.Edit(productDockModel);

                        if (integrationModel.DownloadImageUrls != null && integrationModel.DownloadImageUrls.Count > 0)
                            productDockService.AddNewProductDockPictures(productDockModel.Id, ArrangePictures(productDockModel.Id, integrationModel.DownloadImageUrls.ToList()));

                        if (integrationModel.AttributesAndValues != null && integrationModel.AttributesAndValues.Count > 0)
                            productDockService.AddNewProductDockAttributes(productDockModel.Id, ArrangeAttributes(productDockModel.Id, integrationModel.AttributesAndValues.ToList()));

                        foreach (var variant in integrationModel.Variants)
                        {
                            var variantModel = productDockService.GetProductDockBySku(variant.Sku);
                            variantModel.Stock = variant.Stock == null ? 0 : variant.Stock.Value;
                            variantModel.PsfPrice = variant.PsfPrice;

                            productDockService.Edit(variantModel);

                            if (variant.DownloadImageUrls != null && variant.DownloadImageUrls.Count > 0)
                                productDockService.AddNewProductDockPictures(variantModel.Id, ArrangePictures(variantModel.Id, variant.DownloadImageUrls.ToList()));

                            if (variant.AttributesAndValues != null && variant.AttributesAndValues.Count > 0)
                                productDockService.AddNewProductDockAttributes(variantModel.Id, ArrangeAttributes(variantModel.Id, variant.AttributesAndValues.ToList()));
                        }

                    }


                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        #endregion
    }
}
