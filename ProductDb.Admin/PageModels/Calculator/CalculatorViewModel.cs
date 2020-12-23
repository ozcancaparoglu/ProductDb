using ProductDb.Mapping.BiggBrandDbModels;
using System.Collections.Generic;

namespace ProductDb.Admin.PageModels.Calculator
{
    public class CalculatorViewModel
    {
        public FormulaGroupModel FormulaGroup { get; set; }
        public List<CurrencyModel> MbCurrencies { get; set; }
        public List<CurrencyModel> Currencies { get; set; }
    }
}
