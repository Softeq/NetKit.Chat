// // Developed by Softeq Development Corporation
// // http://www.softeq.com

using System.Data.SqlClient;

namespace Softeq.NetKit.Chat.Data.Repositories.Infrastructure
{
    public class SqlConnectionFactory : ISqlConnectionFactory
    {
        private readonly SqlConnectionStringBuilder _sqlConnectionStringBuilder;

        public SqlConnectionFactory(SqlConnectionStringBuilder sqlConnectionStringBuilder)
        {
            _sqlConnectionStringBuilder = sqlConnectionStringBuilder;
        }

        public SqlConnection CreateConnection()
        {
            return new SqlConnection(_sqlConnectionStringBuilder.ConnectionString);
        }
    }
}