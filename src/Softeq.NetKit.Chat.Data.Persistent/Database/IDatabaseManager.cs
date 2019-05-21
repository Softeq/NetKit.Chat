// Developed by Softeq Development Corporation
// http://www.softeq.com

using System.Threading.Tasks;

namespace Softeq.NetKit.Chat.Data.Persistent.Database
{
    public interface IDatabaseManager
    {
        Task CreateEmptyDatabaseIfNotExistsAsync();
        Task MigrateToLatestVersion();
    }
}