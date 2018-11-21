// Developed by Softeq Development Corporation
// http://www.softeq.com

using System.Data.SqlClient;

namespace Softeq.NetKit.Chat.Infrastructure.Storage.Sql.Database
{
    public interface ISqlConnectionFactory
    {
        SqlConnection CreateConnection();
    }
}