using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductDb.Admin.Helpers.LocalizationHelpers;
using ProductDb.Mapping.BiggBrandDbModels;
using ProductDb.Services.BrandServices;
using ProductDb.Services.LanguageServices;
using ProductDb.Services.PermissionServices;

namespace ProductDb.Admin.Controllers
{
    [MiddlewareFilter(typeof(LocalizationPipeline))]
    [Authorize]
    [Route("{lang}/brand")]
    public class BrandController : BaseController
    {
        private readonly IBrandService brandService;

        public BrandController(ILanguageService languageService, IBrandService brandService,IUserRolePermissionService userRolePermissionService) : base(languageService, userRolePermissionService)
        {
            this.brandService = brandService;
        }

        [Route("list")]
        public IActionResult List()
        {
            var model = brandService.AllBrands();

            return View(model);
        }

        [Route("create")]
        public IActionResult Create()
        {
            var model = new BrandModel();

            return View(model);
        }

        [Route("create")]
        [HttpPost]
        public IActionResult Create(BrandModel model)
        {
            brandService.AddNewBrand(model);

            return RedirectToAction("list", new { lang = CurrentLanguage });
        }

        [Route("edit/{id}")]
        public IActionResult Edit(int id)
        {
            var model = brandService.BrandById(id);

            return View(model);
        }

        [Route("edit/{id}")]
        [HttpPost]
        public IActionResult Edit(BrandModel model)
        {
            brandService.EditBrand(model);

            return RedirectToAction("list", new { lang = CurrentLanguage });
        }

        [Route("setstate/{id}")]
        public IActionResult State(int id)
        {
            brandService.SetState(id);

            return RedirectToAction("list", new { lang = CurrentLanguage });
        }
    }
}