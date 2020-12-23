using ProductDb.Mapping.BiggBrandDbModels;
using System.Collections.Generic;

namespace ProductDb.Admin.PageModels.Language
{
    public class LanguageViewAttributeModel
    {
        public List<LanguageModel> AttributeLanguages { get; set; }
        public Dictionary<int, List<LanguageValuesModel>> AttributeLanguageValues { get; set; }
    }
}
