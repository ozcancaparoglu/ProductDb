using System.Collections.Generic;
using System.Threading.Tasks;

namespace PMS.Common
{
    public interface IApiRepo
    {
        Task<ApiResult> GetBearerToken<T>(string apiPath, T obj, Endpoints uri = Endpoints.PMS);

        Task<List<T>> GetList<T>(string apiPath, string bearerToken, Endpoints uri = Endpoints.PMS);

        Task<T> GetItem<T>(string apiPath, string bearerToken, Endpoints uri = Endpoints.PMS);

        Task<int> Put<T>(string apiPath, T obj, string bearerToken, Endpoints uri = Endpoints.PMS);

        Task<ApiResult> Post<T>(string apiPath, T obj, string bearerToken, Endpoints uri = Endpoints.PMS);

        ApiResult PostSync<T>(string apiPath, T obj, string bearerToken, Endpoints uri = Endpoints.PMS);

        Task<int> Delete<T>(string apiPath, string bearerToken, Endpoints uri = Endpoints.PMS);

        string GetUri(Endpoints uri, string apiPath = "");
    }
}