//using Logger;
using PMS.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PMS.Common
{
    public class ApiRepo : IApiRepo
    {
        //private ILoggerManager _logger;

        //public ApiRepo(ILoggerManager _logger)
        //{
        //    this._logger = _logger;
        //}

        public async Task<int> Delete<T>(string apiPath, string bearerToken, Endpoints uri)
        {
            var requestUri = $"{GetUri(uri)}/{apiPath}";
            var response = await HttpRequestFactory.Delete(requestUri, bearerToken);
            return (int)response.StatusCode;
        }

        public async Task<ApiResult> GetBearerToken<T>(string apiPath, T obj, Endpoints uri)
        {
            var requestUri = $"{GetUri(uri)}/{apiPath}";
            var response = await HttpRequestFactory.Post(requestUri, obj);
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                return null;
            ApiResult result = new ApiResult
            {
                ResponseCode = (int)response.StatusCode,
                JsonContent = response.ContentAsString()
            };
            return result;
        }

        public async Task<T> GetItem<T>(string apiPath, string bearerToken, Endpoints uri)
        {
            var requestUri = $"{GetUri(uri)}/" + apiPath;

            var response = await HttpRequestFactory.Get(requestUri, bearerToken);
            return response.ContentAsType<T>();
        }

        public async Task<List<T>> GetList<T>(string apiPath, string bearerToken, Endpoints uri)
        {
            var requestUri = $"{GetUri(uri)}/{apiPath}";
           
            var response = await HttpRequestFactory.Get(requestUri, bearerToken);
            return response.ContentAsType<List<T>>();
        }

        public string GetUri(Endpoints uri, string apiPath = "")
        {
            switch (uri)
            {
                case Endpoints.PMS:
                    //return "http://localhost:57247/api";
                    return "http://pmsapi.sanalmagaza.com/api";
                //return "http://api.PMT.com/api";

                default:
                    return "";
            }
        }

        public async Task<ApiResult> Post<T>(string apiPath, T obj, string bearerToken, Endpoints uri)
        {
            try
            {
                var requestUri = $"{GetUri(uri)}/{apiPath}";

                var response = await HttpRequestFactory.Post(requestUri, obj, bearerToken);
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

        public async Task<int> Put<T>(string apiPath, T obj, string bearerToken, Endpoints uri)
        {
            var requestUri = $"{GetUri(uri)}/" + apiPath;
            var response = await HttpRequestFactory.Put(requestUri, obj, bearerToken);
            return (int)response.StatusCode;
        }

        public ApiResult PostSync<T>(string apiPath, T obj, string bearerToken, Endpoints uri = Endpoints.PMS)
        {
            //var requestUri = $"{GetUri(uri)}/{apiPath}";
            //System.Net.Http.HttpClient client = new System.Net.Http.HttpClient();
            //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
            //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
            //HttpResponseMessage response = client.PostAsync(requestUri, obj).Result;
            //ApiResult result = new ApiResult
            //{
            //    ResponseCode = (int)response.StatusCode,
            //    JsonContent = response.ContentAsString()
            //};
            //return result;
            throw new NotImplementedException();
        }
    }
}