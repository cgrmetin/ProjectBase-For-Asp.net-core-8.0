using ProjectBase.Data.Abstract;
using StackExchange.Redis;
using System.Text.Json;

namespace ProjectBase.Data.Concrete
{
    public class RedisCacheRepository : ICacheRepository
    {
        private readonly IConnectionMultiplexer _connectionMultiplexer;

        public RedisCacheRepository(IConnectionMultiplexer connectionMultiplexer)
        {
            _connectionMultiplexer = connectionMultiplexer;
        }

        public async Task<T> GetAsync<T>(string key, bool useUserPrefix = false)
        {
            var db = _connectionMultiplexer.GetDatabase();
            var value = await db.StringGetAsync(key);
            return value.IsNullOrEmpty ? default : JsonSerializer.Deserialize<T>(value);
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? duration = null, bool useUserPrefix = false)
        {
            var db = _connectionMultiplexer.GetDatabase();
            var serializedValue = JsonSerializer.Serialize(value);
            if (duration.HasValue)
            {
                await db.StringSetAsync(key, serializedValue, duration.Value);
            }
            else
            {
                await db.StringSetAsync(key, serializedValue);
            }
        }

        public async Task RemoveAsync(string key, bool useUserPrefix = false)
        {
            var db = _connectionMultiplexer.GetDatabase();
            await db.KeyDeleteAsync(key);
        }

        public async Task<bool> ExistsAsync(string key, bool useUserPrefix = false)
        {
            var db = _connectionMultiplexer.GetDatabase();
            return await db.KeyExistsAsync(key);
        }
        public async Task RemoveByPrefixAsync(string prefix)
        {
            var db = _connectionMultiplexer.GetDatabase();
            var server = _connectionMultiplexer.GetServer(_connectionMultiplexer.GetEndPoints().First());
            var keys = server.Keys(pattern: $"{prefix}*").ToArray();

            foreach (var key in keys)
            {
                await db.KeyDeleteAsync(key);
            }
        }
    }
}
