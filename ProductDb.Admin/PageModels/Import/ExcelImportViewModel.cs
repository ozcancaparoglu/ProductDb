using Microsoft.AspNetCore.Http;
using ProductDb.Admin.Resources;
using ProductDb.Mapping.BiggBrandDbModels;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProductDb.Admin.PageModels.Import
{
    public class ExcelImportViewModel
    {
        [Display(Name = "SelectFile", ResourceType = typeof(CommonResource))]
        [Required(ErrorMessage = "FileRequired")]
        public IFormFile file { get; set; }

        public string message { get; set; }

        [Display(Name = "Language")]
        [Required(ErrorMessage = "LanguageRequired")]
        public int? languageId { get; set; }
        public List<LanguageModel> languages { get; set; }
        public List<CurrencyModel> Currencies { get; set; }
    }
}
