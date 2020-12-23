using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using ProductDb.Admin.Helpers.LocalizationHelpers;
using ProductDb.Mapping.BiggBrandDbModels;
using ProductDb.Services.LanguageServices;
using ProductDb.Services.PermissionServices;

namespace ProductDb.Admin.Controllers
{
    [MiddlewareFilter(typeof(LocalizationPipeline))]
    [Authorize]
    [Route("{lang}/language")]
    public class LanguageController : BaseController
    {
        private readonly IConfiguration configuration;
        public LanguageController(ILanguageService languageService, IConfiguration configuration, IUserRolePermissionService userRolePermissionService) : base(languageService, userRolePermissionService)
        {
            this.configuration = configuration;
        }

        [Route("list")]
        public IActionResult List()
        {
            var model = languageService.AllLanguagesWithDefault();

            return View(model);
        }

        [Route("create")]
        public IActionResult Create(bool validation = false)
        {
            ViewBag.Validation = validation;
            var model = new LanguageModel();

            return View(model);
        }

        [Route("create")]
        [HttpPost]
        public IActionResult Create(LanguageModel model)
        {
            #region validations

            model.Name = model.Name.Trim();

            if (model.Abbrevation.Length >= 3)
            {
                model.Abbrevation = model.Abbrevation.Substring(0, 2);
            }

            #endregion

            model.LogoPath = configuration["LanguageLogoPath"] + model.Abbrevation + ".png";
            languageService.AddNewLanguage(model);

            return RedirectToAction("list");
        }

        [Authorize(Roles = "Administrator")]
        [Route("edit/{id}")]
        public IActionResult Edit(int id, bool validation = true)
        {
            ViewBag.Validation = validation;
            var model = languageService.LanguageById(id);

            return View(model);
        }

        [Authorize("")]
        [Route("edit/{id}")]
        [HttpPost]
        public IActionResult Edit(LanguageModel model)
        {
            if (model.Abbrevation.Length >= 3)
                model.Abbrevation = model.Abbrevation.Substring(0, 2);

            model.LogoPath = configuration["LanguageLogoPath"] + model.Abbrevation + ".png";

            var entity = languageService.EditLanguage(model);

            if (entity == null)
                return RedirectToAction("update", new { lang = CurrentLanguage, id = model.Id, validation = false });

            return RedirectToAction("list", new { lang = CurrentLanguage });
        }

        [Authorize]
        [Route("setstate/{id}")]
        public IActionResult State(int id)
        {
            languageService.SetState(id);

            return RedirectToAction("list", new { lang = CurrentLanguage });
        }
    }
}