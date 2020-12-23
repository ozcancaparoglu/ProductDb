using Microsoft.AspNetCore.Mvc;
using ProductDb.Admin.PageModels.Language;
using ProductDb.Services.LanguageServices;
using System.Collections.Generic;
using System.Linq;

namespace ProductDb.Admin.Components
{
    public class LanguageAttributeViewComponent : ViewComponent
    {
        private readonly ILanguageService languageService;

        public LanguageAttributeViewComponent(ILanguageService languageService)
        {
            this.languageService = languageService;
        }

        public IViewComponentResult Invoke(List<string> fieldNames, string tableName, List<int> ids)
        {
            var model = new LanguageViewAttributeModel
            {
                AttributeLanguages = languageService.AllLanguages().ToList(),
                AttributeLanguageValues = ids != null && ids.Count > 0 ? languageService.PrepareLanguageValuesForEditMultipleIds(fieldNames, tableName, ids) : languageService.PrepareLanguageValues(fieldNames, tableName)
            };

            return View("_LanguageAttributesValues", model);
        }
    }
}
