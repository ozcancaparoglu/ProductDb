namespace ProductDb.Services.ImportServices
{
    public interface ISupplierService<T> where T : class
    {
        bool InsertXmlToDb(int? supplierId, int? rootCategoryId, string url, string startNode);
    }
}
