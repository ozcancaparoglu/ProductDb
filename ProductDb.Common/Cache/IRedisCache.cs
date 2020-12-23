using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ProductDb.Common.Cache
{
    public interface IRedisCache
    {
        Task<TItem> GetAsync<TItem>(string key);
        Task SetAsync<TITem>(string key, TITem item,int Time);
        void Remove(string key);
        bool IsCached(string key);
    }
}
