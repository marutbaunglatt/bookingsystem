using StackExchange.Redis;
using System.Text.Json;

namespace BookingSystem1.Services
{
    public class RedisCache : IRedisCache
    {
        private IDatabase _cacheDb;
        private readonly IConfiguration _configuration;

        public RedisCache(IConfiguration configuration)
        {
            _configuration = configuration;
            var redis = ConnectionMultiplexer.Connect(_configuration["Redis:api_url"]);
            _cacheDb = redis.GetDatabase();
        }

        public async Task<T> GetData<T>(string key)
        {
            var value = await _cacheDb.StringGetAsync(key);
            if (!string.IsNullOrWhiteSpace(value))
            {
                return JsonSerializer.Deserialize<T>(value);
            }
            return default;
        }

        public async Task<object> RemoveData(string key)
        {
            var _exist = await _cacheDb.KeyExistsAsync(key);
            if (_exist)
            {
                return await _cacheDb.KeyDeleteAsync(key);
            }

            return false;
        }

        public async Task<bool> SetData<T>(string key, T value, DateTimeOffset expirationTime)
        {
            var expireTime = expirationTime.DateTime.Subtract(DateTime.Now);
            var isSet = await _cacheDb.StringSetAsync(key, JsonSerializer.Serialize(value), expireTime);
            return isSet;
        }
    }
}
