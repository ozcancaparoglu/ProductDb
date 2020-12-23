using ProductDb.Data.OnnetDb;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProductDb.Services.OnnetServices
{
    public interface IOnnetService
    {
        IList<OnnetProduct> GetOnnetProducts(string ProjectCode, List<string> SKUs);
        string GetOnnetProductSqlText(string ProjectCode,List<string> SKUs);
    }
}
