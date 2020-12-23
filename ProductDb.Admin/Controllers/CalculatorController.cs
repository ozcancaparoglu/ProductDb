using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductDb.Admin.Helpers.LocalizationHelpers;
using ProductDb.Admin.PageModels.Calculator;
using ProductDb.Mapping.BiggBrandDbModels;
using ProductDb.Services.CalculationServices;
using ProductDb.Services.CurrencyServices;
using ProductDb.Services.LanguageServices;
using ProductDb.Services.PermissionServices;
using System;
using System.Linq;

namespace ProductDb.Admin.Controllers
{
    [Authorize]
    [MiddlewareFilter(typeof(LocalizationPipeline))]
    [Route("{lang}/calculator")]
    public class CalculatorController : BaseController
    {
        private readonly ICalculationService calculatorService;
        private readonly ICurrencyService currencyService;

        public CalculatorController(ILanguageService languageService, IUserRolePermissionService userRolePermissionService,
            ICalculationService calculatorService, ICurrencyService currencyService) : base(languageService, userRolePermissionService)
        {
            this.calculatorService = calculatorService;
            this.currencyService = currencyService;
        }

        [HttpPost]
        [Route("calculate-store/{storeId}")]
        public JsonResult CalculateStorePrices(int storeId)
        {
            if (calculatorService.SellingPrice(storeId, out string exception))
                return Json(new { success = true, result = "success", id = storeId });

            return Json(new { success = false, result = "error", message = exception });
        }

        [Route("add-formula/{formulaGroupId}")]
        public IActionResult AddFormula(int formulaGroupId)
        {
            var model = new CalculatorViewModel
            {
                FormulaGroup = calculatorService.GetFormulaGroupById(formulaGroupId),
                MbCurrencies = currencyService.GetMbActiveCurrencies().ToList(),
                Currencies = currencyService.AllActiveCurrencies().ToList()
            };

            return View(model);
        }

        [HttpPost]
        [Route("insert-formula")]
        public JsonResult InsertFormula(int formulaGroupId, string formulaStr, string name)
        {
            try
            {
                var order = calculatorService.GetLastOrderFormulaByGroupId(formulaGroupId);

                var res = calculatorService.AddNewFormula(new FormulaModel
                {
                    FormulaGroupId = formulaGroupId,
                    FormulaStr = formulaStr,
                    Order = order,
                    Name = name,
                    Result = $"res{order}"
                });

                return Json(new { result = "success", id = formulaGroupId });
            }
            catch (Exception ex)
            {
                return Json(new { result = "failed", ex.Message });
            }
        }

        [Route("formula-component/{formulaGroupId}")]
        public IActionResult ProductGroupFormulas(int formulaGroupId)
        {
            var result = ViewComponent("FormulaGroup", new { formulaGroupId });
            return result;
        }

        [Route("list")]
        public IActionResult List()
        {
            var model = calculatorService.AllFormulaGroups().ToList();
            return View(model);
        }

        [Route("create")]
        public IActionResult Create()
        {
            var model = new FormulaGroupModel();

            return View(model);
        }

        [Route("create")]
        [HttpPost]
        public IActionResult Create(FormulaGroupModel model)
        {
            calculatorService.AddNewFormulaGroup(model);

            return RedirectToAction("list", new { lang = CurrentLanguage });
        }

        [Route("edit/{id}")]
        public IActionResult Edit(int id)
        {
            var model = calculatorService.GetFormulaGroupById(id);

            return View(model);
        }

        [Route("edit/{id}")]
        [HttpPost]
        public IActionResult Edit(FormulaGroupModel model)
        {
            calculatorService.EditFormulaGroup(model);

            return RedirectToAction("list", new { lang = CurrentLanguage });
        }

        [Route("delete/{id}")]
        public IActionResult Delete(int id)
        {
            calculatorService.DeleteFormulaGroupById(id);

            return RedirectToAction("list", new { lang = CurrentLanguage });
        }

        [Route("delete-formula/{id}")]
        public IActionResult DeleteFormula(int id)
        {
            var formulaGroupId = calculatorService.DeleteFormulaById(id);

            if(formulaGroupId == -1)
                return RedirectToAction("list", new { lang = CurrentLanguage });

            return RedirectToAction("AddFormula", new { formulaGroupId, lang = CurrentLanguage });
        }



    }
}