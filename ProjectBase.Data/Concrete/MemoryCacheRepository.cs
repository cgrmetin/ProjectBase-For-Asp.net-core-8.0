using ProjectBase.Data.Abstract;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Concurrent;

namespace ProjectBase.Data.Concrete
{
    public class MemoryCacheRepository : ICacheRepository
    {
        private readonly IMemoryCache _memoryCache;
        private readonly ConcurrentDictionary<string, bool> _cacheKeys;

        public MemoryCacheRepository(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
            _cacheKeys = new ConcurrentDictionary<string, bool>();
        }

        public Task<T> GetAsync<T>(string key, bool useUserPrefix = false)
        {
            _memoryCache.TryGetValue(key, out T value);
            return Task.FromResult(value);
        }

        public Task SetAsync<T>(string key, T value, TimeSpan? duration = null, bool useUserPrefix = false)
        {
            if (duration.HasValue)
            {
                _memoryCache.Set(key, value, duration.Value);
            }
            else
            {
                _memoryCache.Set(key, value);
            }
            _cacheKeys.TryAdd(key, true);
            return Task.CompletedTask;
        }

        public Task RemoveAsync(string key, bool useUserPrefix = false)
        {
            _memoryCache.Remove(key);
            return Task.CompletedTask;
        }

        public Task<bool> ExistsAsync(string key, bool useUserPrefix = false)
        {
            return Task.FromResult(_memoryCache.TryGetValue(key, out _));
        }
        public Task RemoveByPrefixAsync(string prefix)
        {
            var keysToRemove = _cacheKeys.Keys
                .Where(key => key.StartsWith(prefix))
                .ToList();

            foreach (var key in keysToRemove)
            {
                _memoryCache.Remove(key);
                _cacheKeys.TryRemove(key, out _);
            }

            return Task.CompletedTask;
        }
    }
}
