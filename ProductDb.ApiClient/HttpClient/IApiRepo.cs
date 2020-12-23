using ProductDb.Common.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApiClient.HttpClient
{
    public interface IApiRepo
    {
        Task<ApiResult> GetBearerToken<T>(string apiPath,Endpoints uri,MediaType mediaType);
        Task<ApiResult> GetAuthorizeToken<T>(string apiPath,T obj,string bearerToken,Endpoints uri, MediaType mediaType);
        //Task<ApiResult> GetBearerTokenAdmin<T>(string apiPath, T obj, Endpoints uri = Endpoints.Club);
        Task<ApiResult> GetWithStatusCode<T>(string apiPath, string bearerToken, Endpoints uri, MediaType mediaType);
        Task<List<T>> GetList<T>(string apiPath, string bearerToken,Endpoints uri, MediaType mediaType);
        Task<T> GetItem<T>(string apiPath, string bearerToken, Endpoints uri, MediaType mediaType);
        Task<int> GetExternalItem<T>(string apiPath, string bearerToken, Endpoints uri, MediaType mediaType);
        Task<int> Put<T>(string apiPath, T obj, string bearerToken, Endpoints uri, MediaType mediaType);
        Task<ApiResult> Post<T>(string apiPath, string bearerToken, Endpoints uri, MediaType mediaType);
        Task<ApiResult> Post<T>(string apiPath,T obj ,string bearerToken, Endpoints uri, MediaType mediaType);
        Task<bool> GetBoolean<T>(string apiPath, string bearerToken, Endpoints uri, MediaType mediaType);
        Task<int> Delete<T>(string apiPath, string bearerToken, Endpoints uri, MediaType mediaType);
        string GetUri(Endpoints uri,string apiPath);
        Task<ApiResult> Push<T>(string apiPath,T obj, Endpoints uri, MediaType mediaType);
        Task<ApiResult> BulkPushProduct<T>(string apiPath, T obj, Endpoints uri, MediaType mediaType);
        Task<int> PutInfo<T>(string apiPath, T obj,Endpoints uri, MediaType mediaType);
        Task<int> PutInfo<T>(string apiPath, Endpoints uri, MediaType mediaType);
        Task<List<T>> GetProduct<T>(string apiPath, Endpoints uri, MediaType mediaType);
        Task<ApiResult> BatchPost<T>(string apiPath, T obj, string bearerToken ,Endpoints uri, MediaType mediaType);

    }
}
