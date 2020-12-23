using ExcelDataReader;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using ProductDb.Common.Helpers;
using ProductDb.Data.BiggBrandsDb;
using ProductDb.Mapping.AutoMapperConfigurations;
using ProductDb.Mapping.BiggBrandDbModels;
using ProductDb.Mapping.BiggBrandDbModels.ImportModels;
using ProductDb.Repositories.BaseRepositories;
using ProductDb.Repositories.UnitOfWorks;
using ProductDb.Services.CurrencyServices;
using ProductDb.Services.LanguageServices;
using ProductDb.Services.ProductServices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ProductDb.Services.ImportServices
{
    public class ExcelService : IExcelService
    {
        private readonly IConfiguration configuration;
        private readonly IUnitOfWork unitOfWork;
        private readonly IHostingEnvironment hostingEnvironment;
        private readonly IAutoMapperConfiguration autoMapperConfiguration;

        private readonly IGenericRepository<Product> productRepo;
        private readonly IGenericRepository<VatRate> vatRateRepo;
        private readonly IGenericRepository<ProductAttributeMapping> productAttributeRepo;
        private readonly IGenericRepository<StoreProductMapping> storeProductMappingRepo;
        private readonly IGenericRepository<LanguageValues> languageValueRepo;

        private readonly ILanguageService languageService;
        private readonly IProductService productService;

        public ExcelService(IProductService productService,
                            IAutoMapperConfiguration autoMapperConfiguration,
                            IUnitOfWork unitOfWork,
                            ILanguageService languageService,
                            IConfiguration configuration,
                            IHostingEnvironment hostingEnvironment)
        {
            this.configuration = configuration;
            this.unitOfWork = unitOfWork;
            this.languageService = languageService;
            this.productService = productService;
            this.autoMapperConfiguration = autoMapperConfiguration;
            this.hostingEnvironment = hostingEnvironment;

            productRepo = unitOfWork.Repository<Product>();
            languageValueRepo = this.unitOfWork.Repository<LanguageValues>();
            productAttributeRepo = this.unitOfWork.Repository<ProductAttributeMapping>();
            storeProductMappingRepo = this.unitOfWork.Repository<StoreProductMapping>();
            vatRateRepo = this.unitOfWork.Repository<VatRate>();
        }


        #region Private Methods

        /// <summary>
        /// Arrange Language Value Table
        /// </summary>
        /// <param name="entityId"></param>
        /// <param name="tableName"></param>
        /// <param name="fieldName"></param>
        /// <param name="languageId"></param>
        /// <param name="value"></param>
        /// <param name="isInsert">true for insert false for update</param>
        /// <returns></returns>
        private LanguageValues ArrangeLanguageBasedList(int entityId, string tableName, string fieldName, int languageId, string value, out bool isInsert)
        {
            isInsert = false;

            var genericLanguageAttribute = languageService.LanguageValuesByEntityIdAndTableName(entityId, tableName).FirstOrDefault(a => a.FieldName == fieldName
                           && a.LanguageId == languageId);

            if (genericLanguageAttribute == null && string.IsNullOrWhiteSpace(value))
                return null;

            else if (genericLanguageAttribute == null && !string.IsNullOrWhiteSpace(value))
            {
                isInsert = true;

                return new LanguageValues
                {
                    EntityId = entityId,
                    TableName = tableName,
                    FieldName = fieldName,
                    LanguageId = languageId,
                    Value = value
                };
            }
            else
            {
                genericLanguageAttribute.Value = string.IsNullOrWhiteSpace(value) ? genericLanguageAttribute.Value : value;

                return autoMapperConfiguration.MapObject<LanguageValuesModel, LanguageValues>(genericLanguageAttribute);
            }
        }

        #endregion

        public bool isInCapacity(string path)
        {
            var capacity = Convert.ToInt32(configuration["ImportFilePaths:Capacity"]);
            var readedData = ReadProductModel(path);

            return readedData.Count() > capacity ? false : true;
        }

        public bool isValidDocument(string extension)
        {
            string[] ValidExtensions = new string[] { ".cvs", ".xls", ".xlsx" };

            bool isValid = false;

            var Validation = Array.IndexOf(ValidExtensions, extension.ToLower());

            if (Validation > -1)
                isValid = true;

            return isValid;
        }

        public string DownloadExcelTemplateUrl()
        {
            var excelTemplatePath = configuration.GetValue<string>("ImportFilePaths:ExcelTemplateUrl");

            return $"{hostingEnvironment.WebRootPath}{excelTemplatePath}";
        }


        #region Read Excel

        public IEnumerable<SynchronizationNodeModel> ReadSynchronizationNodeModel(string path)
        {
            var nodeList = new List<SynchronizationNodeModel>();
            using (var stream = File.Open(path, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    reader.Read(); // skip first row

                    while (reader.Read())
                    {
                        var row = new SynchronizationNodeModel
                        {
                            MapId = reader.GetValue(0) == null ? string.Empty : Convert.ChangeType(reader.GetValue(0), reader.GetValue(0).GetType()).ToString(),
                            NodeStr = reader.GetValue(3) == null ? string.Empty : Convert.ChangeType(reader.GetValue(3), reader.GetValue(3).GetType()).ToString()
                        };
                        nodeList.Add(row);
                    }
                }
            }

            return nodeList;
        }
        public IEnumerable<StoreProductFixingModel> ReadStoreProductFixing(string path)
        {
            var nodeList = new List<StoreProductFixingModel>();
            using (var stream = File.Open(path, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    reader.Read(); // skip first row
                 
                    while (reader.Read())
                    {
                        var row = new StoreProductFixingModel
                        {
                            MapId = reader.GetValue(0) == null ? 0 : Convert.ToInt32(Convert.ChangeType(reader.GetValue(0), reader.GetValue(0).GetType())),
                            Price = reader.GetValue(2) == null ? 0 : Convert.ToDecimal(Convert.ChangeType(reader.GetValue(2), reader.GetValue(2).GetType())),
                            IsFixed = reader.GetValue(3) == null ? false : Convert.ToBoolean(Convert.ChangeType(reader.GetValue(3), reader.GetValue(3).GetType())),
                            Point = reader.GetValue(4) == null ? 0 : Convert.ToDecimal(Convert.ChangeType(reader.GetValue(4), reader.GetValue(4).GetType())),
                            IsFixedPoint = reader.GetValue(5) == null ? false : Convert.ToBoolean(Convert.ChangeType(reader.GetValue(5), reader.GetValue(5).GetType())),
                            ErpPrice = reader.GetValue(6) == null ? 0 : Convert.ToDecimal(Convert.ChangeType(reader.GetValue(6), reader.GetValue(6).GetType())),
                            ErpPoint = reader.GetValue(7) == null ? 0 : Convert.ToDecimal(Convert.ChangeType(reader.GetValue(7), reader.GetValue(7).GetType())),
                            CatalogCode = reader.GetValue(8) == null ? string.Empty : Convert.ChangeType(reader.GetValue(8), reader.GetValue(8).GetType()).ToString(),
                            CatalogName = reader.GetValue(9) == null ? string.Empty : Convert.ChangeType(reader.GetValue(9), reader.GetValue(9).GetType()).ToString(),
                            VatRate = reader.GetValue(10) != null ? Convert.ToInt32(Convert.ChangeType(reader.GetValue(10), reader.GetValue(10).GetType())) : (int?)null
                        };
                        nodeList.Add(row);
                    }
                }
            }

            return nodeList;
        }
        public IEnumerable<ProductImportModel> ReadProductModel(string path)
        {
            var productList = new List<ProductImportModel>();

            using (var stream = File.Open(path, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    reader.Read(); // skip first row

                    while (reader.Read())
                    {
                        var row = new ProductImportModel
                        {
                            Sku = reader.GetValue(0) == null ? string.Empty : reader.GetString(0)
                        };

                        if (string.IsNullOrWhiteSpace(row.Sku))
                            continue;

                        row.Barcode = reader.GetValue(1) == null ? string.Empty : reader.GetString(1);
                        row.Name = reader.GetValue(2) == null ? string.Empty : reader.GetString(2);
                        row.Title = reader.GetValue(3) == null ? string.Empty : reader.GetString(3);
                        row.ShortDescription = reader.GetValue(4) == null ? string.Empty : reader.GetString(4);
                        row.Description = reader.GetValue(5) == null ? string.Empty : reader.GetString(5);

                        row.BulletPoint1 = reader.GetValue(6) == null ? string.Empty : reader.GetString(6);
                        row.BulletPoint2 = reader.GetValue(7) == null ? string.Empty : reader.GetString(7);
                        row.BulletPoint3 = reader.GetValue(8) == null ? string.Empty : reader.GetString(8);
                        row.BulletPoint4 = reader.GetValue(9) == null ? string.Empty : reader.GetString(9);
                        row.BulletPoint5 = reader.GetValue(10) == null ? string.Empty : reader.GetString(10);
                        row.ShippingWeight = reader.GetValue(11) == null ? string.Empty : reader.GetString(11);
                        row.SearchEngineTerms = reader.GetValue(12) == null ? string.Empty : reader.GetString(12);

                        row.BuyingPrice = reader.GetValue(13) == null ? 0 : Convert.ToDecimal(reader.GetDouble(13));
                        row.CurrencyId = reader.GetValue(14) == null ? 0 : Convert.ToInt32(reader.GetDouble(14));
                        row.MarketPrice = reader.GetValue(15) == null ? 0 : Convert.ToDecimal(reader.GetDouble(15));
                        row.PsfCurrencyId = reader.GetValue(16) == null ? 0 : Convert.ToInt32(reader.GetDouble(16));
                        row.CorporatePrice = reader.GetValue(17) == null ? 0 : Convert.ToDecimal(reader.GetDouble(17));
                        row.CorporateCurrencyId = reader.GetValue(18) == null ? 0 : Convert.ToInt32(reader.GetDouble(18));
                        row.FobPrice = reader.GetValue(19) == null ? 0 : Convert.ToDecimal(reader.GetDouble(19));
                        row.FobCurrencyId = reader.GetValue(20) == null ? 0 : Convert.ToInt32(reader.GetDouble(20));
                        row.Desi = reader.GetValue(21) == null ? 0 : Convert.ToDecimal(reader.GetDouble(21));
                        row.AbroadDesi = reader.GetValue(22) == null ? 0 : Convert.ToDecimal(reader.GetDouble(22));
                        row.VatRate = reader.GetValue(23) == null ? -1 : Convert.ToInt32(reader.GetDouble(23));

                        productList.Add(row);
                    }
                }
            }

            return productList;
        }

        #endregion

        #region Update Via Excel

        public void UpdateProductModel(IEnumerable<ProductImportModel> products, int languageId, out StringBuilder errorList)
        {
            errorList = new StringBuilder();
            int defaultLanguageId = languageService.AllLanguagesWithDefault().Where(a => a.IsDefault).FirstOrDefault().Id;

            foreach (var item in products)
            {
                try
                {
                    List<ProductAttributeMapping> attrList = new List<ProductAttributeMapping>();
                    List<LanguageValues> languageValuesUpdate = new List<LanguageValues>();
                    List<LanguageValues> languageValuesInsert = new List<LanguageValues>();

                    var product = productService.ProductBySku(item.Sku);
                    if (product != null)
                    {
                        product.Barcode = string.IsNullOrWhiteSpace(item.Barcode) ? product.Barcode : item.Barcode;
                        product.BuyingPrice = item.BuyingPrice == 0 ? (product.BuyingPrice == null ? 0 : product.BuyingPrice.Value) : item.BuyingPrice;
                        product.CurrencyId = item.CurrencyId == 0 ? (product.CurrencyId == null ? 1 : product.CurrencyId.Value) : item.CurrencyId;
                        product.PsfPrice = item.MarketPrice == 0 ? (product.PsfPrice == null ? 0 : product.PsfPrice.Value) : item.MarketPrice;
                        product.PsfCurrencyId = item.PsfCurrencyId == 0 ? (product.PsfCurrencyId == null ? 1 : product.PsfCurrencyId.Value) : item.PsfCurrencyId;
                        product.CorporatePrice = item.CorporatePrice == 0 ? (product.CorporatePrice == null ? 0 : product.CorporatePrice.Value) : item.CorporatePrice;
                        product.CorporateCurrencyId = item.CorporateCurrencyId == 0 ? (product.CorporateCurrencyId == null ? 1 : product.CorporateCurrencyId.Value) : item.CorporateCurrencyId;
                        product.FobPrice = item.FobPrice == 0 ? (product.FobPrice == null ? 0 : product.FobPrice.Value) : item.FobPrice;
                        product.FobCurrencyId = item.FobCurrencyId == 0 ? (product.FobCurrencyId == null ? 1 : product.FobCurrencyId.Value) : item.FobCurrencyId;
                        product.Desi = item.Desi == 0 ? (product.Desi == null ? 0 : product.Desi.Value) : item.Desi;
                        product.AbroadDesi = item.AbroadDesi == 0 ? (product.AbroadDesi == null ? 0 : product.Desi.Value) : item.AbroadDesi;
                        product.VatRateId = item.VatRate == -1 ? product.VatRateId : vatRateRepo.Find(x => x.Value == item.VatRate).Id;

                        var attrBullet1 = productAttributeRepo.Filter(a => a.ProductId == product.Id && a.AttributesId.Value == 2).FirstOrDefault();
                        var attrBullet2 = productAttributeRepo.Filter(a => a.ProductId == product.Id && a.AttributesId.Value == 4).FirstOrDefault();
                        var attrBullet3 = productAttributeRepo.Filter(a => a.ProductId == product.Id && a.AttributesId.Value == 8).FirstOrDefault();
                        var attrBullet4 = productAttributeRepo.Filter(a => a.ProductId == product.Id && a.AttributesId.Value == 9).FirstOrDefault();
                        var attrBullet5 = productAttributeRepo.Filter(a => a.ProductId == product.Id && a.AttributesId.Value == 10).FirstOrDefault();
                        var attrShippingWeight = productAttributeRepo.Filter(a => a.ProductId == product.Id && a.AttributesId.Value == 12).FirstOrDefault();

                        if (languageId == defaultLanguageId)
                        {
                            product.Name = string.IsNullOrWhiteSpace(item.Name) ? product.Name : item.Name;
                            product.ShortDescription = string.IsNullOrWhiteSpace(item.ShortDescription) ? product.ShortDescription : item.ShortDescription;
                            product.Title = string.IsNullOrWhiteSpace(item.Title) ? product.Title : item.Title;
                            product.Description = string.IsNullOrWhiteSpace(item.Description) ? product.Description : item.Description;
                            product.SearchEngineTerms = string.IsNullOrWhiteSpace(item.SearchEngineTerms) ? product.SearchEngineTerms : item.SearchEngineTerms;
                            if (product.SearchEngineTerms != null)
                                product.SearchEngineTerms = Convertions.SubStringWithMaxCharNumber(product.SearchEngineTerms, 249);

                            var data = autoMapperConfiguration.MapObject<ProductModel, Product>(product);
                            productRepo.Update(data);

                            attrBullet1.RequiredAttributeValue = string.IsNullOrWhiteSpace(item.BulletPoint1) ? attrBullet1.RequiredAttributeValue : item.BulletPoint1;
                            attrList.Add(attrBullet1);
                            attrBullet2.RequiredAttributeValue = string.IsNullOrWhiteSpace(item.BulletPoint2) ? attrBullet2.RequiredAttributeValue : item.BulletPoint2;
                            attrList.Add(attrBullet2);
                            attrBullet3.RequiredAttributeValue = string.IsNullOrWhiteSpace(item.BulletPoint3) ? attrBullet3.RequiredAttributeValue : item.BulletPoint3;
                            attrList.Add(attrBullet3);
                            attrBullet4.RequiredAttributeValue = string.IsNullOrWhiteSpace(item.BulletPoint4) ? attrBullet4.RequiredAttributeValue : item.BulletPoint4;
                            attrList.Add(attrBullet4);
                            attrBullet5.RequiredAttributeValue = string.IsNullOrWhiteSpace(item.BulletPoint5) ? attrBullet5.RequiredAttributeValue : item.BulletPoint5;
                            attrList.Add(attrBullet5);
                            attrShippingWeight.RequiredAttributeValue = string.IsNullOrWhiteSpace(item.ShippingWeight) ? attrShippingWeight.RequiredAttributeValue : item.ShippingWeight;
                            attrList.Add(attrShippingWeight);

                            productAttributeRepo.UpdateRange(attrList);
                        }
                        else
                        {

                            //TODO: Burayı sevmedim bir daha bak.

                            bool isInsert = false;

                            var name = ArrangeLanguageBasedList(product.Id, "Product", "Name", languageId, item.Name, out isInsert);
                            if (name != null)
                            {
                                if (isInsert)
                                    languageValuesInsert.Add(name);
                                else
                                    languageValuesUpdate.Add(name);
                            }

                            var title = ArrangeLanguageBasedList(product.Id, "Product", "Title", languageId, item.Title, out isInsert);
                            if (title != null)
                            {
                                if (isInsert)
                                    languageValuesInsert.Add(title);
                                else
                                    languageValuesUpdate.Add(title);
                            }

                            var shortDescription = ArrangeLanguageBasedList(product.Id, "Product", "ShortDescription", languageId, item.ShortDescription, out isInsert);
                            if (shortDescription != null)
                            {
                                if (isInsert)
                                    languageValuesInsert.Add(shortDescription);
                                else
                                    languageValuesUpdate.Add(shortDescription);
                            }

                            var description = ArrangeLanguageBasedList(product.Id, "Product", "Description", languageId, item.Description, out isInsert);
                            if (description != null)
                            {
                                if (isInsert)
                                    languageValuesInsert.Add(description);
                                else
                                    languageValuesUpdate.Add(description);
                            }

                            var searchEngineTerms = ArrangeLanguageBasedList(product.Id, "Product", "SearchEngineTerms", languageId, item.SearchEngineTerms, out isInsert);
                            if (searchEngineTerms != null)
                            {
                                searchEngineTerms.Value = Convertions.SubStringWithMaxCharNumber(searchEngineTerms.Value, 249);
                                if (isInsert)
                                    languageValuesInsert.Add(searchEngineTerms);
                                else
                                    languageValuesUpdate.Add(searchEngineTerms);
                            }

                            var bulletPoint1 = ArrangeLanguageBasedList(attrBullet1.Id, "ProductAttributeMapping", "BulletPoint1", languageId, item.BulletPoint1, out isInsert);
                            if (bulletPoint1 != null)
                            {
                                if (isInsert)
                                    languageValuesInsert.Add(bulletPoint1);
                                else
                                    languageValuesUpdate.Add(bulletPoint1);
                            }


                            var bulletPoint2 = ArrangeLanguageBasedList(attrBullet2.Id, "ProductAttributeMapping", "BulletPoint2", languageId, item.BulletPoint2, out isInsert);
                            if (bulletPoint2 != null)
                            {
                                if (isInsert)
                                    languageValuesInsert.Add(bulletPoint2);
                                else
                                    languageValuesUpdate.Add(bulletPoint2);
                            }

                            var bulletPoint3 = ArrangeLanguageBasedList(attrBullet3.Id, "ProductAttributeMapping", "BulletPoint3", languageId, item.BulletPoint3, out isInsert);
                            if (bulletPoint3 != null)
                            {
                                if (isInsert)
                                    languageValuesInsert.Add(bulletPoint3);
                                else
                                    languageValuesUpdate.Add(bulletPoint3);
                            }

                            var bulletPoint4 = ArrangeLanguageBasedList(attrBullet4.Id, "ProductAttributeMapping", "BulletPoint4", languageId, item.BulletPoint4, out isInsert);
                            if (bulletPoint4 != null)
                            {
                                if (isInsert)
                                    languageValuesInsert.Add(bulletPoint4);
                                else
                                    languageValuesUpdate.Add(bulletPoint4);
                            }

                            var bulletPoint5 = ArrangeLanguageBasedList(attrBullet5.Id, "ProductAttributeMapping", "BulletPoint5", languageId, item.BulletPoint5, out isInsert);
                            if (bulletPoint5 != null)
                            {
                                if (isInsert)
                                    languageValuesInsert.Add(bulletPoint5);
                                else
                                    languageValuesUpdate.Add(bulletPoint5);
                            }

                            var shippingWeight = ArrangeLanguageBasedList(attrShippingWeight.Id, "ProductAttributeMapping", "ShippingWeight", languageId, item.ShippingWeight, out isInsert);
                            if (shippingWeight != null)
                            {
                                if (isInsert)
                                    languageValuesInsert.Add(shippingWeight);
                                else
                                    languageValuesUpdate.Add(shippingWeight);
                            }

                            if (languageValuesInsert.Count > 0)
                                languageValueRepo.AddRange(languageValuesInsert);

                            if (languageValuesUpdate.Count > 0)
                                languageValueRepo.BulkUpdate(languageValuesUpdate);

                        }
                    }
                }
                catch (Exception ex)
                {
                    errorList.AppendLine($"{item.Sku} - {ex.Message}");
                    continue;
                }
            }
        }
        public void UpdateStoreNodes(IEnumerable<SynchronizationNodeModel> nodeModels, out List<string> errorCodes)
        {
            errorCodes = new List<string>();
            List<StoreProductMapping> nodeList = new List<StoreProductMapping>();

            foreach (var item in nodeModels)
            {
                int mapId = Convert.ToInt32(item.MapId);
                var map = storeProductMappingRepo.Table().FirstOrDefault(x => x.Id == mapId);
                if (map != null)
                {
                    map.StoreCategory = item.NodeStr;
                    nodeList.Add(map);
                }
            }

            storeProductMappingRepo.BulkUpdate(nodeList);
        }
        public void UpdateStoreProductFixing(IEnumerable<StoreProductFixingModel> nodeModels, out List<string> errorCodes)
        {
            errorCodes = new List<string>();
            List<StoreProductMapping> nodeList = new List<StoreProductMapping>();
            var list = nodeModels.ToList();
            var sProducts = storeProductMappingRepo.Table().Where(x => nodeModels.Any(m => m.MapId == x.Id)).ToList();
            var nLenght = list.Count;
            for (int i = 0; i < nLenght; i++)
            {
                var map = sProducts.FirstOrDefault(x => x.Id == list[i].MapId);
                if (map != null)
                {
                    if (list[i].IsFixed)
                        map.Price = list[i].Price;
                    if (list[i].IsFixedPoint.HasValue && list[i].IsFixedPoint.Value)
                        map.Point = list[i].Point;


                    if (list[i].ErpPrice != 0)
                        map.ErpPrice = list[i].ErpPrice;
                    if (!string.IsNullOrWhiteSpace(list[i].CatalogName))
                        map.CatalogName = list[i].CatalogName;
                    if (!string.IsNullOrWhiteSpace(list[i].CatalogCode))
                        map.CatalogCode = list[i].CatalogCode;
                    if (list[i].ErpPoint != 0)
                        map.ErpPoint = list[i].ErpPoint;

                    map.IsFixedPoint = list[i].IsFixedPoint;
                    map.IsFixed = list[i].IsFixed;
                    map.VatValue = list[i].VatRate;

                    nodeList.Add(map);
                }
            }
            storeProductMappingRepo.BulkUpdate(nodeList);
        }

        #endregion
    }
}
