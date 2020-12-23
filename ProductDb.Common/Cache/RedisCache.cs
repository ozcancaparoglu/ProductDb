using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ProductDb.Common.Cache
{
    public class RedisCache : IRedisCache
    {
        private IDistributedCache cache;

        public RedisCache(IDistributedCache cache)
        {
            this.cache = cache;
        }
        public async Task<TItem> GetAsync<TItem>(string key)
        {  
            var data = await cache.GetStringAsync(key);
            return JsonConvert.DeserializeObject<TItem>(data);
        }

        public bool IsCached(string key)
        {
            return !string.IsNullOrEmpty(cache.GetString(key)) ? true : false;
        }

        public void Remove(string key)
        {
            var data = cache.GetString(key);
            if (!string.IsNullOrEmpty(data))
            {
                cache.Remove(key);
            }
        }

        public async Task SetAsync<TITem>(string key, TITem item, int Time)
        {
            var option = new DistributedCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(Time));

            var data = JsonConvert.SerializeObject(item);
            await cache.SetStringAsync(key, data, option);
        }
    }
}
