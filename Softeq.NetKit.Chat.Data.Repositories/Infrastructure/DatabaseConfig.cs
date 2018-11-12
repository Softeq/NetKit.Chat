// // Developed by Softeq Development Corporation
// // http://www.softeq.com

using Microsoft.Extensions.Configuration;

namespace Softeq.NetKit.Chat.Data.Repositories.Infrastructure
{
    public class DatabaseConfig
    {
        public DatabaseConfig(IConfiguration configuration)
        {
            ConnectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public string ConnectionString { get; }
    }
}