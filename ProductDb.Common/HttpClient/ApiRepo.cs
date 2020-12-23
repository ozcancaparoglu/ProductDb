using ProductDb.Common.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProductDb.Common.HttpClient
{
    public class ApiRepo : IApiRepo
    {
        //private ILoggerManager _logger;

        //public ApiRepo(ILoggerManager _logger)
        //{
        //    this._logger = _logger;
        //}


        public async Task<int> Delete<T>(string apiPath, string bearerToken, Endpoints uri, MediaType mediaType)
        {
            var requestUri = $"{GetUri(uri)}/{apiPath}";
            var response = await HttpRequestFactory.Delete(requestUri, bearerToken,GetMedia(mediaType));
            return (int)response.StatusCode;
        }
        public async Task<ApiResult> GetBearerToken<T>(string apiPath, Endpoints uri, MediaType mediaType)
        {
            var requestUri = $"{GetUri(uri)}/{apiPath}";
            var response = await HttpRequestFactory.Post(requestUri, GetMedia(mediaType));
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                return null;
            ApiResult result = new ApiResult
            {
                ResponseCode = (int)response.StatusCode,
                JsonContent = response.ContentAsString()
            };
            return result;
        }
        public async Task<ApiResult> GetWithStatusCode<T>(string apiPath, string bearerToken, Endpoints uri, MediaType mediaType)
        {
            var requestUri = $"{GetUri(uri)}/" + apiPath;
            var response = await HttpRequestFactory.Get(requestUri, bearerToken, GetMedia(mediaType));
            ApiResult result = new ApiResult
            {
                ResponseCode = (int)response.StatusCode,
                JsonContent = response.ContentAsString()
            };
            return result;
        }
        public async Task<bool> GetBoolean<T>(string apiPath, string bearerToken, Endpoints uri, MediaType mediaType)
        {
            var requestUri = $"{GetUri(uri)}/" + apiPath;
            var response = await HttpRequestFactory.Get(requestUri, bearerToken, GetMedia(mediaType));
            return response.ContentAsType<bool>();
        }
        public async Task<T> GetItem<T>(string apiPath, string bearerToken, Endpoints uri, MediaType mediaType)
        {
            var requestUri = $"{GetUri(uri)}/" + apiPath;
            var response = await HttpRequestFactory.Get(requestUri, bearerToken, GetMedia(mediaType));
            return response.ContentAsType<T>();
        }

        public async Task<List<T>> GetList<T>(string apiPath, string bearerToken, Endpoints uri, MediaType mediaType)
        {
            var requestUri = $"{GetUri(uri)}/{apiPath}";
            var response = await HttpRequestFactory.Get(requestUri, bearerToken, GetMedia(mediaType));
            return response.ContentAsType<List<T>>();
        }

        public async Task<ApiResult> Post<T>(string apiPath, string bearerToken, Endpoints uri, MediaType mediaType)
        {
            try
            {
                var requestUri = $"{GetUri(uri)}/{apiPath}";
                var response = await HttpRequestFactory.Post(requestUri, bearerToken, GetMedia(mediaType));
                ApiResult result = new ApiResult
                {
                    ResponseCode = (int)response.StatusCode,
                    JsonContent = response.ContentAsString()
                };
                return result;
            }
            catch (Exception ex)
            {
                return new ApiResult();
            }
        }

        public async Task<int> Put<T>(string apiPath, T obj, string bearerToken, Endpoints uri, MediaType mediaType)
        {
            var requestUri = $"{GetUri(uri)}/{apiPath}";
            var response = await HttpRequestFactory.Put(requestUri, obj, bearerToken, GetMedia(mediaType));
            return (int)response.StatusCode;
        }

        public async Task<ApiResult> GetAuthorizeToken<T>(string apiPath, T obj, string bearerToken, Endpoints uri, MediaType mediaType)
        {
            try
            {
                var requestUri = $"{GetUri(uri)}/{apiPath}";
                var response = await HttpRequestFactory.RefreshToken(requestUri, obj, bearerToken, GetMedia(mediaType));
                ApiResult result = new ApiResult
                {
                    ResponseCode = (int)response.StatusCode,
                    JsonContent = response.ContentAsString()
                };
                return result;
            }
            catch (Exception ex)
            {
                return new ApiResult();
            }
        }

        public async Task<ApiResult> Post<T>(string apiPath, T obj, string bearerToken, Endpoints uri, MediaType mediaType)
        {
            try
            {
                var requestUri = $"{GetUri(uri)}/{apiPath}";
                var response = await HttpRequestFactory.Post(requestUri, obj,bearerToken, GetMedia(mediaType));
                ApiResult result = new ApiResult
                {
                    ResponseCode = (int)response.StatusCode,
                    JsonContent = response.ContentAsString()
                };
                return result;
            }
            catch (Exception ex)
            {
                return new ApiResult();
            }
        }

        public async Task<ApiResult> Push<T>(string apiPath, T obj, Endpoints uri, MediaType mediaType)
        {
            try
            {
                var requestUri = $"{GetUri(uri)}/{apiPath}";
                var response = await HttpRequestFactory.Push(requestUri, obj, GetMedia(mediaType));
                ApiResult result = new ApiResult
                {
                    ResponseCode = (int)response.StatusCode,
                    JsonContent = response.ContentAsString()
                };
                return result;
            }
            catch (Exception ex)
            {
                return new ApiResult();
            }
        }

        public async Task<ApiResult> BulkPushProduct<T>(string apiPath, T obj, Endpoints uri, MediaType mediaType)
        {
            try
            {
                var requestUri = $"{GetUri(uri)}/{apiPath}";
                var response = await HttpRequestFactory.BulkPushProduct(requestUri, obj, GetMedia(mediaType));
                ApiResult result = new ApiResult
                {
                    ResponseCode = (int)response.StatusCode,
                    JsonContent = response.ContentAsString()
                };
                return result;
            }
            catch (Exception ex)
            {
                return new ApiResult();
            }
        }

        public async Task<int> PutInfo<T>(string apiPath, T obj, Endpoints uri, MediaType mediaType)
        {
            var requestUri = $"{GetUri(uri)}/{apiPath}";
            var response = await HttpRequestFactory.Put(requestUri, obj, GetMedia(mediaType));
            return (int)response.StatusCode;
        }

        public async Task<int> PutInfo<T>(string apiPath, Endpoints uri, MediaType mediaType)
        {
            var requestUri = $"{GetUri(uri)}/{apiPath}";
            var response = await HttpRequestFactory.Put(requestUri, GetMedia(mediaType));
            return (int)response.StatusCode;
        }

        public string GetUri(Endpoints uri, string apiPath = "")
        {
            switch (uri)
            {
                case Endpoints.ChannelAdvisor:
                    return "https://api.channeladvisor.com";
                case Endpoints.PMS:
                    return "https://api.channeladvisor.com";
                case Endpoints.Joomv2:
                    return "https://api-merchant.joom.com/api/v2";
                case Endpoints.ExportApi:
                    //return "https://localhost:44347/api";
                    return "http://exportapi.sanalmagaza.com/api";
                case Endpoints.Joomv3:
                    return "https://api-merchant.joom.com/api/v3";
                default:
                    return "";
            }
        }

        public string GetMedia(MediaType mediaType)
        {
            switch (mediaType)
            {
                case MediaType.Encoded:
                    return "application/x-www-form-urlencoded";
                case MediaType.Json:
                    return "application/json";
                case MediaType.Text:
                    return "text/tab-separated-values";
                case MediaType.Multipart:
                    return "multipart/mixed";
                default:
                    return "";
            }
        }

        public async Task<List<T>> GetProduct<T>(string apiPath, Endpoints uri, MediaType mediaType)
        {
            var requestUri = $"{GetUri(uri)}/{apiPath}";
            var response = await HttpRequestFactory.Get(requestUri, GetMedia(mediaType));
            string json = response.ContentAsJson();
            return response.ContentAsType<List<T>>();
        }

        public Task<ApiResult> BatchPost<T>(string apiPath, T obj, Endpoints uri, MediaType mediaType)
        {
            throw new NotImplementedException();
        }
    }
}
