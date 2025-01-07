using ProjectBase.Data.Abstract;
using ProjectBase.Entity.Attributes;
using Microsoft.AspNetCore.Http;

namespace ProjectBase.Core.Concrete
{
    [IocContainerItem]
    public class CacheManager : ICacheRepository
    {
        private readonly ICacheRepository _cacheService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CacheManager(ICacheRepository cacheService, IHttpContextAccessor httpContextAccessor)
        {
            _cacheService = cacheService;
            _httpContextAccessor = httpContextAccessor;
        }

        private string AddUserPrefix(string key, bool useUserPrefix)
        {
            if (!useUserPrefix) return key;
            var userName = _httpContextAccessor.HttpContext?.User?.Identity?.Name;
            return string.IsNullOrEmpty(userName) ? key : $"{userName}-{key}";
        }

        public async Task<T> GetAsync<T>(string key, bool useUserPrefix = false)
        {
            var result = await _cacheService.GetAsync<T>(AddUserPrefix(key, useUserPrefix));
            return result == null ? default : result;
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? duration = null, bool useUserPrefix = false)
        {
            await _cacheService.SetAsync(AddUserPrefix(key, useUserPrefix), value, duration);
        }

        public async Task RemoveAsync(string key, bool useUserPrefix = false)
        {
            await _cacheService.RemoveAsync(AddUserPrefix(key, useUserPrefix));
        }

        public async Task<bool> ExistsAsync(string key, bool useUserPrefix = false)
        {
            return await _cacheService.ExistsAsync(AddUserPrefix(key, useUserPrefix));
        }
        public async Task RemoveByPrefixAsync(string prefix)
        {
            await _cacheService.RemoveByPrefixAsync(prefix);
        }
    }
}
