using ProductDb.Mapping.BiggBrandDbModels;
using System.Collections.Generic;

namespace ProductDb.Admin.PageModels.Language
{
    public class LanguageViewModel
    {
        public List<LanguageModel> Languages { get; set; }
        public Dictionary<int, List<LanguageValuesModel>> LanguageValues { get; set; }

    }
}
