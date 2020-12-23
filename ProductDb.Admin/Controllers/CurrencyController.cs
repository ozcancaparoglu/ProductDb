using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductDb.Admin.Helpers.LocalizationHelpers;
using ProductDb.Mapping.BiggBrandDbModels;
using ProductDb.Services.CurrencyServices;
using ProductDb.Services.LanguageServices;
using ProductDb.Services.PermissionServices;

namespace ProductDb.Admin.Controllers
{
    [MiddlewareFilter(typeof(LocalizationPipeline))]
    [Authorize]
    [Route("{lang}/currency")]
    public class CurrencyController : BaseController
    {
        private readonly ICurrencyService currencyService;

        public CurrencyController(ILanguageService languageService, 
            ICurrencyService currencyService, IUserRolePermissionService userRolePermissionService ) : base(languageService, userRolePermissionService)
        {
            this.currencyService = currencyService;
        }

        [Route("list")]
        public IActionResult List()
        {
            var model = currencyService.AllCurrencies();

            return View(model);
        }
        // for json result
        [Route("AllList")]
        public IActionResult AllList()
        {
            var model = currencyService.AllCurrencies();
            return Json(model);
        }


        [Route("create")]
        public IActionResult Create()
        {
            var model = new CurrencyModel();

            return View(model);
        }

        [Route("create")]
        [HttpPost]
        public IActionResult Create(CurrencyModel model)
        {
            currencyService.AddNewCurrency(model);

            return RedirectToAction("list", new { lang = CurrentLanguage });
        }

        [Route("edit/{id}")]
        public IActionResult Edit(int id)
        {
            var model = currencyService.CurrencyById(id);

            return View(model);
        }

        [Route("edit/{id}")]
        [HttpPost]
        public IActionResult Edit(CurrencyModel model)
        {
            currencyService.EditCurrency(model);

            return RedirectToAction("list", new { lang = CurrentLanguage });
        }

        [Route("setstate/{id}")]
        public IActionResult State(int id)
        {
            currencyService.SetState(id);

            return RedirectToAction("list", new { lang = CurrentLanguage });
        }

        [Route("updateToLive/{abbrevation}")]
        public IActionResult UpdateToLiveCurrencyRate(string abbrevation)
        {
            if(!string.IsNullOrWhiteSpace(abbrevation))
            {
                var currency = currencyService.CurrencyByAbbrevation(abbrevation);
                currencyService.EditCurrency(currency);
            }

            return RedirectToAction("list", new { lang = CurrentLanguage });
        }
    }
}