using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using ApiClient.HttpClient;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductDb.Common.GlobalEntity;
using ProductDb.Services.ProductServices;

namespace ProductDb.Api.Export.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DataExportController : ControllerBase
    {
        private readonly IProductService productService;
        private readonly IApiRepo apiRepo;
        public DataExportController(IProductService productService, IApiRepo apiRepo)
        {
            this.productService = productService;
            this.apiRepo = apiRepo;
        }
        [HttpGet]
        [Route("get-xml-data/{store_id}/{language_id}/{AllowedUserVariables}")]
        public IActionResult GetXmlData(int store_id, int language_id, string AllowedUserVariables)
        {
            if (AllowedUserVariables != "b6732ad4-2d13-40e1-a4eb-c843a5fe28cd")
                return BadRequest();
            List<EntegraXmlModel> entegraXmlModels = productService.EntegraProducts(store_id, language_id);

            var xmlText = ExportEntegraToXml(entegraXmlModels);
            return Content(xmlText, "text/xml");
        }

        [HttpGet]
        [Route("get-xml-data-deneme/{store_id}/{language_id}/{AllowedUserVariables}")]
        public IActionResult GetXmlDataDeneme(int store_id, int language_id, string AllowedUserVariables)
        {
            if (AllowedUserVariables != "b6732ad4-2d13-40e1-a4eb-c843a5fe28cd")
                return BadRequest();
            ProductVM productVM = new ProductVM();
            List<EntegraXmlModel> entegraXmlModels = new List<EntegraXmlModel>();
            decimal value = 0;

            var productJsonModels = apiRepo.GetList<ProductJsonModel>($"integration/ProductDetailByStoreWithLanguage/{store_id}/{language_id}", "", Common.Enums.Endpoints.RegisApi, Common.Enums.MediaType.Json).Result.Where(x => x.ProductVariants == null);
            var productPriceandStockModel = apiRepo.GetList<ProductStockAndPriceJsonModel>($"integration/ProductStockAndPriceByStoreId/{store_id}", "", Common.Enums.Endpoints.RegisApi, Common.Enums.MediaType.Json).Result;
            foreach (var item in productJsonModels)
            {
                var productPriceandStock = productPriceandStockModel.FirstOrDefault(x => x.Sku == item.Sku);
                try
                {
                    value = item.Attributes.FirstOrDefault(y => y.FieldName == "Shipping Weight") == null ? 0 : Convert.ToDecimal(item.Attributes.FirstOrDefault(y => y.FieldName == "Shipping Weight").Value.Replace(".", ","));
                }
                catch (Exception) { value = 0; }

                entegraXmlModels.Add(new EntegraXmlModel()
                {
                    SKU = item.Sku,
                    Name = item.Name,
                    Brand = item.Brand,
                    ProductCategory = item.Category.Split(">>").Last(),
                    Supplier = item.Supplier,
                    FullDescription = item.Description,
                    ShortDescription = item.ShortDescription,
                    Price = productPriceandStock.Price,
                    ProductPictures = item.Pictures.OrderBy(y => y.Order).Select(y => y.CdnPath).ToList(),
                    StockQuantity = productPriceandStock.Stock,
                    Desi = value
                });
            }

            var xmlText = ExportEntegraToXml(entegraXmlModels);
            return Content(xmlText, "text/xml");
        }

        [HttpGet]
        [Route("get-xml-data-variation/{store_id}/{language_id}/{AllowedUserVariables}")]
        public IActionResult GetXmlDataVariation(int store_id, int language_id, string AllowedUserVariables)
        {
            if (AllowedUserVariables != "b6732ad4-2d13-40e1-a4eb-c843a5fe28cd")
                return BadRequest();
            ProductVM productVM = new ProductVM();
            List<EntegraXmlVariantModel> entegraXmlModels = new List<EntegraXmlVariantModel>();
            decimal value = 0;

            var productJsonModels = apiRepo.GetList<ProductJsonModel>($"integration/ProductDetailByStoreWithLanguage/{store_id}/{language_id}", "", Common.Enums.Endpoints.RegisApi, Common.Enums.MediaType.Json).Result.Where(x => x.ProductVariants != null);
            var productPriceandStockModel = apiRepo.GetList<ProductStockAndPriceJsonModel>($"integration/ProductStockAndPriceByStoreId/{store_id}", "", Common.Enums.Endpoints.RegisApi, Common.Enums.MediaType.Json).Result;
            foreach (var item in productJsonModels)
            {
                var productPriceandStock = productPriceandStockModel.FirstOrDefault(x => x.Sku == item.Sku);
                try
                {
                    value = item.Attributes.FirstOrDefault(y => y.FieldName == "Shipping Weight") == null ? 0 : Convert.ToDecimal(item.Attributes.FirstOrDefault(y => y.FieldName == "Shipping Weight").Value.Replace(".", ","));
                }
                catch (Exception) { value = 0; }

                EntegraXmlVariantModel entegraXmlVariantModel = new EntegraXmlVariantModel();

                entegraXmlVariantModel = new EntegraXmlVariantModel()
                {
                    Sku = item.Sku,
                    Name = item.Name,
                    Brand = item.Brand,
                    Category = item.Category.Split(">>").Last(),
                    Description = item.Description,
                    ShortDescription = item.ShortDescription,
                    Price = productPriceandStock.Price,
                    ProductPictures = item.Pictures.OrderBy(y => y.Order).Select(y => y.CdnPath).ToList(),
                    Quantity = productPriceandStock.Stock.ToString(),
                    Desi = value,
                    Barcode = item.Barcode,
                    CurrencyType = item.Currency
                };

                List<Variant> variants = new List<Variant>();
                foreach (var variant in item.ProductVariants.Skip(1))
                {
                    var productPriceandStockVariant = productPriceandStockModel.FirstOrDefault(x => x.Sku == variant.Sku);
                    List<Spec> ProductVariantAttribute = new List<Spec>();
                    for (int i = 0; i < item.ProductVariantAttr.Length; i++)
                    {
                        ProductVariantAttribute.Add(new Spec()
                        {
                            Name = item.ProductVariantAttr[i],
                            Text = item.Attributes.FirstOrDefault(x => x.FieldName == item.ProductVariantAttr[i]).Value
                        });
                    }

                    variants.Add(new Variant()
                    {
                        Attribute = ProductVariantAttribute,
                        Barcode = variant.Barcode,
                        Price = productPriceandStockVariant.Price,
                        Quantity = productPriceandStockVariant.Stock.ToString(),
                        Sku = variant.Sku
                    });
                }

                entegraXmlVariantModel.Variants = new Variants()
                { Variant = variants };

                entegraXmlModels.Add(entegraXmlVariantModel);
            }

            var xmlText = ExportEntegraToXmlVariant(entegraXmlModels);
            return Content(xmlText, "text/xml");
        }

        [HttpGet]
        [Route("get-xml-data-with-attribute/{store_id}/{language_id}/{AllowedUserVariables}")]
        public IActionResult GetXmlDataWithAttribute(int store_id, int language_id, string AllowedUserVariables)
        {
            if (AllowedUserVariables != "00c4a02c-2722-4235-9c40-2910dd5c6c5f")
                return BadRequest();

            ProductVM productVM = new ProductVM();

            productVM.products = apiRepo.GetList<ProductJsonModel>($"integration/ProductDetailByStoreWithLanguage/{store_id}/{language_id}", "", Common.Enums.Endpoints.RegisApi, Common.Enums.MediaType.Json).Result;
            //List<ProductWithAttributeXMLModel> XmlModels = productService.productWithAttributeXMLs(store_id, language_id);
            productVM.priceandStock = apiRepo.GetList<ProductStockAndPriceJsonModel>($"integration/ProductStockAndPriceByStoreId/{store_id}", "", Common.Enums.Endpoints.RegisApi, Common.Enums.MediaType.Json).Result;
            if (store_id == 6)
            {
                productVM.products.AddRange(apiRepo.GetList<ProductJsonModel>($"integration/ProductDetailByStoreWithLanguage/37/{language_id}", "", Common.Enums.Endpoints.RegisApi, Common.Enums.MediaType.Json).Result);
                productVM.priceandStock.AddRange(apiRepo.GetList<ProductStockAndPriceJsonModel>($"integration/ProductStockAndPriceByStoreId/37", "", Common.Enums.Endpoints.RegisApi, Common.Enums.MediaType.Json).Result);
            }
            var xmlText = XmlCreator(productVM);
            return Content(xmlText, "text/xml");
        }

        [HttpGet]
        [Route("get-excel-data/{language_id}/{AllowedUserVariables}")]
        public IActionResult GetExcelData(int language_id, string AllowedUserVariables)
        {
            if (AllowedUserVariables == "b6732ad4-2d13-40e1-a4eb-c843a5fe28cd")
                return BadRequest();
            return Ok();
        }

        [HttpGet]
        [Route("get-json-data/{language_id}/{AllowedUserVariables}")]
        public IActionResult GetJsonData(int language_id, string AllowedUserVariables)
        {
            if (AllowedUserVariables == "b6732ad4-2d13-40e1-a4eb-c843a5fe28cd")
                return BadRequest();
            return Ok();
        }

        public string ExportEntegraToXml(List<EntegraXmlModel> products)
        {
            EntegraXMLSerializer xmlserializer = new EntegraXMLSerializer();
            xmlserializer.XmlProducts = products;
            using (var tw = new StringWriterWithEncoding(Encoding.UTF8))
            {
                var xmlS = new XmlSerializer(typeof(EntegraXMLSerializer));
                xmlS.Serialize(tw, xmlserializer);
                var serialized = tw.ToString();
                return serialized;
            }
        }

        public string ExportEntegraToXmlVariant(List<EntegraXmlVariantModel> products)
        {
            EntegraXMLVariantSerializer xmlserializer = new EntegraXMLVariantSerializer();
            xmlserializer.XmlProducts = products;
            using (var tw = new StringWriterWithEncoding(Encoding.UTF8))
            {
                var xmlS = new XmlSerializer(typeof(EntegraXMLVariantSerializer));
                xmlS.Serialize(tw, xmlserializer);
                var serialized = tw.ToString();
                return serialized;
            }
        }

        public string ExportToXml(List<ProductWithAttributeXMLModel> XMLModels)
        {
            ProductWithAttributeXMLModelSerializer xmlserializer = new ProductWithAttributeXMLModelSerializer();
            xmlserializer.XmlProducts = XMLModels;
            using (var tw = new StringWriterWithEncoding(Encoding.UTF8))
            {
                var xmlS = new XmlSerializer(typeof(ProductWithAttributeXMLModelSerializer));
                xmlS.Serialize(tw, xmlserializer);
                var serialized = tw.ToString();
                return serialized;
            }

        }

        public string XmlCreator(ProductVM productVM)
        {
            XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
            xmlWriterSettings.CheckCharacters = false;
            xmlWriterSettings.Indent = true;
            xmlWriterSettings.NewLineHandling = NewLineHandling.None;
            var sb = new StringWriterWithEncoding(Encoding.UTF8);
            var w = XmlWriter.Create(sb, xmlWriterSettings);
            w.WriteStartDocument();
            w.WriteStartElement("Products");
            foreach (var product in productVM.products)
            {
                int i = 0;
                w.WriteStartElement("Product");
                w.WriteElementString("Name", product.Name);
                w.WriteElementString("Brand", product.Brand);
                w.WriteElementString("Supplier", product.Supplier);
                w.WriteStartElement("FullDescription");
                w.WriteCData(System.Net.WebUtility.HtmlDecode(product.Description));
                w.WriteEndElement();
                //w.WriteElementString("FullDescription", product.FullDescription);
                w.WriteElementString("BarCode", product.Barcode);
                w.WriteElementString("SKU", product.Sku);
                if (productVM.priceandStock.Any(x => x.Sku == product.Sku))
                {
                    w.WriteElementString("Price", productVM.priceandStock.FirstOrDefault(x => x.Sku == product.Sku).Price.ToString());
                    w.WriteElementString("StockQuantity", productVM.priceandStock.FirstOrDefault(x => x.Sku == product.Sku).Stock.ToString());
                }
                else
                    continue;

                w.WriteElementString("ProductCategory", product.Category);
                foreach (var productPicture in product.Pictures)
                {
                    i++;
                    w.WriteElementString("ProductPicture" + i.ToString(), productPicture.CdnPath);
                }
                i = 0;
                foreach (var attribute in product.Attributes.ToList())
                {
                    if (product.Attributes.Where(x => x.FieldName == attribute.FieldName).Count() > 1)
                    {
                        i++;
                        w.WriteElementString(attribute.FieldName.Replace(" ", "") + i.ToString(), attribute.Value);
                    }
                    else
                    {
                        w.WriteElementString(attribute.FieldName.Replace(" ", ""), attribute.Value);
                    }
                }
                w.WriteEndElement();

                if(product.ProductVariants!=null)
                {
                    foreach (var productVariant in product.ProductVariants.Skip(1))
                    {
                        int k = 0;
                        w.WriteStartElement("Product");
                        w.WriteElementString("Name", productVariant.Name);
                        w.WriteElementString("Brand", productVariant.Brand);
                        w.WriteElementString("Supplier", productVariant.Supplier);
                        w.WriteStartElement("FullDescription");
                        w.WriteCData(System.Net.WebUtility.HtmlDecode(productVariant.Description));
                        w.WriteEndElement();
                        //w.WriteElementString("FullDescription", productVariant.FullDescription);
                        w.WriteElementString("BarCode", productVariant.Barcode);
                        w.WriteElementString("SKU", productVariant.Sku);
                        if(productVM.priceandStock.Any(x => x.Sku == productVariant.Sku))
                        {
                            w.WriteElementString("Price", productVM.priceandStock.FirstOrDefault(x => x.Sku == productVariant.Sku).Price.ToString());
                            w.WriteElementString("StockQuantity", productVM.priceandStock.FirstOrDefault(x => x.Sku == productVariant.Sku).Stock.ToString());
                        }
                        else
                            continue;
                        w.WriteElementString("ProductCategory", productVariant.Category);
                        foreach (var productVariantPicture in productVariant.Pictures)
                        {
                            k++;
                            w.WriteElementString("ProductPicture" + k.ToString(), productVariantPicture.CdnPath);
                        }
                        k = 0;
                        foreach (var attribute in productVariant.Attributes.ToList())
                        {
                            if (productVariant.Attributes.Where(x => x.FieldName == attribute.FieldName).Count() > 1)
                            {
                                k++;
                                w.WriteElementString(attribute.FieldName.Replace(" ", "") + k.ToString(), attribute.Value);
                            }
                            else
                            {
                                w.WriteElementString(attribute.FieldName.Replace(" ", ""), attribute.Value);
                            }
                        }
                        w.WriteEndElement();
                    }
                }
            }
            //Products
            w.WriteEndElement();
            w.WriteEndDocument();
            w.Close();
            w.Close();

            //var xml = "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>" + sb.ToString();
            var xml = sb.ToString();
            return xml;
        }

        public sealed class StringWriterWithEncoding : StringWriter
        {
            private readonly Encoding encoding;

            public StringWriterWithEncoding(Encoding encoding)
            {
                this.encoding = encoding;
            }

            public override Encoding Encoding
            {
                get { return encoding; }
            }
        }
    }
}