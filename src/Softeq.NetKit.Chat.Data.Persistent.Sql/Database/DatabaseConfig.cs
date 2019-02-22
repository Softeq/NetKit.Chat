// Developed by Softeq Development Corporation
// http://www.softeq.com

using Microsoft.Extensions.Configuration;

namespace Softeq.NetKit.Chat.Data.Persistent.Sql.Database
{
    public class DatabaseConfig
    {
        public DatabaseConfig(IConfiguration configuration)
        {
            ConnectionString = configuration["Database:ConnectionString"];
        }

        public string ConnectionString { get; }
    }
}