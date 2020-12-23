using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using ProductDb.Common.Cache;
using ProductDb.Data.BiggBrandsDb;
using ProductDb.Mapping.AutoMapperConfigurations;
using ProductDb.Mapping.BiggBrandDbModels;
using ProductDb.Repositories.BaseRepositories;
using ProductDb.Repositories.UnitOfWorks;
using ProductDb.Services.AttributesServices;
using ProductDb.Services.ProductServices;

namespace ProductDb.Services.ProductVariantServices
{
    public class ProductVariantService : IProductVariantService
    {
        private readonly IProductService productService;
        private readonly IAutoMapperConfiguration autoMapper;
        private readonly IGenericRepository<Product> productRepo;
        private readonly IGenericRepository<ParentProduct> parentProductRepo;
        private readonly IGenericRepository<ProductVariant> productVariantRepo;
        private readonly ICacheManager cacheManager;
        private readonly IParentProductService parentProductService;
        private readonly IAttributesService attributesService;

        public ProductVariantService(IUnitOfWork unitOfWork,
            IAutoMapperConfiguration autoMapper,
            IParentProductService parentProductService,
            IProductService productService, ICacheManager cacheManager,
            IAttributesService attributesService)
        {
            this.productService = productService;
            this.autoMapper = autoMapper;
            this.parentProductService = parentProductService;
            this.attributesService = attributesService;
            productRepo = unitOfWork.Repository<Product>();
            parentProductRepo = unitOfWork.Repository<ParentProduct>();
            productVariantRepo = unitOfWork.Repository<ProductVariant>();
            this.cacheManager = cacheManager;
        }
        public void Add(List<ProductVariantModel> productVariants)
        {
            var datas = autoMapper.MapCollection<ProductVariantModel, ProductVariant>(productVariants).ToList();
            datas.ForEach(x => x.ParentProduct = null);
            productVariantRepo.AddRange(datas);
        }
        public IEnumerable<ProductVariantModel> AllProductVariant()
        {
            return autoMapper.MapCollection<ProductVariant, ProductVariantModel>(productVariantRepo.GetAll());
        }

        public IEnumerable<ProductModel> AllProductVariantWithProduct()
        {
            var datas = productVariantRepo.Table().Include(x => x.Product).Where(x => x.BaseId == null);
            return autoMapper.MapCollection<Product, ProductModel>(datas.Select(x => x.Product));
        }
        public IEnumerable<ProductVariantModel> AllProductVariants()
        {
            IEnumerable<ProductVariantModel> products = null;

            if (!cacheManager.TryGetValue(CacheStatics.ProductVariantCache, out products))
            {
                var datas = productVariantRepo.Table().Include(x => x.Product).Where(x => x.BaseId != null);
                products = autoMapper.MapCollection<ProductVariant, ProductVariantModel>(datas).ToList();
                cacheManager.Set(CacheStatics.ProductVariantCache, products, CacheStatics.ProductVariantCacheTime);
            }
            return products;
        }

        public IList<ProductModel> GetParentProductsVariantById(int id, List<int> IDs)
        {
            var count = IDs.Count;

            List<Product> filteredData = new List<Product>();

            var products = productRepo.Filter(x => x.ParentProductId == id, null,
                "ProductAttributeMappings").ToList();

            if (IDs.Count > 0)
            {
                for (int i = 0; i < products.Count; i++)
                {
                    var eqCount = 0;
                    var isContain = products[i].ProductAttributeMappings.Select(x => x.AttributesId);
                    foreach (var item in isContain)
                    {
                        if (IDs.Any(x => x == item))
                            eqCount++;
                    }
                    if (eqCount == count)
                        filteredData.Add(products[i]);
                }
            }

            //var variantedProductIDs = productVariantRepo.Filter(x => x.ParentProductId == id && x.BaseId != null).Select(x => x.ProductId);

            var variantedProductIDs = productVariantRepo.Filter(x => x.ParentProductId == id && x.BaseId != null)
                .Select(x => new ProductVariant()
                {
                    Id = x.Id,
                    ProductId = x.ProductId,
                    ProductAttributes = x.ProductAttributes
                });

            // new varianted products
            List<ProductVariant> newVariantedList = new List<ProductVariant>();

            var attributeCount = IDs.Count;
           
            foreach (var variantedProduct in variantedProductIDs)
            {
                int selector = 0;
                var attributes = variantedProduct.ProductAttributes.Split(",");
                foreach (var item in attributes)
                {
                    if (IDs.Contains(int.Parse(item)))
                        selector++;
                }
                if (selector == attributeCount)
                    newVariantedList.Add(variantedProduct);
            }

            // Attribute ID Control
            //variantedProductIDs = variantedProductIDs.Where(x => x.)

            filteredData = filteredData.Except(products.Where(x => newVariantedList.Any(k => k.ProductId == x.Id)).ToList()).ToList();
            //filteredData = filteredData.Except(products.Where(x => variantedProductIDs.Any(k => k == x.Id)).ToList()).ToList();

            return autoMapper.MapCollection<Product, ProductModel>(filteredData).ToList();
        }
        //  new
        public IList<ProductVariantModel> GetProductsVariant(int id)
        {
            var variants = productVariantRepo.Filter(x => x.BaseId == id, null, "Product");
            var mVariants = autoMapper.MapCollection<ProductVariant, ProductVariantModel>(variants).ToList();

            foreach (var item in mVariants)
            {
                var parent = parentProductRepo.Table().FirstOrDefault(x => x.Id == item.ParentProductId);
                item.ParentProductSku = parent != null ? parent.Sku : string.Empty;
            }
            return mVariants;
        }

        // Attribute Control
        public IList<ProductVariantModel> GetProductsVariant(int id,List<int> IDs)
        {
            var variants = productVariantRepo.Filter(x => x.BaseId == id, null, "Product");
            var mVariants = autoMapper.MapCollection<ProductVariant, ProductVariantModel>(variants).ToList();

            var attributeCount = IDs.Count;
            List<ProductVariantModel> newVariantedList = new List<ProductVariantModel>();

            foreach (var variantedProduct in mVariants)
            {
                int selector = 0;
                var attributes = variantedProduct.ProductAttributes.Split(",");
                foreach (var item in attributes)
                {
                    if (IDs.Contains(int.Parse(item)))
                        selector++;
                }
                if (selector == attributeCount)
                    newVariantedList.Add(variantedProduct);
            }

            foreach (var item in newVariantedList)
            {
                var parent = parentProductRepo.Table().FirstOrDefault(x => x.Id == item.ParentProductId);
                item.ParentProductSku = parent != null ? parent.Sku : string.Empty;
            }
            return newVariantedList;
        }

        public void ClearProductVariants(int id)
        {
            var variants = productVariantRepo.Table().Where(x => x.ParentProductId == id).ToList();
            productVariantRepo.DeleteRange(variants);
        }

        public ProductVariantModel ProductVariantByBaseId(int id)
        {
            return autoMapper.MapObject<ProductVariant, ProductVariantModel>(productVariantRepo.Filter(x => x.BaseId == id).FirstOrDefault());
        }
        public string PrepareAttributes(List<int> IDs)
        {
            StringBuilder attrlist = new StringBuilder();
            IDs.Sort();
            foreach (var item in IDs)
                attrlist.Append($"{item},");

            return attrlist.ToString().Remove(attrlist.Length - 1, 1);
        }

        public void ChangeProductVariant(int baseId, int productId)
        {
            var productVariant = productVariantRepo.Filter(x => x.ProductId == productId).FirstOrDefault();
            if (productVariant != null)
            {
                // parent
                var parent = productVariantRepo.Filter(x => x.ProductId == productVariant.BaseId).FirstOrDefault();

                if (baseId != 0)
                {
                    var rows = productVariantRepo.Filter(x => x.BaseId == productVariant.BaseId);
                    if (rows.Count == 1)
                    {
                        productVariantRepo.Delete(parent);
                        productVariantRepo.Delete(productVariant);
                    }
                    else
                    {
                        productVariant.BaseId = baseId;
                        productVariantRepo.Update(productVariant);
                    }
                }
                else
                {
                    var deletedBaseId = productVariant.BaseId;
                    productVariantRepo.Delete(productVariant);
                    var newRows = productVariantRepo.Filter(x => x.BaseId == productVariant.BaseId);
                    // Control
                    if (newRows.Count == 0)
                        productVariantRepo.Delete(parent);

                }
            }

        }
        // refactor
        public IList<ProductModel> GetParentProductsNotVariantById(int id, List<int> IDs)
        {
            var count = IDs.Count;

            List<Product> filteredData = new List<Product>();

            var products = productRepo.Filter(x => x.ParentProductId == id, null,
                "ProductAttributeMappings").ToList();

            if (IDs.Count > 0)
            {
                for (int i = 0; i < products.Count; i++)
                {
                    var eqCount = 0;
                    var isContain = products[i].ProductAttributeMappings.Select(x => x.AttributesId);
                    foreach (var item in isContain)
                    {
                        if (IDs.Any(x => x == item))
                            eqCount++;
                    }
                    if (eqCount == count)
                        filteredData.Add(products[i]);
                }
            }

            var variantedProductIDs = productVariantRepo.Filter(x => x.ParentProductId == id).Select(x => x.ProductId);

            filteredData = filteredData.Except(products.Where(x => variantedProductIDs.Any(k => k == x.Id)).ToList()).ToList();

            return autoMapper.MapCollection<Product, ProductModel>(filteredData).ToList();
        }

        public string GetProductsVariantsAttribute(int id)
        {
            List<string> variantedAttributes = new List<string>();

            var variants = productVariantRepo.Filter(x => x.ParentProductId == id);
            foreach (var item in variants)
            {
                var attibutes = item.ProductAttributes.Split(",");
                foreach (var attr in attibutes)
                {
                    variantedAttributes.Add( $"{attributesService.AttributesById(int.Parse(attr)).Name}" );
                }
            }
            var attributes = variantedAttributes.Distinct().ToList();
            var list = string.Join(",", attributes.ToArray());
            return list;
        }

        public ProductVariantModel GetProductVariantByProductId(int productId)
        {
            var variants = productVariantRepo.Table().FirstOrDefault(x => x.ProductId == productId && x.BaseId.HasValue);
            var mVariants = autoMapper.MapObject<ProductVariant, ProductVariantModel>(variants);

            return mVariants;
        }
    }
}
