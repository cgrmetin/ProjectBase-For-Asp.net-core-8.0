using ProjectBase.Data.Abstract;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;


namespace ProjectBase.Data.Concrete
{
    public static class CacheServiceRegistration
    {
        public static void AddCacheServices(this IServiceCollection services, CacheOptions options)
        {
            if (options.UseRedis)
            {
                services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(options.RedisConnectionString));
                services.AddSingleton<ICacheRepository, RedisCacheRepository>();
            }
            else
            {
                services.AddMemoryCache();
                services.AddSingleton<ICacheRepository, MemoryCacheRepository>();
            }
        }
    }
}
