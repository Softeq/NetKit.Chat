// Developed by Softeq Development Corporation
// http://www.softeq.com

using EnsureThat;
using Softeq.NetKit.Chat.Data.Persistent.Sql.Database;

namespace Softeq.NetKit.Chat.Data.Persistent.Sql.Repositories
{
    public abstract class BaseRepository
    {
        protected readonly ISqlConnectionFactory _sqlConnectionFactory;

        protected BaseRepository(ISqlConnectionFactory sqlConnectionFactory)
        {
            Ensure.That(sqlConnectionFactory).IsNotNull();

            _sqlConnectionFactory = sqlConnectionFactory;
        }
    }
}
