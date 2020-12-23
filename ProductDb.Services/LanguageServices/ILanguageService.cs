using ProductDb.Mapping.BiggBrandDbModels;
using System.Collections.Generic;

namespace ProductDb.Services.LanguageServices
{
    public interface ILanguageService
    {
        ICollection<LanguageModel> AllLanguages();
        ICollection<LanguageModel> AllLanguagesWithDefault();
        LanguageModel AddNewLanguage(LanguageModel languageModel);
        int CreateLanguages();
        ICollection<LanguageValuesModel> LanguageValuesByEntityIdAndTableName(int id, string tableName);
        LanguageValuesModel LanguageValueByEntityIdAndTableName(int id, string tableName, int languageId);
        LanguageModel LanguageById(int id);
        LanguageModel EditLanguage(LanguageModel model);
        void SetState(int objectId);
        LanguageModel GetDefaultLanguage();
        bool isDefaultLanguage(int? id);
        #region LanguageValues

        Dictionary<int, List<LanguageValuesModel>> PrepareLanguageValues(ICollection<string> fieldNameList, string tableName);
        Dictionary<int, List<LanguageValuesModel>> PrepareLanguageValuesForEdit(ICollection<string> fieldNameList, string tableName, int id);
        Dictionary<int, List<LanguageValuesModel>> PrepareLanguageValuesForEditMultipleIds(ICollection<string> fieldNameList, string tableName, List<int> ids);
        bool AddNewLanguageValues(Dictionary<int, List<LanguageValuesModel>> languageValuesDic, int entityId);
        bool EditLanguageValues(Dictionary<int, List<LanguageValuesModel>> languageValuesDic, int entityId, string tableName);
        //int EditLanguageValuesMultipleForEntityIds(int languageId, List<LanguageValuesModel> languageValues, List<int> entityIds, string tableName);
        bool EditAttributeValuesLanguage(int languageId, List<LanguageValuesModel> languageValues, List<int> entityIds, string tableName);

        #endregion
    }
}