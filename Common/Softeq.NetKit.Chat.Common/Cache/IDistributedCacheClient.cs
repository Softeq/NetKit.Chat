using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Softeq.NetKit.Chat.Common.Cache
{
    public interface IDistributedCacheClient
    {
        IEnumerable<string> HashGetFields(string key);
        Task<T> HashGetAsync<T>(string key, string field, Task<T> getValueFallbackTask = null) where T : class, new();
        Task HashSetAsync<T>(string key, string field, T value) where T : class, new();
        Task HashDeleteAsync(string key, string field);
        Task RemoveKeyAsync(string key);
    }
}
