using BestStories.Retriever.Application.Abstractions;
using Microsoft.Extensions.Caching.Memory;

namespace BestStories.Retriever.Application.Services
{
    public class CacheService : ICacheService
    {
        private readonly IMemoryCache _cache;

        public CacheService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public T Get<T>(string key)
        {
            _cache.TryGetValue(key, out T value);
            return value;
        }

        public void Set<T>(string key, T value, TimeSpan expiration)
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(expiration);

            _cache.Set(key, value, cacheEntryOptions);
        }
    }
}
