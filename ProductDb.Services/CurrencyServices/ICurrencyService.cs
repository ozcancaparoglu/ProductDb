using ProductDb.Data.BiggBrandsDb;
using ProductDb.Mapping.BiggBrandDbModels;
using System.Collections.Generic;

namespace ProductDb.Services.CurrencyServices
{
    public interface ICurrencyService
    {
        ICollection<CurrencyModel> GetMbActiveCurrencies();
        ICollection<CurrencyModel> AllActiveCurrencies();
        ICollection<CurrencyModel> AllCurrencies();
        CurrencyModel CurrencyById(int id);
        CurrencyModel CurrencyByAbbrevation(string abbrevation);
        CurrencyModel AddNewCurrency(CurrencyModel model);
        CurrencyModel EditCurrency(CurrencyModel model);
        void SetState(int objectId);
        decimal CurrencyLiveValue(string code);
        void AllCurrenciesUpdateToLive();
    }
}
