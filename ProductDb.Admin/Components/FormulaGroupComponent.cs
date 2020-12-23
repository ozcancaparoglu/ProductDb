using Microsoft.AspNetCore.Mvc;
using ProductDb.Services.CalculationServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductDb.Admin.Components
{
    public class FormulaGroupViewComponent : ViewComponent
    {
        private readonly ICalculationService calculatorService;

        public FormulaGroupViewComponent(ICalculationService calculatorService)
        {
            this.calculatorService = calculatorService;
        }

        public IViewComponentResult Invoke(int formulaGroupId)
        {
            var model = calculatorService.GetFormulasWithGroupId(formulaGroupId);

            ViewBag.IsRound = false;

            if (model.Any(x => x.FormulaStr.Contains("ROUND")))
                ViewBag.IsRound = true;

            return View("_FormulaGroupView", model);
        }
    }
}
