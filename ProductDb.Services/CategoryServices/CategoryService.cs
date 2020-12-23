using ProductDb.Common.Enums;
using ProductDb.Data.BiggBrandsDb;
using ProductDb.Mapping.BiggBrandDbModels;
using ProductDb.Mapping.AutoMapperConfigurations;
using ProductDb.Repositories.BaseRepositories;
using ProductDb.Repositories.UnitOfWorks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using ProductDb.Services.LanguageServices;
using ProductDb.Services.ProductServices;

namespace ProductDb.Services.CategoryServices
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IAutoMapperConfiguration autoMapper;
        private readonly ILanguageService languageService;
        private readonly IParentProductService parentProductService;
        private readonly IGenericRepository<Category> categoryRepo;
        private readonly IGenericRepository<CategoryAttributeMapping> categoryAttributeRepo;
        private readonly IGenericRepository<Attributes> attributesRepo;

        private readonly Expression<Func<Category, bool>> defaultCategoryFilter = null;

        public CategoryService(IUnitOfWork unitOfWork, IAutoMapperConfiguration autoMapper,
            ILanguageService languageService, IParentProductService parentProductService)
        {
            this.unitOfWork = unitOfWork;
            this.autoMapper = autoMapper;

            this.languageService = languageService;
            this.parentProductService = parentProductService;

            categoryRepo = this.unitOfWork.Repository<Category>();
            categoryAttributeRepo = this.unitOfWork.Repository<CategoryAttributeMapping>();
            attributesRepo = this.unitOfWork.Repository<Attributes>();

            defaultCategoryFilter = entity => entity.State == (int)State.Active;
        }

        public ICollection<CategoryModel> AllCategories()
        {
            return autoMapper.MapCollection<Category, CategoryModel>(categoryRepo.GetAll()).ToList();
        }

        public ICollection<CategoryModel> AllActiveCategories()
        {
            return autoMapper.MapCollection<Category, CategoryModel>(categoryRepo.FindAll(defaultCategoryFilter)).ToList();
        }

        public ICollection<CategoryModel> AllCategoriesExceptSelf(int id)
        {
            return autoMapper.MapCollection<Category, CategoryModel>(categoryRepo.FindAll(x => x.Id != id && x.State == (int)State.Active)).ToList();
        }

        public ICollection<CategoryModel> AllCategoriesWithParentNames()
        {
            var list = AllCategories();

            foreach (var item in list)
            {
                item.CategoryNameWithParents = string.Join(" >> ", CategoryWithParents(item.Id).Select(x => x.Name).Reverse().ToList());
                item.SkuCount = parentProductService.ParentProductsCountWithCategory(item.Id);
            }

            return list;
        }

        public ICollection<CategoryModel> AllActiveCategoriesWithParentNames()
        {
            var list = AllActiveCategories();

            foreach (var item in list)
                item.CategoryNameWithParents = string.Join(" >> ", CategoryWithParents(item.Id).Select(x => x.Name).Reverse().ToList());

            return list;
        }

        public ICollection<CategoryModel> AllCategoriesWithParentNamesExceptSelf(int id)
        {
            var list = AllCategoriesExceptSelf(id);

            foreach (var item in list)
                item.CategoryNameWithParents = string.Join(" >> ", CategoryWithParents(item.Id).Select(x => x.Name).Reverse().ToList());

            return list;
        }

        public CategoryModel CategoryById(int id)
        {
            return autoMapper.MapObject<Category, CategoryModel>(categoryRepo.GetById(id));
        }

        public CategoryModel AddNewCategory(CategoryModel model)
        {
            var entity = autoMapper.MapObject<CategoryModel, Category>(model);

            var savedEntity = categoryRepo.Add(entity);

            return autoMapper.MapObject<Category, CategoryModel>(savedEntity);

        }

        public CategoryModel EditCategory(CategoryModel model)
        {
            var entity = autoMapper.MapObject<CategoryModel, Category>(model);

            var updatedEntity = categoryRepo.Update(entity);

            return autoMapper.MapObject<Category, CategoryModel>(updatedEntity);
        }

        public int DeleteCategoryById(int id)
        {
            var entity = categoryRepo.GetById(id);

            try
            {
                var categoryAttributesToDelete = categoryAttributeRepo.FindAll(x => x.CategoryId == entity.Id).ToList();

                categoryAttributeRepo.DeleteRange(categoryAttributesToDelete);

                return categoryRepo.Delete(entity);

            }
            catch
            {
                return -1;
            }
        }

        public ICollection<CategoryModel> CategoryWithParents(int id)
        {
            var categoryWithParents = new List<CategoryModel>();

            var category = categoryRepo.GetById(id);

            while (category.ParentCategoryId.HasValue)
            {
                categoryWithParents.Add(autoMapper.MapObject<Category, CategoryModel>(category));
                category = categoryRepo.GetById(category.ParentCategoryId.Value);
            }

            categoryWithParents.Add(autoMapper.MapObject<Category, CategoryModel>(category)); //En son ana kategorisi null olanı ekler. Anaların anası.

            return categoryWithParents;
        }

        public ICollection<CategoryModel> CategoryWithParentsAndLanguages(int id, int languageId)
        {
            var categoryWithParents = new List<CategoryModel>();

            var category = categoryRepo.GetById(id);

            while (category.ParentCategoryId.HasValue)
            {
                var lValue = languageService.LanguageValueByEntityIdAndTableName(category.ParentCategoryId.Value, "Category", languageId);
                if (lValue != null)
                    category.Name = lValue.Value;

                categoryWithParents.Add(autoMapper.MapObject<Category, CategoryModel>(category));
                category = categoryRepo.GetById(category.ParentCategoryId.Value);
            }

            categoryWithParents.Add(autoMapper.MapObject<Category, CategoryModel>(category)); //En son ana kategorisi null olanı ekler. Anaların anası.

            return categoryWithParents;
        }

        public List<string> CategoreLanguageTree(int id, int languageId)
        {
            Category category = categoryRepo.GetById(id);
            List<string> categorylist = new List<string>();
            while (category != null)
            {
                if (category.ParentCategoryId.HasValue)
                {
                    var lValue = languageService.LanguageValueByEntityIdAndTableName(category.Id, "Category", languageId);
                    if (lValue != null)
                        categorylist.Add(lValue.Value);
                    else
                        categorylist.Add(category.Name);
                    category = categoryRepo.GetById(category.ParentCategoryId.Value);
                }
                else
                {
                    var lValue = languageService.LanguageValueByEntityIdAndTableName(category.Id, "Category", languageId);
                    if (lValue != null)
                        categorylist.Add(lValue.Value);
                    else
                        categorylist.Add(category.Name);
                    category = null;
                }
            }
            categorylist.Reverse();
            return categorylist;
        }

        public string CategoryWithParentNames(int id)
        {
            return string.Join(" >> ", CategoryWithParents(id).Reverse().Select(x => x.Name));
        }

        public string CategoryWithParentNamesAndLanguages(int id, int languageId)
        {
            return string.Join(" >> ", CategoreLanguageTree(id, languageId));
        }

        public int CategoryCount()
        {
            return categoryRepo.Count();
        }

        #region CategoryAttributes

        public ICollection<CategoryAttributeMappingModel> CategoryParentAttributes(int categoryId)
        {
            var categoryAttributesMapping = new List<CategoryAttributeMappingModel>();

            var categoryWithParents = CategoryWithParents(categoryId);

            foreach (var category in categoryWithParents.Where(x => x.Id != categoryId).Reverse())
            {
                var categoryAttributesRange = autoMapper.MapCollection<CategoryAttributeMapping, CategoryAttributeMappingModel>
                    (categoryAttributeRepo.Filter(x => x.CategoryId == category.Id && x.State == (int)State.Active && x.IsRequired, null, "Attributes"));
                categoryAttributesMapping.AddRange(categoryAttributesRange);
            }

            return categoryAttributesMapping;
        }

        public ICollection<CategoryAttributeMappingModel> CategoryAttributes(int categoryId)
        {
            var entities = categoryAttributeRepo.Filter(x => x.CategoryId == categoryId && x.State == (int)State.Active, null, "Attributes");

            return autoMapper.MapCollection<CategoryAttributeMapping, CategoryAttributeMappingModel>(entities).ToList();
        }

        public CategoryAttributeMappingModel AddNewCategoryAttributeMapping(int attributeId, int categoryId, bool isRequired = false)
        {
            var categoryAttribute = new CategoryAttributeMapping
            {
                AttributesId = attributeId,
                CategoryId = categoryId,
                IsRequired = isRequired
            };

            return autoMapper.MapObject<CategoryAttributeMapping, CategoryAttributeMappingModel>(categoryAttributeRepo.Add(categoryAttribute));
        }

        public CategoryAttributeMappingModel EditCategoryAttributeMapping(CategoryAttributeMappingModel model)
        {
            var entity = autoMapper.MapObject<CategoryAttributeMappingModel, CategoryAttributeMapping>(model);

            return autoMapper.MapObject<CategoryAttributeMapping, CategoryAttributeMappingModel>(entity);
        }

        public bool EditCategoryAttributeMappings(List<CategoryAttributeMappingModel> models)
        {
            var entities = autoMapper.MapCollection<CategoryAttributeMappingModel, CategoryAttributeMapping>(models);

            return categoryAttributeRepo.UpdateRange(entities.ToList()) == -1 ? false : true;
        }

        public int DeleteCategoryAttributeMapping(CategoryAttributeMappingModel model)
        {
            var entity = autoMapper.MapObject<CategoryAttributeMappingModel, CategoryAttributeMapping>(model);

            return categoryAttributeRepo.Delete(entity);
        }

        public int DeleteCategoryAttributeMappingWithIdReturnCategoryId(int categoryAttributeMappingId)
        {
            var entity = categoryAttributeRepo.GetById(categoryAttributeMappingId);

            var categoryId = entity.CategoryId;

            categoryAttributeRepo.Delete(entity);

            return categoryId ?? 0;
        }

        public bool IsCategoryDefined(string name)
        {
            return categoryRepo.Filter(x => x.Name.ToLowerInvariant() == name.ToLowerInvariant()).Any();
        }

        #endregion


    }
}
