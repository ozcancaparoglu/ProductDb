using ProductDb.Common.Entities;
using ProductDb.Mapping.BiggBrandDbModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProductDb.Services.CalculationServices
{
    public interface ICalculationService
    {
        #region Formula Group
        FormulaGroupModel GetFormulaGroupById(int? id);
        FormulaGroupModel AddNewFormulaGroup(FormulaGroupModel model);
        FormulaGroupModel EditFormulaGroup(FormulaGroupModel model);
        int DeleteFormulaGroupById(int id);
        ICollection<FormulaGroupModel> AllFormulaGroups();

        #endregion

        #region Formula
        ICollection<FormulaModel> GetFormulasWithGroupId(int formulaGroupId);
        int GetLastOrderFormulaByGroupId(int formulaGroupId);
        FormulaModel AddNewFormula(FormulaModel model);
        FormulaModel EditFormula(FormulaModel model);
        int DeleteFormulaById(int id);
        #endregion

        #region Cargo
        ICollection<CargoTypeModel> AllCargoTypes();
        ICollection<CargoModel> CargoByStoreId(int storeId);
        CargoModel InsertCargo(CargoModel cargo);
        CargoModel UpdateCargo(CargoModel cargo);
        void DeleteCargo(int id);
        #endregion

        #region Transportation
        ICollection<TransportationTypeModel> AllTransportationTypes();
        ICollection<TransportationModel> AllActiveTransportations(int storeId);
        TransportationModel AddNewTransportation(TransportationModel model);
        TransportationModel EditTransportation(TransportationModel model);
        void DeleteTransportation(int id);
        ICollection<TransportationModel> StoreTransportationProducts(int storeId);
        ICollection<TransportationModel> StoreTransportationBrands(int storeId);
        ICollection<TransportationTypeModel> StoreTransportationTypeEnum(List<int> IDs);
        #endregion

        #region Calculation
        bool SellingPrice(int storeId, out string exceptionMessage);
        void CalculateAllPrices();
        #endregion

        #region Margins
        ICollection<MarginTypeModel> MarginTypes();
        ICollection<MarginModel> StoreMarginsByStoreId(int storeId, int marginTypeId);
        #endregion

        #region StoreBrands
        ICollection<MarginModel> StoreBrands(int storeId, int marginTypeId);
        #endregion

        #region StoreCategory
        ICollection<MarginModel> StoreCategory(int storeId, int marginTypeId);
        #endregion

        #region StoreProducts
        ICollection<MarginModel> StoreProducts(int storeId, int marginTypeId);
        #endregion

        #region StoreCategoryBrands
        ICollection<MarginModel> MarginCategoryBrand(int storeId, int marginTypeId);
        #endregion

        #region MarginCRUD
        MarginModel MarginUpdate(MarginModel margin);
        MarginModel MarginDelete(MarginModel margin);
        MarginModel MarginInsert(MarginModel margin);
        #endregion

    }
}