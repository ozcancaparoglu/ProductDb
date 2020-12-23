using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ApiClient.HttpClient
{
    public static class HttpRequestFactory
    {
        public static async Task<HttpResponseMessage> Get(string requestUri,string mediaType)
            => await Get(requestUri,"",mediaType);

        public static async Task<HttpResponseMessage> Get(string requestUri, string bearerToken, string mediaType)
        {
            var builder = new HttpRequestBuilder()
                                .AddMethod(HttpMethod.Get)
                                .AddRequestUri(requestUri)
                                .AddBearerToken(bearerToken);

            return await builder.SendAsync(mediaType);
        }

        public static async Task<HttpResponseMessage> Post(string requestUri, string mediaType)
            => await Post(requestUri,"",mediaType);

        public static async Task<HttpResponseMessage> Post(
            string requestUri, object value, string bearerToken, string mediaType)
        {
            var jsoncontent = new JsonContent(value);

            var builder = new HttpRequestBuilder()
                                .AddMethod(HttpMethod.Post)
                                .AddRequestUri(requestUri)
                                .AddContent(jsoncontent)
                                .AddBearerToken(bearerToken);

            return await builder.SendAsync(mediaType);
        }

        public static async Task<HttpResponseMessage> Post(
        string requestUri, string bearerToken, string mediaType)
        {

            var builder = new HttpRequestBuilder()
                                .AddMethod(HttpMethod.Post)
                                .AddRequestUri(requestUri)
                                .AddBearerToken(bearerToken);

            return await builder.SendAsync(mediaType);
        }

        public static async Task<HttpResponseMessage> Post(
            string requestUri, object value, string customKey, string customValue, string mediaType)
        {
            var builder = new HttpRequestBuilder()
                                .AddMethod(HttpMethod.Post)
                                .AddRequestUri(requestUri)
                                .AddContent(new JsonContent(value))
                                .AddCustom(customKey, customValue);

            return await builder.SendAsync(mediaType);
        }

        public static async Task<HttpResponseMessage> Put(string requestUri, string mediaType)
        {
            var builder = new HttpRequestBuilder()
                    .AddMethod(HttpMethod.Put)
                    .AddRequestUri(requestUri);

            return await builder.SendAsync(mediaType);
        }

        public static async Task<HttpResponseMessage> Put(string requestUri, object value, string mediaType)
        {
            var builder = new HttpRequestBuilder()
                    .AddMethod(HttpMethod.Put)
                    .AddRequestUri(requestUri)
                    .AddContent(new JsonContent(value));

            return await builder.SendTimeLimit(mediaType);
        }

        public static async Task<HttpResponseMessage> Put(
            string requestUri, object value, string bearerToken, string mediaType)
        {
            var builder = new HttpRequestBuilder()
                                .AddMethod(HttpMethod.Put)
                                .AddRequestUri(requestUri)
                                .AddContent(new JsonContent(value))
                                .AddBearerToken(bearerToken);

            return await builder.SendAsync(mediaType);
        }

        public static async Task<HttpResponseMessage> Patch(string requestUri, object value, string mediaType)
            => await Patch(requestUri, value, "");

        public static async Task<HttpResponseMessage> Patch(
            string requestUri, object value, string bearerToken, string mediaType)
        {
            var builder = new HttpRequestBuilder()
                                .AddMethod(new HttpMethod("PATCH"))
                                .AddRequestUri(requestUri)
                                .AddContent(new PatchContent(value))
                                .AddBearerToken(bearerToken);

            return await builder.SendAsync(mediaType);
        }

        public static async Task<HttpResponseMessage> Delete(string requestUri, string mediaType)
            => await Delete(requestUri, "");

        public static async Task<HttpResponseMessage> Delete(
            string requestUri, string bearerToken, string mediaType)
        {
            var builder = new HttpRequestBuilder()
                                .AddMethod(HttpMethod.Delete)
                                .AddRequestUri(requestUri)
                                .AddBearerToken(bearerToken);

            return await builder.SendAsync(mediaType);
        }

        public static async Task<HttpResponseMessage> PostFile(string requestUri,
            string filePath, string apiParamName, string mediaType)
            => await PostFile(requestUri, filePath, apiParamName, "");

        public static async Task<HttpResponseMessage> PostFile(string requestUri,
            string filePath, string apiParamName, string bearerToken, string mediaType)
        {
            var builder = new HttpRequestBuilder()
                                .AddMethod(HttpMethod.Post)
                                .AddRequestUri(requestUri)
                                .AddContent(new FileContent(filePath, apiParamName))
                                .AddBearerToken(bearerToken);

            return await builder.SendAsync(mediaType);
        }

        public static async Task<HttpResponseMessage> RefreshToken(string requestUri, object value, string bearerToken, string mediaType)
        {
            var keyValues = value.ToKeyValue();

            var content = new FormUrlEncodedContent(keyValues);

            var urlEncodedString = await content.ReadAsStringAsync();

            var jsoncontent = new StringContent(urlEncodedString, Encoding.UTF8, "application/x-www-form-urlencoded");

            var builder = new HttpRequestBuilder()
                                .AddMethod(HttpMethod.Post)
                                .AddRequestUri(requestUri)
                                .AddBearerToken(bearerToken)
                                .AddContent(jsoncontent);

            return await builder.GetCAToken(mediaType);
        }

        public static async Task<HttpResponseMessage> Push(string requestUri, object value,string mediaType)
        {

            var jsoncontentObj = new JsonContent(value);

            var builder = new HttpRequestBuilder()
                                .AddMethod(HttpMethod.Post)
                                .AddRequestUri(requestUri)
                                .AddContent(jsoncontentObj);
            //sendasync yapman lazım
            return await builder.SendTimeLimit(mediaType);
        }

        public static async Task<HttpResponseMessage> BulkPushProduct(string requestUri, object value, string mediaType)
        {
            //excel için gerekn dataya bakmak lazım
            var jsoncontentObj = new JsonContent(value);

            var builder = new HttpRequestBuilder()
                                .AddMethod(HttpMethod.Post)
                                .AddRequestUri(requestUri)
                                .AddContent(jsoncontentObj);

            return await builder.SendTimeLimit(mediaType);
        }

        public static async Task<HttpResponseMessage> JoomUpdate(string requestUri, object value, string bearerToken, string mediaType)
        {
            var keyValues = value.ToKeyValue();

            var content = new FormUrlEncodedContent(keyValues);

            var urlEncodedString = await content.ReadAsStringAsync();

            var jsoncontent = new StringContent(urlEncodedString, Encoding.UTF8, "application/x-www-form-urlencoded");

            var builder = new HttpRequestBuilder()
                                .AddMethod(HttpMethod.Post)
                                .AddRequestUri(requestUri)
                                .AddBearerToken(bearerToken)
                                .AddContent(jsoncontent);

            return await builder.SendTimeLimit(mediaType);
        }
    }

    public static class ObjectExtensions
    {
        public static IDictionary<string, string> ToKeyValue(this object metaToken)
        {
            if (metaToken == null)
            {
                return null;
            }

            JToken token = metaToken as JToken;
            if (token == null)
            {
                return ToKeyValue(JObject.FromObject(metaToken));
            }

            if (token.HasValues)
            {
                var contentData = new Dictionary<string, string>();
                foreach (var child in token.Children().ToList())
                {
                    var childContent = child.ToKeyValue();
                    if (childContent != null)
                    {
                        contentData = contentData.Concat(childContent)
                            .ToDictionary(k => k.Key, v => v.Value);
                    }
                }

                return contentData;
            }

            var jValue = token as JValue;
            if (jValue?.Value == null)
            {
                return null;
            }

            var value = jValue?.Type == JTokenType.Date ?
                jValue?.ToString("o", CultureInfo.InvariantCulture) :
                jValue?.ToString(CultureInfo.InvariantCulture);

            return new Dictionary<string, string> { { token.Path, value } };
        }
    }
}
