// // Developed by Softeq Development Corporation
// // http://www.softeq.com

using System.Threading.Tasks;

namespace Softeq.NetKit.Chat.Data.Repositories.Infrastructure
{
    public interface IDatabaseManager
    {
        Task CreateEmptyDatabaseIfNotExistsAsync();
        void MigrateToLatestVersion();
    }
}