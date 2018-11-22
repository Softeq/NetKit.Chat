// Developed by Softeq Development Corporation
// http://www.softeq.com

using System.Data.SqlClient;

namespace Softeq.NetKit.Chat.Data.Persistent.Sql.Database
{
    public interface ISqlConnectionFactory
    {
        SqlConnection CreateConnection();
    }
}