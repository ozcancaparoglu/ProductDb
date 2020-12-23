using Microsoft.Extensions.Caching.Memory;
using System;

namespace ProductDb.Common.Cache
{
    public class CacheManager : ICacheManager
    {
        private readonly IMemoryCache memoryCache;
        public CacheManager(IMemoryCache memoryCache)
        {
            this.memoryCache = memoryCache;
        }
        public T Set<T>(string key, T t, int time)
        {
            var cacheExpirationOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddMinutes(time),
                Priority = CacheItemPriority.High
            };

            memoryCache.Set(key, t, cacheExpirationOptions);

            return t;
        }

        public bool TryGetValue<TItem>(string key, out TItem value)
        {
            return memoryCache.TryGetValue(key, out value);
        }
        public TItem Get<TItem>(string key)
        {
            return memoryCache.Get<TItem>(key);
        }

        public void Remove(string key)
        {
            memoryCache.Remove(key);
        }
    }
}
