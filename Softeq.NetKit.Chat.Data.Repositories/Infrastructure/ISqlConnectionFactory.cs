// Developed by Softeq Development Corporation
// http://www.softeq.com

using System.Data.SqlClient;

namespace Softeq.NetKit.Chat.Data.Repositories.Infrastructure
{
    public interface ISqlConnectionFactory
    {
        SqlConnection CreateConnection();
    }
}