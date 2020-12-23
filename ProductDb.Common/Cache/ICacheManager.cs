namespace ProductDb.Common.Cache
{
    public interface ICacheManager
    {
        T Set<T>(string key, T t, int time);
        bool TryGetValue<TItem>(string key, out TItem value);
        TItem Get<TItem>(string key);
        void Remove(string key);
    }
}