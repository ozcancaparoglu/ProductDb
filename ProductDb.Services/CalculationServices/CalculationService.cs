using Jace;
using Microsoft.EntityFrameworkCore;
using ProductDb.Common.Enums;
using ProductDb.Data.BiggBrandsDb;
using ProductDb.Mapping.AutoMapperConfigurations;
using ProductDb.Mapping.BiggBrandDbModels;
using ProductDb.Repositories.BaseRepositories;
using ProductDb.Repositories.UnitOfWorks;
using ProductDb.Services.BrandServices;
using ProductDb.Services.CalculationServices.HelperModels;
using ProductDb.Services.CategoryServices;
using ProductDb.Services.CurrencyServices;
using ProductDb.Services.StoreServices;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ProductDb.Services.CalculationServices
{
    public class CalculationService : ICalculationService
    {
        private readonly IBrandService brandService;
        private readonly IUnitOfWork unitOfWork;
        private readonly IAutoMapperConfiguration autoMapper;

        private readonly ICurrencyService currencyService;
        private readonly IStoreService storeService;
        private readonly ICategoryService categoryService;
        private readonly IGenericRepository<Formula> formulaRepo;
        private readonly IGenericRepository<FormulaGroup> formulaGroupRepo;
        private readonly IGenericRepository<Margin> marginRepo;
        private readonly IGenericRepository<StoreProductMapping> storeProductRepo;
        private readonly IGenericRepository<Product> productRepo;
        private readonly IGenericRepository<MarginType> marginTypeRepo;
        private readonly IGenericRepository<Cargo> cargoRepo;
        private readonly IGenericRepository<CargoType> cargoTypeRepo;
        private readonly IGenericRepository<Transportation> transportationRepo;
        private readonly IGenericRepository<TransportationType> transportationTypeRepo;
        private readonly IGenericRepository<Store> storeRepo;

        public CalculationService(IUnitOfWork unitOfWork, IAutoMapperConfiguration autoMapper, ICurrencyService currencyService, IStoreService storeService, ICategoryService categoryService, IBrandService brandService)
        {
            this.brandService = brandService;
            this.unitOfWork = unitOfWork;
            this.autoMapper = autoMapper;

            this.currencyService = currencyService;
            this.storeService = storeService;
            this.categoryService = categoryService;

            formulaRepo = this.unitOfWork.Repository<Formula>();
            formulaGroupRepo = this.unitOfWork.Repository<FormulaGroup>();
            marginRepo = this.unitOfWork.Repository<Margin>();
            storeProductRepo = this.unitOfWork.Repository<StoreProductMapping>();
            productRepo = this.unitOfWork.Repository<Product>();
            marginTypeRepo = this.unitOfWork.Repository<MarginType>();
            cargoRepo = this.unitOfWork.Repository<Cargo>();
            cargoTypeRepo = this.unitOfWork.Repository<CargoType>();
            transportationRepo = this.unitOfWork.Repository<Transportation>();
            transportationTypeRepo = this.unitOfWork.Repository<TransportationType>();
            storeRepo = this.unitOfWork.Repository<Store>();
        }

        #region Formula Group

        public FormulaGroupModel GetFormulaGroupById(int? id)
        {
            if (!id.HasValue)
                return null;

            return autoMapper.MapObject<FormulaGroup, FormulaGroupModel>(formulaGroupRepo.GetById(id.Value));
        }

        public FormulaGroupModel AddNewFormulaGroup(FormulaGroupModel model)
        {
            var entity = autoMapper.MapObject<FormulaGroupModel, FormulaGroup>(model);

            var savedEntity = formulaGroupRepo.Add(entity);

            return autoMapper.MapObject<FormulaGroup, FormulaGroupModel>(savedEntity);
        }

        public FormulaGroupModel EditFormulaGroup(FormulaGroupModel model)
        {
            var entity = autoMapper.MapObject<FormulaGroupModel, FormulaGroup>(model);

            var updatedEntity = formulaGroupRepo.Update(entity);

            return autoMapper.MapObject<FormulaGroup, FormulaGroupModel>(updatedEntity);
        }

        public int DeleteFormulaGroupById(int id)
        {
            var entity = formulaGroupRepo.GetById(id);

            try
            {
                var formulasToDelete = formulaRepo.FindAll(x => x.FormulaGroupId == entity.Id).ToList();

                formulaRepo.DeleteRange(formulasToDelete);

                return formulaGroupRepo.Delete(entity);

            }
            catch
            {
                return -1;
            }
        }

        public ICollection<FormulaGroupModel> AllFormulaGroups()
        {
            return autoMapper.MapCollection<FormulaGroup, FormulaGroupModel>(formulaGroupRepo.Filter(x => x.State == (int)State.Active, p => p.OrderBy(o => o.Name))).ToList();
        }

        #endregion

        #region Formula

        private void ArrangeOrders(List<FormulaModel> formulaModels)
        {
            var order = 1;
            var orderedList = formulaModels.OrderBy(x => x.Order);

            foreach (var formulaOrder in orderedList)
            {
                formulaOrder.Order = order;
                EditFormula(formulaOrder);
                order++;
            }
        }

        public int GetLastOrderFormulaByGroupId(int formulaGroupId)
        {
            var lastOrder = formulaRepo.Filter(x => x.FormulaGroupId == formulaGroupId, p => p.OrderByDescending(o => o.Order)).FirstOrDefault();

            return lastOrder != null ? lastOrder.Order + 1 : 1;
        }

        public ICollection<FormulaModel> GetFormulasWithGroupId(int formulaGroupId)
        {
            var entities = formulaRepo.Filter(x => x.FormulaGroupId == formulaGroupId, p => p.OrderBy(o => o.Order));

            return autoMapper.MapCollection<Formula, FormulaModel>(entities).ToList();
        }

        public FormulaModel AddNewFormula(FormulaModel model)
        {
            var entity = autoMapper.MapObject<FormulaModel, Formula>(model);

            var savedEntity = formulaRepo.Add(entity);

            return autoMapper.MapObject<Formula, FormulaModel>(savedEntity);
        }

        public FormulaModel EditFormula(FormulaModel model)
        {
            var entity = autoMapper.MapObject<FormulaModel, Formula>(model);

            var updatedEntity = formulaRepo.Update(entity);

            return autoMapper.MapObject<Formula, FormulaModel>(updatedEntity);
        }

        public int DeleteFormulaById(int id)
        {
            try
            {
                var entity = formulaRepo.GetById(id);
                var formulaGroupId = entity.FormulaGroupId.Value;

                formulaRepo.Delete(entity);

                var formulaGroupFormulas = formulaRepo.FindAll(x => x.FormulaGroupId == formulaGroupId);
                ArrangeOrders(autoMapper.MapCollection<Formula, FormulaModel>(formulaGroupFormulas).ToList());

                return formulaGroupId;
            }
            catch
            {
                return -1;
            }
        }

        #endregion

        #region Cargo
        private void ArrangeCargoIsLastDesi(int id)
        {
            var cargoPrices = cargoRepo.Filter(x => x.IsLastDesi && x.Id != id).ToList();
            if (cargoPrices.Count > 0)
            {
                cargoPrices.ForEach(x => x.IsLastDesi = false);
                cargoRepo.BulkUpdate(cargoPrices);
            }
        }

        public ICollection<CargoTypeModel> AllCargoTypes()
        {
            return autoMapper.MapCollection<CargoType, CargoTypeModel>(cargoTypeRepo.Filter(x => x.State == (int)State.Active)).ToList();
        }

        public ICollection<CargoModel> CargoByStoreId(int storeId)
        {
            return autoMapper.MapCollection<Cargo, CargoModel>(cargoRepo.Filter(x => x.State == (int)State.Active && x.StoreId == storeId, o => o.OrderByDescending(p => p.Id))).ToList();
        }

        public CargoModel InsertCargo(CargoModel cargo)
        {
            var entity = cargoRepo.Add(autoMapper.MapObject<CargoModel, Cargo>(cargo));
            if (entity.IsLastDesi)
            {
                ArrangeCargoIsLastDesi(entity.Id);
            }
            return autoMapper.MapObject<Cargo, CargoModel>(entity);
        }

        public CargoModel UpdateCargo(CargoModel cargo)
        {
            if (cargo.IsLastDesi)
                ArrangeCargoIsLastDesi(cargo.Id);

            var data = cargoRepo.Filter(x => x.Id == cargo.Id).FirstOrDefault();
            if (data != null)
            {
                return autoMapper.MapObject<Cargo, CargoModel>(cargoRepo.Update(autoMapper.MapObject<CargoModel, Cargo>(cargo)));
            }
            return InsertCargo(cargo);
        }

        public void DeleteCargo(int id)
        {
            cargoRepo.Delete(cargoRepo.GetById(id));
        }

        #endregion

        #region Transportation

        public ICollection<TransportationTypeModel> AllTransportationTypes()
        {
            return autoMapper.MapCollection<TransportationType, TransportationTypeModel>(transportationTypeRepo.GetAll()).ToList();
        }

        public ICollection<TransportationModel> AllActiveTransportations(int storeId)
        {
            return autoMapper.MapCollection<Transportation, TransportationModel>(transportationRepo.FindAll(x => x.StoreId == storeId && x.State == (int)State.Active)).ToList();
        }

        public TransportationModel AddNewTransportation(TransportationModel model)
        {
            var entity = autoMapper.MapObject<TransportationModel, Transportation>(model);

            var savedEntity = transportationRepo.Add(entity);

            return autoMapper.MapObject<Transportation, TransportationModel>(savedEntity);
        }

        public TransportationModel EditTransportation(TransportationModel model)
        {
            if (model.Id == 0)
                return AddNewTransportation(model);

            var entity = autoMapper.MapObject<TransportationModel, Transportation>(model);

            var savedEntity = transportationRepo.Update(entity);

            return autoMapper.MapObject<Transportation, TransportationModel>(savedEntity);
        }

        public void DeleteTransportation(int id)
        {
            transportationRepo.Delete(transportationRepo.GetById(id));
        }

        public ICollection<TransportationModel> StoreTransportationProducts(int storeId)
        {
            var currencies = currencyService.AllActiveCurrencies();
            var transTypes = AllTransportationTypes();
            List<int> prodIDs = new List<int> { 1, 3, 5 };

            var list = storeProductRepo.Table().Where(x => x.StoreId == storeId).Include(x => x.Product).Select(x => new ProductModel
            {
                Id = x.Product.Id,
                Sku = x.Product.Sku,
                Name = x.Product.Sku
            }).ToList();

            var productTransportations = transportationRepo.Table().Where(x => list.Any(k => k.Id == x.EntityId) && x.StoreId == storeId && prodIDs.Any(m => m == x.TransportationTypeId)).ToList();

            var _productTransportations = (from s in list
                                           join b in productTransportations on s.Id equals b.EntityId into sb
                                           from b in sb.DefaultIfEmpty()
                                           select new TransportationModel
                                           {
                                               Id = b != null ? b.Id : 0,
                                               Name = s.Name,
                                               EntityId = s.Id,
                                               StoreId = storeId,
                                               TransportationType = b != null ? transTypes.FirstOrDefault(x => x.Id == b.TransportationTypeId) : transTypes.FirstOrDefault(x => x.Id == 1),
                                               TransportationTypeId = b != null ? b.TransportationTypeId : 0,
                                               Value = b != null ? b.Value.Value : 0,
                                               CurrencyId = b != null ? b.CurrencyId : 1,
                                               Currency = b != null ? currencies.FirstOrDefault(x => x.Id == b.CurrencyId) : currencies.FirstOrDefault(x => x.Id == 1)
                                           }).ToList();

            return _productTransportations;
        }

        public ICollection<TransportationModel> StoreTransportationBrands(int storeId)
        {
            var list = brandService.AllActiveBrands();
            var currencies = currencyService.AllActiveCurrencies();
            var transTypes = AllTransportationTypes();
            List<int> prodIDs = new List<int> { 2, 4, 6 };

            var brandTransportations = transportationRepo.Table().Where(x => list.Any(k => k.Id == x.EntityId) && x.StoreId == storeId && prodIDs.Any(m => m == x.TransportationTypeId)).ToList();

            var _brandTransportations = (from s in list
                                         join b in brandTransportations on s.Id equals b.EntityId into sb
                                         from b in sb.DefaultIfEmpty()
                                         select new TransportationModel
                                         {
                                             Id = b != null ? b.Id : 0,
                                             Name = s.Name,
                                             EntityId = s.Id,
                                             StoreId = storeId,
                                             TransportationType = b != null ? transTypes.FirstOrDefault(x => x.Id == b.TransportationTypeId) : transTypes.FirstOrDefault(x => x.Id == 2),
                                             TransportationTypeId = b != null ? b.TransportationTypeId : 0,
                                             Value = b != null ? b.Value.Value : 0,
                                             CurrencyId = b != null ? b.CurrencyId : 1,
                                             Currency = b != null ? currencies.FirstOrDefault(x => x.Id == b.CurrencyId) : currencies.FirstOrDefault(x => x.Id == 1)
                                         }).ToList();

            return _brandTransportations;
        }

        public ICollection<TransportationTypeModel> StoreTransportationTypeEnum(List<int> IDs)
        {
            return autoMapper.MapCollection<TransportationType, TransportationTypeModel>(transportationTypeRepo.Table()
                .Where(x => IDs.Any(m => x.Id == m))).ToList();
        }

        #endregion

        #region Calculation

        public bool SellingPrice(int storeId, out string exceptionMessage)
        {
            CalculationEngine engine = new CalculationEngine();

            engine.AddFunction("Round99", (a) => Math.Round(a, 0) - 0.01);
            engine.AddFunction("Round90", (a) => Math.Round(a, 0) - 0.01 - 0.09);
            engine.AddFunction("RoundToUpper", (a) => Math.Ceiling(a));

            currencyService.AllCurrenciesUpdateToLive();
            var store = storeService.StoreWithCurrency(storeId);
            var formulaGroupId = store.FormulaGroupId.Value;
            var currencies = currencyService.AllActiveCurrencies();
            var margins = marginRepo.Filter(x => x.StoreId == storeId, null, "MarginType");
            var cargo = cargoRepo.Filter(x => x.StoreId == storeId, o => o.OrderByDescending(p => p.MaxDesi));
            var transportations = transportationRepo.Filter(x => x.StoreId == storeId, null, "TransportationType");
            var formulas = GetFormulasWithGroupId(formulaGroupId);
            var storeProducts = storeProductRepo.Filter(x => x.StoreId == storeId && x.IsFixed == false);
            var formulaResults = new Dictionary<string, string>();
            string functionType = null;

            try
            {

                var products = productRepo.Table()
                    .Include(x => x.VatRate)
                    .Include(x => x.ParentProduct)
                    .Include(x => x.ProductAttributeMappings)
                    .Where(x => storeProducts.Select(p => p.ProductId).Contains(x.Id))
                    .Select(x => new ProductParametersHelper()
                    {
                        ProductId = x.Id,
                        BuyingPrice = x.BuyingPrice == null ? 0 : x.BuyingPrice.Value,
                        BuyingPriceCurrencyId = x.CurrencyId ?? 1,
                        MarketPrice = x.PsfPrice == null ? 0 : x.PsfPrice.Value,
                        MarketPriceCurrencyId = x.PsfCurrencyId ?? 1,
                        CorporatePrice = x.CorporatePrice == null ? 0 : x.CorporatePrice.Value,
                        CorporatePriceCurrencyId = x.CorporateCurrencyId ?? 1,
                        Desi = x.Desi == null ? 0 : x.Desi.Value,
                        AbroadDesi = x.AbroadDesi == null ? 0 : x.AbroadDesi.Value,
                        BrandId = x.ParentProduct == null ? 0 : x.ParentProduct.BrandId.Value,
                        CategoryId = x.ParentProduct == null ? 0 : x.ParentProduct.CategoryId.Value,
                        Margin = 1,
                        // TODO: 12 id'si elle verildi(ShippingWeight), buna bir çözüm bul amısına kadar uzadı algoritma
                        ShippingWeight = x.ProductAttributeMappings.FirstOrDefault(sw => sw.AttributesId == 12).RequiredAttributeValue,
                        VatNumber = x.VatRate == null ? 1 : x.VatRate.Value,
                        StoreVatNumber = storeProducts.FirstOrDefault(s => s.ProductId == x.Id).VatValue //verdiğiniz analizi gondiklesinler
                    }).ToList();


                foreach (var product in products)
                {
                    foreach (var formula in formulas)
                    {
                        var clearedFormula = ArrangeFormula(formula, product, formulaResults, currencies, margins, cargo, transportations, store, out functionType);
                        if (string.IsNullOrWhiteSpace(functionType))
                        {
                            formula.CalculatedResult = engine.Calculate(clearedFormula).ToString("0.####");
                            formulaResults.Add(formula.Result, formula.CalculatedResult);
                        }
                    }

                    var price = engine.Calculate($"{formulaResults.LastOrDefault().Value}/{store.Currency.LiveValue}").ToString("0.##");

                    product.SellingPrice = !string.IsNullOrWhiteSpace(functionType) ? engine.Calculate($"{functionType}({price})").ToString("0.####") : price;

                    var storeProduct = storeProducts.FirstOrDefault(x => x.ProductId == product.ProductId);

                    if (storeProduct.Price != decimal.Parse(product.SellingPrice, NumberStyles.Currency, new CultureInfo("en-US")))
                    {
                        storeProduct.Price = decimal.Parse(product.SellingPrice, NumberStyles.Currency, new CultureInfo("en-US"));
                        storeProduct.UpdatedDate = DateTime.Now;
                    }

                    if (storeProduct.IsFixedPoint != true)
                        storeProduct.Point = decimal.Parse(engine.Calculate($"RoundToUpper({product.SellingPrice}*{store.Rate})").ToString(), NumberStyles.Currency, new CultureInfo("en-US"));

                    formulaResults.Clear();
                    formulas = GetFormulasWithGroupId(formulaGroupId);
                }

                storeProductRepo.BulkUpdate(storeProducts.ToList());

                exceptionMessage = string.Empty;

                return true;

            }
            catch (Exception ex)
            {
                exceptionMessage = ex.Message;
                return false;
            }
        }

        private decimal CalculateParite(int firstCurrencyId, int secondCurrencyId, ICollection<CurrencyModel> currencies)
        {
            if (firstCurrencyId == secondCurrencyId)
                return 1.0M;

            var currencyIds = new List<int>() { firstCurrencyId, secondCurrencyId };

            var calculationCurrencies = currencies.Where(x => currencyIds.Contains(x.Id));

            return calculationCurrencies.ElementAt(1).LiveValue / calculationCurrencies.ElementAt(0).LiveValue;

        }

        private decimal CalculateMargin(ProductParametersHelper productParameters, ICollection<Margin> margins, StoreModel store)
        {
            var marj = 0M;

            if (margins.Count == 0)
                return 1 + marj;

            var marjIds = new List<int>() { productParameters.ProductId, productParameters.CategoryId, productParameters.BrandId };
            
            var filtered = new List<Margin>();
            
            //TODO: Düzelt Burayı
            var productMarj = margins.FirstOrDefault(x => x.EntityId == productParameters.ProductId && x.MarginTypeId == 1);
            var categoryBrandMarj = margins.FirstOrDefault(x => x.EntityId == productParameters.CategoryId && x.SecondEntityId == productParameters.BrandId && x.MarginTypeId == 2);
            var brandMarj = margins.FirstOrDefault(x => x.EntityId == productParameters.BrandId && x.MarginTypeId == 3);
            var categoryMarj = margins.FirstOrDefault(x => x.EntityId == productParameters.CategoryId && x.MarginTypeId == 4);

            if (productMarj != null)
                filtered.Add(productMarj);
            if (categoryBrandMarj != null)
                filtered.Add(categoryBrandMarj);
            if (brandMarj != null)
                filtered.Add(brandMarj);
            if (categoryMarj != null)
                filtered.Add(categoryMarj);

            //var filtered = margins.Where(x => marjIds.Contains(x.EntityId) || (marjIds.Contains(x.EntityId) && marjIds.Contains(x.SecondEntityId.Value))).OrderBy(o => o.MarginType.Rank);

            if (filtered != null && filtered.Count() > 0)
                marj = filtered.OrderBy(o => o.MarginType.Rank).FirstOrDefault().Profit / 100;
            else
                marj = (store.DefaultMarj ?? 0) / 100;

            return 1 + marj;
        }

        private decimal CalculateCargo(ProductParametersHelper productParameters, ICollection<Cargo> cargo, int cargoType, int currencyId, ICollection<CurrencyModel> currencies)
        {
            var cargoValue = 0M;

            if (cargo.Count == 0)
                return cargoValue;

            if (cargoType == (int)CargoEnum.Desi)
            {
                var isBiggerThanMax = productParameters.Desi > cargo.FirstOrDefault(x => x.IsLastDesi).MaxDesi ? true : false;

                if (!isBiggerThanMax)
                    cargoValue = cargo.FirstOrDefault(x => productParameters.Desi <= x.MaxDesi && productParameters.Desi >= x.MinDesi).Value;
                else
                    cargoValue = cargo.FirstOrDefault(x => !x.IsLastDesi).Value + (cargo.FirstOrDefault(x => x.IsLastDesi).Value * (productParameters.Desi - cargo.FirstOrDefault(x => x.IsLastDesi).MaxDesi));
            }
            else if (cargoType == (int)CargoEnum.AbroadDesi)
            {
                var isBiggerThanMax = productParameters.AbroadDesi > cargo.FirstOrDefault(x => x.IsLastDesi).MaxDesi ? true : false;

                if (!isBiggerThanMax)
                    cargoValue = cargo.FirstOrDefault(x => productParameters.AbroadDesi <= x.MaxDesi && productParameters.AbroadDesi >= x.MinDesi).Value;
                else
                    cargoValue = cargo.FirstOrDefault(x => !x.IsLastDesi).Value + (cargo.FirstOrDefault(x => x.IsLastDesi).Value * (productParameters.AbroadDesi - cargo.FirstOrDefault(x => x.IsLastDesi).MaxDesi));
            }
            else if (cargoType == (int)CargoEnum.ShippingWeight)
            {
                //TODO: Sevmedim ama vereceğiniz analizi gondiklesinler
                if (decimal.TryParse(productParameters.ShippingWeight, out decimal shippingWeight))
                {
                    var isBiggerThanMax = shippingWeight > cargo.FirstOrDefault(x => x.IsLastDesi).MaxDesi ? true : false;

                    if (!isBiggerThanMax)
                        cargoValue = cargo.FirstOrDefault(x => shippingWeight <= x.MaxDesi && shippingWeight >= x.MinDesi).Value;
                    else
                        cargoValue = cargo.FirstOrDefault(x => !x.IsLastDesi).Value + (cargo.FirstOrDefault(x => x.IsLastDesi).Value * (shippingWeight - cargo.FirstOrDefault(x => x.IsLastDesi).MaxDesi));
                }
            }

            return cargoValue * CalculateParite(1, currencyId, currencies);
        }

        private decimal CalculateTransportation(ProductParametersHelper productParameters, ICollection<Transportation> transportations, ICollection<CurrencyModel> currencies)
        {
            var transport = 0M;

            if (transportations.Count == 0)
                return transport;

            var transportationIds = new List<int>() { productParameters.ProductId, productParameters.BrandId };

            var filtered = transportations.Where(x => transportationIds.Contains(x.EntityId.Value)).OrderBy(o => o.TransportationType.Rank).FirstOrDefault();

            if (filtered != null)
                transport = filtered.Value * CalculateParite(1, filtered.CurrencyId.Value, currencies) ?? 1 * CalculateParite(1, filtered.CurrencyId.Value, currencies);

            return transport;

        }

        private decimal CalculateVatRate(int vatNumber)
        {
            if (vatNumber == 0)
                return 1M;

            return vatNumber / 100M + 1;
        }

        private IEnumerable<string> GetSubStrings(string input, string start, string end)
        {
            Regex r = new Regex(Regex.Escape(start) + "(.*?)" + Regex.Escape(end));
            MatchCollection matches = r.Matches(input);
            foreach (Match match in matches)
                yield return match.Groups[1].Value;
        }

        private string ArrangeFormula(FormulaModel formula, ProductParametersHelper productParameters,
            Dictionary<string, string> formulaResults,
            ICollection<CurrencyModel> currencies,
            ICollection<Margin> margins,
            ICollection<Cargo> cargo,
            ICollection<Transportation> transportations,
            StoreModel store, out string functionType)
        {

            var parite = 1.0M;
            var allSubVariables = GetSubStrings(formula.FormulaStr, "{", "}");
            functionType = null;

            //TODO: Anasının amı demeyin oluyor. Sonra düzeltilecek.
            foreach (var variable in allSubVariables)
            {
                switch (variable)
                {
                    case "BP":
                        parite = CalculateParite(1, productParameters.BuyingPriceCurrencyId, currencies);
                        formula.FormulaStr = formula.FormulaStr.Replace("{" + variable + "}", $"({productParameters.BuyingPrice.ToString()}*{parite})");
                        break;
                    case "MP":
                        parite = CalculateParite(1, productParameters.MarketPriceCurrencyId, currencies);
                        formula.FormulaStr = formula.FormulaStr.Replace("{" + variable + "}", $"({productParameters.MarketPrice.ToString()}*{parite})");
                        break;
                    case "CP":
                        parite = CalculateParite(1, productParameters.CorporatePriceCurrencyId, currencies);
                        formula.FormulaStr = formula.FormulaStr.Replace("{" + variable + "}", $"({productParameters.CorporatePrice.ToString()}*{parite})");
                        break;
                    case "D":
                        formula.FormulaStr = formula.FormulaStr.Replace("{" + variable + "}", productParameters.Desi.ToString());
                        break;
                    case "AD":
                        formula.FormulaStr = formula.FormulaStr.Replace("{" + variable + "}", productParameters.AbroadDesi.ToString());
                        break;
                    case "TRY":
                        formula.FormulaStr = formula.FormulaStr.Replace("{" + variable + "}", currencies.FirstOrDefault(x => x.Abbrevation == "TRY").Value.ToString());
                        break;
                    case "USD":
                        formula.FormulaStr = formula.FormulaStr.Replace("{" + variable + "}", currencies.FirstOrDefault(x => x.Abbrevation == "USD").Value.ToString());
                        break;
                    case "EUR":
                        formula.FormulaStr = formula.FormulaStr.Replace("{" + variable + "}", currencies.FirstOrDefault(x => x.Abbrevation == "EUR").Value.ToString());
                        break;
                    case "GBP":
                        formula.FormulaStr = formula.FormulaStr.Replace("{" + variable + "}", currencies.FirstOrDefault(x => x.Abbrevation == "GBP").Value.ToString());
                        break;
                    case "CHF":
                        formula.FormulaStr = formula.FormulaStr.Replace("{" + variable + "}", currencies.FirstOrDefault(x => x.Abbrevation == "CHF").Value.ToString());
                        break;
                    case "AED":
                        formula.FormulaStr = formula.FormulaStr.Replace("{" + variable + "}", currencies.FirstOrDefault(x => x.Abbrevation == "AED").Value.ToString());
                        break;
                    case "PLN":
                        formula.FormulaStr = formula.FormulaStr.Replace("{" + variable + "}", currencies.FirstOrDefault(x => x.Abbrevation == "PLN").Value.ToString());
                        break;
                    case "MB_USD":
                        formula.FormulaStr = formula.FormulaStr.Replace("{" + variable + "}", currencies.FirstOrDefault(x => x.Abbrevation == "USD").LiveValue.ToString());
                        break;
                    case "MB_EUR":
                        formula.FormulaStr = formula.FormulaStr.Replace("{" + variable + "}", currencies.FirstOrDefault(x => x.Abbrevation == "EUR").LiveValue.ToString());
                        break;
                    case "MB_GBP":
                        formula.FormulaStr = formula.FormulaStr.Replace("{" + variable + "}", currencies.FirstOrDefault(x => x.Abbrevation == "GBP").LiveValue.ToString());
                        break;
                    case "MB_CHF":
                        formula.FormulaStr = formula.FormulaStr.Replace("{" + variable + "}", currencies.FirstOrDefault(x => x.Abbrevation == "CHF").LiveValue.ToString());
                        break;
                    case "KDV":
                        var vatNumber = CalculateVatRate(productParameters.StoreVatNumber ?? productParameters.VatNumber);
                        formula.FormulaStr = formula.FormulaStr.Replace("{" + variable + "}", vatNumber.ToString());
                        break;
                    case "MARJ":
                        formula.FormulaStr = formula.FormulaStr.Replace("{" + variable + "}", $"{CalculateMargin(productParameters, margins, store)}");
                        break;
                    case "KARGO":
                        formula.FormulaStr = formula.FormulaStr.Replace("{" + variable + "}", $"{CalculateCargo(productParameters, cargo, store.CargoTypeId ?? 1, store.CargoCurrencyId ?? 1, currencies)}");
                        break;
                    case "NAKLIYE":
                        formula.FormulaStr = formula.FormulaStr.Replace("{" + variable + "}", $"{CalculateTransportation(productParameters, transportations, currencies)}");
                        break;
                    case "SARF":
                        formula.FormulaStr = formula.FormulaStr.Replace("{" + variable + "}", store.Sarf.ToString());
                        break;
                    case "ROUNDTO90":
                        functionType = "Round90";
                        break;
                    case "ROUNDTO99":
                        functionType = "Round99";
                        break;
                    default:
                        break;
                }

                if (variable.Contains("res"))
                    formula.FormulaStr = formula.FormulaStr.Replace("{" + variable + "}", formulaResults[variable]);
            }

            return formula.FormulaStr;
        }

        public void CalculateAllPrices()
        {
            var storesWithFormulas = storeRepo.FindAll(x => x.FormulaGroupId != null);

            var storeIds = storesWithFormulas.Select(x => x.Id).ToList();

            foreach (var storeId in storeIds)
            {
                SellingPrice(storeId, out string exception);
            }
        }

        #endregion

        #region Margins

        public ICollection<MarginTypeModel> MarginTypes()
        {
            return autoMapper.MapCollection<MarginType, MarginTypeModel>(marginTypeRepo.GetAll()).ToList();
        }

        public ICollection<MarginModel> StoreMarginsByStoreId(int storeId, int marginTypeId)
        {
            return autoMapper.MapCollection<Margin, MarginModel>(marginRepo.Filter(x => x.StoreId == storeId && x.MarginTypeId == marginTypeId)).ToList();
        }

        // for margins
        public ICollection<MarginModel> StoreBrands(int storeId, int marginTypeId)
        {
            var list = brandService.AllActiveBrands();

            var brandMargins = marginRepo.Table().Where(x => list.Any(k => k.Id == x.EntityId) && x.StoreId == storeId && x.MarginTypeId == marginTypeId).ToList();

            var _brandMargins = (from s in list
                                 join b in brandMargins on s.Id equals b.EntityId into sb
                                 from b in sb.DefaultIfEmpty()
                                 select new MarginModel
                                 {
                                     Id = b != null ? b.Id : 0,
                                     MarginTypeId = marginTypeId,
                                     Name = s.Name,
                                     EntityId = s.Id,
                                     StoreId = storeId,
                                     Profit = b != null ? b.Profit : 0
                                 }).ToList();

            return _brandMargins;
        }

        public ICollection<MarginModel> StoreCategory(int storeId, int marginTypeId)
        {
            var list = categoryService.AllActiveCategories();


            var categoryMargins = marginRepo.Table().Where(x => list.Any(k => k.Id == x.EntityId) && x.StoreId == storeId && x.MarginTypeId == marginTypeId).ToList();

            var _categoryMargins = (from s in list
                                    join b in categoryMargins on s.Id equals b.EntityId into sb
                                    from b in sb.DefaultIfEmpty()
                                    select new MarginModel
                                    {
                                        Id = b != null ? b.Id : 0,
                                        MarginTypeId = marginTypeId,
                                        EntityId = s.Id,
                                        StoreId = storeId,
                                        Profit = b != null ? b.Profit : 0,
                                        categoryWithParents = categoryService.CategoryWithParentNames(s.Id)
                                    }).ToList();

            return _categoryMargins;
        }

        public ICollection<MarginModel> StoreProducts(int storeId, int marginTypeId)
        {
            var list = storeProductRepo.Table().Where(x => x.StoreId == storeId).Include(x => x.Product).Select(x => new ProductModel
            {
                Id = x.Product.Id,
                Sku = x.Product.Sku,
                Name = x.Product.Name
            }).ToList();

            var productMargins = marginRepo.Table().Where(x => list.Any(k => k.Id == x.EntityId) && x.StoreId == storeId && x.MarginTypeId == marginTypeId).ToList();

            var _productMargins = (from s in list
                                   join b in productMargins on s.Id equals b.EntityId into sb
                                   from b in sb.DefaultIfEmpty()
                                   select new MarginModel
                                   {
                                       Id = b != null ? b.Id : 0,
                                       EntityId = s.Id,
                                       MarginTypeId = marginTypeId,
                                       Profit = b != null ? b.Profit : 0,
                                       ProductId = s.Id,
                                       StoreId = storeId,
                                       Name = s.Name,
                                       Sku = s.Sku
                                   }).ToList();

            return _productMargins;
        }

        public ICollection<MarginModel> MarginCategoryBrand(int storeId, int marginTypeId)
        {
            var _brands = brandService.AllActiveBrands();
            var _catBrandMargins = marginRepo.Table().Where(x => x.StoreId == storeId && x.MarginTypeId == marginTypeId)
                                .Select(x => new MarginModel
                                {
                                    Id = x.Id,
                                    EntityId = x.EntityId,
                                    SecondEntityId = x.SecondEntityId,
                                    MarginTypeId = marginTypeId,
                                    StoreId = storeId,
                                    Profit = x.Profit
                                }).ToList();

            foreach (var item in _catBrandMargins)
            {
                var _brand = _brands.FirstOrDefault(m => m.Id == item.SecondEntityId);
                item.Name = categoryService.CategoryWithParentNames(item.EntityId);
                item.SecondName = _brand != null ? _brand.Name : string.Empty;
            }
            return _catBrandMargins;
        }

        public MarginModel MarginUpdate(MarginModel margin)
        {
            if (margin.Id != 0)
            {
                return autoMapper.MapObject<Margin, MarginModel>(marginRepo.Update(autoMapper.MapObject<MarginModel, Margin>(margin)));
            }
            else
                return autoMapper.MapObject<Margin, MarginModel>(marginRepo.Add(autoMapper.MapObject<MarginModel, Margin>(margin)));

        }

        public MarginModel MarginDelete(MarginModel margin)
        {
            if (margin.Id != 0)
                marginRepo.Delete(autoMapper.MapObject<MarginModel, Margin>(margin));

            return margin;
        }

        public MarginModel MarginInsert(MarginModel margin)
        {
            var data = marginRepo.Filter(x => x.StoreId == margin.StoreId && x.MarginTypeId == margin.MarginTypeId && x.EntityId == margin.EntityId && x.SecondEntityId == x.SecondEntityId).FirstOrDefault();
            if (data == null)
            {
                return autoMapper.MapObject<Margin, MarginModel>(marginRepo.Add(autoMapper.MapObject<MarginModel, Margin>(margin)));
            }
            else
                return margin;
        }

        #endregion
    }
}
