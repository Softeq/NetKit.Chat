using EnsureThat;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Softeq.NetKit.Chat.Common.Cache
{
    public class RedisCacheClient : IDistributedCacheClient
    {
        private static IConnectionMultiplexer Connection => _connection.Value;
        private static IDatabase CacheDb => Connection.GetDatabase();

        private static Lazy<IConnectionMultiplexer> _connection;
        private string _fieldName;

        public RedisCacheClient(IConfiguration configuration)
        {
            string connectionString = configuration["RedisCache:connectionString"];
            Ensure.That(connectionString, new ArgumentNullException().ToString())
                            .IsNotNullOrWhiteSpace();

            _connection = new Lazy<IConnectionMultiplexer>(() => ConnectionMultiplexer.Connect(connectionString));
            _fieldName = nameof(RedisCacheClient);
        }

        public async Task<T> HashGetAsync<T>(string key, string field, Task<T> getValueFallbackTask = null) where T : class, new()
        {
            Ensure.That(key, new ArgumentNullException().ToString())
                           .IsNotNullOrWhiteSpace();
            Ensure.That(key, new ArgumentNullException().ToString())
                            .IsNotNullOrWhiteSpace();

            T result = null;

            var cacheValue = await CacheDb.HashGetAsync(_fieldName, key);
            if (!cacheValue.HasValue && getValueFallbackTask != null)
            {
                result = await getValueFallbackTask;
                await HashSetAsync(key, _fieldName, result);
            }
            else if (cacheValue.HasValue)
            {
                result = JsonConvert.DeserializeObject<T>(cacheValue);
            }

            return result;
        }

        public IEnumerable<string> HashGetFields(string key)
        {
            Ensure.That(key, new ArgumentNullException().ToString())
                            .IsNotNullOrWhiteSpace();

            return CacheDb.HashKeys(key).ToStringArray();
        }

        public async Task HashSetAsync<T>(string key, string field, T value) where T : class, new()
        {
            Ensure.That(key, new ArgumentNullException().ToString())
                            .IsNotNullOrWhiteSpace();
            Ensure.That(field, new ArgumentNullException().ToString())
                            .IsNotNullOrWhiteSpace();
            Ensure.That(value, new ArgumentNullException().ToString())
                           .IsNotNull();

            var serializedValue = JsonConvert.SerializeObject(value);
           
            await CacheDb.HashSetAsync(_fieldName, key, serializedValue);
        }

        public async Task HashDeleteAsync(string key, string field)
        {
            Ensure.That(key, new ArgumentNullException().ToString())
                           .IsNotNullOrWhiteSpace();
            Ensure.That(key, new ArgumentNullException().ToString())
                           .IsNotNullOrWhiteSpace();

            await CacheDb.HashDeleteAsync(key, _fieldName);
        }

        public async Task RemoveKeyAsync(string key)
        {
             Ensure.That(key, new ArgumentNullException().ToString())
                            .IsNotNullOrWhiteSpace();

            await CacheDb.KeyDeleteAsync(key);
        }
    }
}
