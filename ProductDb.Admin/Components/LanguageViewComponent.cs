using Microsoft.AspNetCore.Mvc;
using ProductDb.Admin.PageModels.Language;
using ProductDb.Services.LanguageServices;
using System.Collections.Generic;
using System.Linq;

namespace ProductDb.Admin.Components
{
    public class LanguageViewComponent : ViewComponent
    {
        private readonly ILanguageService languageService;

        public LanguageViewComponent(ILanguageService languageService)
        {
            this.languageService = languageService;
        }

        public IViewComponentResult Invoke(List<string> fieldNames, string tableName, int? id)
        {
            var model = new LanguageViewModel
            {
                Languages = languageService.AllLanguages().ToList(),
                LanguageValues = id.HasValue ? languageService.PrepareLanguageValuesForEdit(fieldNames, tableName, id.Value) : languageService.PrepareLanguageValues(fieldNames, tableName)
            };

            return View("_LanguageValues", model);
        }
    }
}
