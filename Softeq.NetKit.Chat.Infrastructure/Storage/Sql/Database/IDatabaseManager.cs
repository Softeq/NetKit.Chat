// Developed by Softeq Development Corporation
// http://www.softeq.com

using System.Threading.Tasks;

namespace Softeq.NetKit.Chat.Infrastructure.Storage.Sql.Database
{
    public interface IDatabaseManager
    {
        Task CreateEmptyDatabaseIfNotExistsAsync();
        void MigrateToLatestVersion();
    }
}