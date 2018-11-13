using EnsureThat;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Softeq.NetKit.Chat.Common.Cache
{
    public class RedisCacheClient : IDistributedCacheClient
    {
        private static IConnectionMultiplexer Connection => _connection.Value;
        private static IDatabase CacheDb => Connection.GetDatabase();

        private static Lazy<IConnectionMultiplexer> _connection;

        public RedisCacheClient(string connectionString)
        {
            Ensure.That(connectionString, new ArgumentNullException().ToString())
                            .IsNotNullOrWhiteSpace();

            _connection = new Lazy<IConnectionMultiplexer>(() => ConnectionMultiplexer.Connect(connectionString));
        }

        public async Task<T> HashGetAsync<T>(string key, string field, Task<T> getValueFallbackTask = null) where T : class, new()
        {
            Ensure.That(key, new ArgumentNullException().ToString())
                           .IsNotNullOrWhiteSpace();
            Ensure.That(key, new ArgumentNullException().ToString())
                            .IsNotNullOrWhiteSpace();

            T result = null;

            var cacheValue = await CacheDb.HashGetAsync(key, field);
            if (!cacheValue.HasValue && getValueFallbackTask != null)
            {
                result = await getValueFallbackTask;
                await HashSetAsync(key, field, result);
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

            // todo: https://jira.softeq.com/browse/EMRA-957
            await CacheDb.HashSetAsync(key, field, serializedValue);
        }

        public async Task HashDeleteAsync(string key, string field)
        {
            Ensure.That(key, new ArgumentNullException().ToString())
                           .IsNotNullOrWhiteSpace();
            Ensure.That(key, new ArgumentNullException().ToString())
                           .IsNotNullOrWhiteSpace();

            await CacheDb.HashDeleteAsync(key, field);
        }

        public async Task RemoveKeyAsync(string key)
        {
             Ensure.That(key, new ArgumentNullException().ToString())
                            .IsNotNullOrWhiteSpace();

            await CacheDb.KeyDeleteAsync(key);
        }
    }
}
