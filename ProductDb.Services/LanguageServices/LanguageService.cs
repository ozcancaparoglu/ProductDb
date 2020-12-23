using ProductDb.Common.Enums;
using ProductDb.Data.BiggBrandsDb;
using ProductDb.Mapping.BiggBrandDbModels;
using ProductDb.Mapping.AutoMapperConfigurations;
using ProductDb.Repositories.BaseRepositories;
using ProductDb.Repositories.UnitOfWorks;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;

namespace ProductDb.Services.LanguageServices
{
    public class LanguageService : ILanguageService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IAutoMapperConfiguration autoMapper;

        private readonly IGenericRepository<Language> languageRepo;
        private readonly IGenericRepository<LanguageValues> languageValuesRepo;
        private readonly IGenericRepository<ProductAttributeMapping> productMappingRepo;
        private readonly IConfiguration configuration;

        public LanguageService(IUnitOfWork unitOfWork, IAutoMapperConfiguration autoMapper, IConfiguration configuration)
        {
            this.unitOfWork = unitOfWork;
            this.autoMapper = autoMapper;
            this.configuration = configuration;

            languageRepo = this.unitOfWork.Repository<Language>();
            languageValuesRepo = this.unitOfWork.Repository<LanguageValues>();
            productMappingRepo = this.unitOfWork.Repository<ProductAttributeMapping>();
        }

        public LanguageModel GetDefaultLanguage()
        {
            return autoMapper.MapObject<Language, LanguageModel>(languageRepo.Filter(a => a.IsDefault).FirstOrDefault());
        }

        public ICollection<LanguageModel> AllLanguages()
        {
            var entities = languageRepo.FindAll(x => x.IsDefault == false && x.State == (int)State.Active);

            return autoMapper.MapCollection<Language, LanguageModel>(entities).ToList();
        }

        public ICollection<LanguageModel> AllLanguagesWithDefault()
        {
            var entities = languageRepo.FindAll(x => x.State == (int)State.Active);

            return autoMapper.MapCollection<Language, LanguageModel>(entities).ToList();
        }

        public LanguageModel AddNewLanguage(LanguageModel model)
        {
            model.LogoPath = $"{configuration.GetValue<string>("PicturePaths:LanguageLogoPath")}{model.LogoPath}";

            var entity = autoMapper.MapObject<LanguageModel, Language>(model);

            var savedEntity = languageRepo.Add(entity);

            return autoMapper.MapObject<Language, LanguageModel>(savedEntity);
        }

        public int CreateLanguages()
        {
            var languages = new List<Language>
            {
                new Language { Name = "Turkish", Abbrevation = "tr", LogoPath = "/Assets/dist/img/flags/tr.png", IsDefault = true, State = (int)State.Active },
                new Language { Name = "English", Abbrevation = "gb", LogoPath = "/Assets/dist/img/flags/gb.png", IsDefault = false, State = (int)State.Active },
                new Language { Name = "German", Abbrevation = "de", LogoPath = "/Assets/dist/img/flags/de.png", IsDefault = false, State = (int)State.Active },
                new Language { Name = "Dutch", Abbrevation = "nl", LogoPath = "/Assets/dist/img/flags/nl.png", IsDefault = false, State = (int)State.Active },
                new Language { Name = "French", Abbrevation = "fr", LogoPath = "/Assets/dist/img/flags/fr.png", IsDefault = false, State = (int)State.Active },
                new Language { Name = "Russian", Abbrevation = "ru", LogoPath = "/Assets/dist/img/flags/ru.png", IsDefault = false, State = (int)State.Active },
            };

            return languageRepo.AddRange(languages);
        }

        public LanguageModel LanguageById(int id)
        {
            return autoMapper.MapObject<Language, LanguageModel>(languageRepo.GetById(id));
        }

        public LanguageModel EditLanguage(LanguageModel model)
        {
            var entity = autoMapper.MapObject<LanguageModel, Language>(model);

            #region Validations

            if (languageRepo.Filter(x => x.Id != entity.Id && x.Name.ToLowerInvariant() == entity.Name.ToLowerInvariant()).Count() > 0)
                return null;

            #endregion

            var savedEntity = languageRepo.Update(entity);

            #region Default Dil değişimi kontrolü

            if (savedEntity != null)
            {
                var isDefaultLanguages = languageRepo.Filter(x => x.Id != entity.Id && x.IsDefault == true).ToList();

                if (isDefaultLanguages.Count() > 0)
                {
                    foreach (var item in isDefaultLanguages)
                        item.IsDefault = false;

                    languageRepo.UpdateRange(isDefaultLanguages);
                }
            }
            #endregion

            return autoMapper.MapObject<Language, LanguageModel>(savedEntity);
        }

        public void SetState(int objectId)
        {
            var entity = languageRepo.GetById(objectId);

            if (entity.State == (int)State.Active)
                entity.State = (int)State.Passive;

            else
                entity.State = (int)State.Active;

            languageRepo.Update(entity);
        }

        #region LanguageValues

        public ICollection<LanguageValuesModel> LanguageValuesByEntityIdAndTableName(int id, string tableName)
        {
            var entities = languageValuesRepo.FindAll(x => x.EntityId == id && x.TableName == tableName && x.State == (int)State.Active);

            return autoMapper.MapCollection<LanguageValues, LanguageValuesModel>(entities).ToList();
        }

        public LanguageValuesModel LanguageValueByEntityIdAndTableName(int id, string tableName, int languageId)
        {
            var entity = languageValuesRepo.Filter(x => x.EntityId == id && x.TableName == tableName
            && x.State == (int)State.Active && x.LanguageId == languageId).FirstOrDefault();

            return autoMapper.MapObject<LanguageValues, LanguageValuesModel>(entity);
        }
        public Dictionary<int, List<LanguageValuesModel>> PrepareLanguageValues(ICollection<string> fieldNameList, string tableName)
        {
            var dicLanguageValues = new Dictionary<int, List<LanguageValuesModel>>();

            foreach (var language in languageRepo.FindAll(x => x.IsDefault == false && x.State == (int)State.Active))
            {
                var languageValues = new List<LanguageValuesModel>();

                foreach (var fieldName in fieldNameList)
                {
                    languageValues.Add(new LanguageValuesModel
                    {
                        EntityId = 0,
                        LanguageId = language.Id,
                        FieldName = fieldName,
                        TableName = tableName,
                        Value = ""
                    });
                }

                dicLanguageValues.Add(language.Id, languageValues);
            }

            return dicLanguageValues;
        }

        public Dictionary<int, List<LanguageValuesModel>> AttributeLanguageValues(List<ProductAttributeMapping> productAttributeMappings, string tableName)
        {
            var dicLanguageValues = new Dictionary<int, List<LanguageValuesModel>>();

            foreach (var language in languageRepo.FindAll(x => x.IsDefault == false && x.State == (int)State.Active))
            {
                var languageValues = new List<LanguageValuesModel>();

                foreach (var fieldName in productAttributeMappings)
                {
                    languageValues.Add(new LanguageValuesModel
                    {
                        EntityId = fieldName.Id,
                        LanguageId = language.Id,
                        FieldName = fieldName.Attributes.Name,
                        TableName = tableName,
                        Value = ""
                    });
                }

                dicLanguageValues.Add(language.Id, languageValues);
            }

            return dicLanguageValues;
        }
        public Dictionary<int, List<LanguageValuesModel>> PrepareLanguageValuesForEdit(ICollection<string> fieldNameList, string tableName, int id)
        {
            //TODO: Refactor

            var languageValues = LanguageValuesByEntityIdAndTableName(id, tableName);

            var allLanguageValues = PrepareLanguageValues(fieldNameList, tableName);

            foreach (var languageValue in languageValues)
            {
                try
                {
                    var forEditing = allLanguageValues[languageValue.LanguageId.Value].FirstOrDefault(x => x.FieldName == languageValue.FieldName && x.TableName == tableName);
                    if (forEditing != null)
                    {
                        for (int i = 0; i < allLanguageValues[languageValue.LanguageId.Value].Count; i++)
                        {
                            if (allLanguageValues[languageValue.LanguageId.Value][i].FieldName == languageValue.FieldName)
                                allLanguageValues[languageValue.LanguageId.Value][i] = languageValue;
                        }
                    }
                }
                catch
                {
                    continue;
                }
            }

            return allLanguageValues;

        }

        public Dictionary<int, List<LanguageValuesModel>> PrepareLanguageValuesForEditMultipleIds(ICollection<string> fieldNameList, string tableName, List<int> ids)
        {
            //TODO: Burası tutarsa üstteki fonksiyon önemini yitirir ona göre kodu modifiye et
            var languageValues = autoMapper.MapCollection<LanguageValues, LanguageValuesModel>(languageValuesRepo.FindAll(x => ids.Contains(x.EntityId) && x.TableName == tableName && x.State == (int)State.Active));

            // Değişiklik Yapıldı.
            var productMapping = productMappingRepo.Table().Where(x => ids.Any(y => y == x.Id)).Include(x => x.Attributes).ToList();

            var allLanguageValues = AttributeLanguageValues(productMapping, tableName);

            foreach (var languageValue in languageValues)
            {
                try
                {
                    var forEditing = allLanguageValues[languageValue.LanguageId.Value].FirstOrDefault(x => x.FieldName == languageValue.FieldName && x.TableName == tableName);
                    if (forEditing != null)
                    {
                        for (int i = 0; i < allLanguageValues[languageValue.LanguageId.Value].Count; i++)
                        {
                            if (allLanguageValues[languageValue.LanguageId.Value][i].FieldName == languageValue.FieldName)
                                allLanguageValues[languageValue.LanguageId.Value][i] = languageValue;
                        }
                    }
                }
                catch
                {
                    continue;
                }
            }

            return allLanguageValues;

        }

        public bool AddNewLanguageValues(Dictionary<int, List<LanguageValuesModel>> languageValuesDic, int entityId)
        {
            foreach (KeyValuePair<int, List<LanguageValuesModel>> kvp in languageValuesDic)
            {
                foreach (var item in kvp.Value)
                {
                    if (!string.IsNullOrWhiteSpace(item.Value))
                    {
                        if (item.EntityId == 0)
                            item.EntityId = entityId;

                        languageValuesRepo.Add(autoMapper.MapObject<LanguageValuesModel, LanguageValues>(item));
                    }
                }
            }

            return true;
        }

        public bool EditLanguageValues(Dictionary<int, List<LanguageValuesModel>> languageValuesDic, int entityId, string tableName)
        {
            var dbLanguageValues = languageValuesRepo.FindAll(x => x.EntityId == entityId && x.TableName == tableName);
            var del = languageValuesRepo.DeleteRange(dbLanguageValues.ToList());

            if (del == -1)
                return false;

            var languageValueList = new List<LanguageValuesModel>();

            //Entity Framework iteration içinde entry.State'i kaybediyor o yüzden dışarı çıkartıldı. Linq ile yemiyor.
            foreach (KeyValuePair<int, List<LanguageValuesModel>> kvp in languageValuesDic)
            {
                foreach (var item in kvp.Value)
                {
                    if (!string.IsNullOrWhiteSpace(item.Value))
                    {
                        languageValueList.Add(item);
                        item.EntityId = entityId;
                        item.Id = 0;
                    }
                }
            }

            if (languageValueList != null)
                languageValuesRepo.AddRange(autoMapper.MapCollection<LanguageValuesModel, LanguageValues>(languageValueList).ToList());

            return true;
        }

        public bool EditAttributeValuesLanguage(int languageId, List<LanguageValuesModel> languageValues, List<int> entityIds, string tableName)
        {
            var dbLanguageValues = languageValuesRepo.FindAll(x => entityIds.Contains(x.EntityId) && x.LanguageId == languageId && x.TableName == tableName);
            var del = languageValuesRepo.DeleteRange(dbLanguageValues.ToList());
            var languageValueList = new List<LanguageValuesModel>();

            if (del == -1)
                return false;

            foreach (var item in languageValues)
            {
                item.Id = 0;
                languageValueList.Add(item);
            }

            if (languageValueList != null)
                languageValuesRepo.AddRange(autoMapper.MapCollection<LanguageValuesModel, LanguageValues>(languageValueList).ToList());

            return true;

            //foreach (var dbLanguageValue in dbLanguageValues)
            //{
            //    dbLanguageValue.Value = languageValues.FirstOrDefault(x => x.EntityId == dbLanguageValue.EntityId 
            //        && x.LanguageId == dbLanguageValue.LanguageId
            //        && x.FieldName == dbLanguageValue.FieldName).Value;
            //}

            //return languageValuesRepo.UpdateRange(dbLanguageValues.ToList());
        }

        public bool isDefaultLanguage(int? id)
        {
            return languageRepo.Filter(a => a.Id == id).FirstOrDefault().IsDefault;
        }

        #endregion


    }
}
