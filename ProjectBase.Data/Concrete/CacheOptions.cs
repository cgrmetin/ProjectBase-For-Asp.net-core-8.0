using ProjectBase.Entity.Attributes;

namespace ProjectBase.Data.Concrete
{
    [IocContainerItem]
    public class CacheOptions
    {
        public bool UseRedis { get; set; } = false; // Default olarak Memory Cache
        public string RedisConnectionString { get; set; } = "localhost:6379"; // Redis için default
    }
}
