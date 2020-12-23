using ProductDb.Mapping.BiggBrandDbModels.ImportModels;
using System.Collections.Generic;
using System.Text;

namespace ProductDb.Services.ImportServices
{
    public interface IExcelService
    {
        IEnumerable<ProductImportModel> ReadProductModel(string path);
        void UpdateProductModel(IEnumerable<ProductImportModel> products, int languageId, out StringBuilder errorList);
        bool isInCapacity(string path);
        bool isValidDocument(string extension);
        string DownloadExcelTemplateUrl();
        IEnumerable<SynchronizationNodeModel> ReadSynchronizationNodeModel(string path);
        void UpdateStoreNodes(IEnumerable<SynchronizationNodeModel> nodeModels, out List<string> errorCodes);
        IEnumerable<StoreProductFixingModel> ReadStoreProductFixing(string path);
        void UpdateStoreProductFixing(IEnumerable<StoreProductFixingModel> nodeModels, out List<string> errorCodes);
    }
}
