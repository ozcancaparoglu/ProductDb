using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductDb.Admin.Helpers.LocalizationHelpers;
using ProductDb.Mapping.BiggBrandDbModels;
using ProductDb.Services.LanguageServices;
using ProductDb.Services.PermissionServices;
using ProductDb.Services.SupplierServices;

namespace ProductDb.Admin.Controllers
{
    [MiddlewareFilter(typeof(LocalizationPipeline))]
    [Authorize]
    [Route("{lang}/supplier")]
    public class SupplierController : BaseController
    {
        private readonly ISupplierService supplierService;

        public SupplierController(ILanguageService languageService, ISupplierService supplierService,
            IUserRolePermissionService userRolePermissionService) : base (languageService, userRolePermissionService)
        {
            this.supplierService = supplierService;
        }

        [Route("list")]
        public IActionResult List()
        {
            var model = supplierService.AllSuppliers();

            return View(model);
        }

        [Route("create")]
        public IActionResult Create(bool validation = true)
        {
            ViewBag.Validation = validation;
            var model = new SupplierModel();

            return View(model);
        }

        [Route("create")]
        [HttpPost]
        public IActionResult Create(SupplierModel model)
        {
            var entity = supplierService.AddNewSupplier(model);

            if (entity == null)
                return RedirectToAction("Create", new { lang = CurrentLanguage, validation = false });

            return RedirectToAction("list", new { lang = CurrentLanguage });
        }

        [Route("edit/{id}")]
        public IActionResult Edit(int id, bool validation = true)
        {
            ViewBag.Validation = validation;

            var model = supplierService.SupplierById(id);

            return View(model);
        }

        [Route("edit/{id}")]
        [HttpPost]
        public IActionResult Edit(SupplierModel model)
        {
            var entity = supplierService.EditSupplier(model);

            if (entity == null)
                return RedirectToAction("Edit", new { lang = CurrentLanguage, id = model.Id, validation = false });

            return RedirectToAction("list", new { lang = CurrentLanguage });
        }

        [Authorize]
        [Route("setstate/{id}")]
        public IActionResult State(int id)
        {
            supplierService.SetState(id);

            return RedirectToAction("list", new { lang = CurrentLanguage });
        }
    }
}