using ProductDb.Common.Enums;
using ProductDb.Data.BiggBrandsDb;
using ProductDb.Mapping.AutoMapperConfigurations;
using ProductDb.Mapping.BiggBrandDbModels;
using ProductDb.Repositories.BaseRepositories;
using ProductDb.Repositories.UnitOfWorks;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Xml;

namespace ProductDb.Services.CurrencyServices
{
    public class CurrencyService : ICurrencyService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IAutoMapperConfiguration autoMapper;

        private readonly IGenericRepository<Currency> currencyRepo;

        private readonly Expression<Func<Currency, bool>> defaultCurrencyFilter = null;
        private static readonly List<string> MBXmlCurrencies = new List<string> { "USD", "AUD", "DKK", "EUR", "GBP", "CHF", "SEK", "CAD", "KWD", "NOK", "SAR", "JPY", "BGN", "RON", "RUB", "IRR", "CNY", "PKR", "QAR", "XDR" };

        public CurrencyService(IUnitOfWork unitOfWork, IAutoMapperConfiguration autoMapper)
        {
            this.unitOfWork = unitOfWork;
            this.autoMapper = autoMapper;

            currencyRepo = this.unitOfWork.Repository<Currency>();

            defaultCurrencyFilter = entity => entity.State == (int)State.Active;
        }

        public CurrencyModel AddNewCurrency(CurrencyModel model)
        {
            var entity = autoMapper.MapObject<CurrencyModel, Currency>(model);

            if (currencyRepo.Exist(x => x.Name.ToLowerInvariant() == entity.Name.ToLowerInvariant())) 
                return null;

            entity.LiveValue = CurrencyLiveValue(entity.Abbrevation);

            var savedEntity = currencyRepo.Add(entity);

            return autoMapper.MapObject<Currency, CurrencyModel>(savedEntity);
        }

        public ICollection<CurrencyModel> GetMbActiveCurrencies()
        {
            return AllActiveCurrencies().Where(x => MBXmlCurrencies.Contains(x.Abbrevation)).ToList();

        }

        public ICollection<CurrencyModel> AllActiveCurrencies()
        {
            return autoMapper.MapCollection<Currency, CurrencyModel>(currencyRepo.FindAll(defaultCurrencyFilter)).ToList();
        }

        public ICollection<CurrencyModel> AllCurrencies()
        {
            var currencies = autoMapper.MapCollection<Currency, CurrencyModel>(currencyRepo.GetAll());
            foreach (var item in currencies)
            {
                if (!MBXmlCurrencies.Contains(item.Abbrevation))
                    item.IsInMB = false;
                else
                    item.IsInMB = true;
            }

            return currencies.ToList();
        }

        public CurrencyModel CurrencyByAbbrevation(string abbrevation)
        {
            return autoMapper.MapObject<Currency, CurrencyModel>(currencyRepo.FindBy(x=>x.Abbrevation == abbrevation).FirstOrDefault());
        }

        public CurrencyModel CurrencyById(int id)
        {
            return autoMapper.MapObject<Currency, CurrencyModel>(currencyRepo.GetById(id));
        }

        public CurrencyModel EditCurrency(CurrencyModel model)
        {
            var entity = autoMapper.MapObject<CurrencyModel, Currency>(model);

            entity.LiveValue = CurrencyLiveValue(entity.Abbrevation);

            var updatedEntity = currencyRepo.Update(entity);

            return autoMapper.MapObject<Currency, CurrencyModel>(updatedEntity);
        }

        public void SetState(int objectId)
        {
            var entity = currencyRepo.GetById(objectId);

            if (entity.State == (int)State.Active)
                entity.State = (int)State.Passive;

            else
                entity.State = (int)State.Active;

            currencyRepo.Update(entity);
        }

        public decimal CurrencyLiveValue(string code)
        {
            string exchangeRate = "http://www.tcmb.gov.tr/kurlar/today.xml";

            //load XML document
            var document = new XmlDocument();
            document.Load(exchangeRate);
            
            XmlNodeList xmlNodeList = document.SelectNodes("/Tarih_Date/Currency");
            foreach (XmlNode currency in xmlNodeList)
            {
                if (currency.Attributes["CurrencyCode"].Value.ToLowerInvariant() == code.ToLowerInvariant())
                    return (currency["ForexSelling"].InnerText != "") ? Convert.ToDecimal(currency["ForexSelling"].InnerText, new CultureInfo("en-EN")) : 1.0M;
            }

            return 1.0M;

        }

        public void AllCurrenciesUpdateToLive()
        {
            var currencies = currencyRepo.GetAll();
            string exchangeRate = "http://www.tcmb.gov.tr/kurlar/today.xml";

            //load XML document
            var document = new XmlDocument();
            document.Load(exchangeRate);

            XmlNodeList xmlNodeList = document.SelectNodes("/Tarih_Date/Currency");
            foreach (XmlNode xmlNode in xmlNodeList)
            {
                var currency = currencies.FirstOrDefault(x => x.Abbrevation.ToLowerInvariant().Contains(xmlNode.Attributes["CurrencyCode"].Value.ToLowerInvariant()));
                if(currency != null)
                    currency.LiveValue = (xmlNode["ForexSelling"].InnerText != "") ? Convert.ToDecimal(xmlNode["ForexSelling"].InnerText, new CultureInfo("en-EN")) : 1.0M;
            }
        }
    }
}
