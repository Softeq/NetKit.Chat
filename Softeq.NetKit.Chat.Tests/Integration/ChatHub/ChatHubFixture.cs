// Developed by Softeq Development Corporation
// http://www.softeq.com

using System.Data.SqlClient;
using System.IO;
using Dapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;
using Softeq.NetKit.Chat.Data.Persistent.Sql.Database;
using Softeq.NetKit.Chat.Web;

namespace Softeq.NetKit.Chat.Tests.Integration.ChatHub
{
    public sealed class ChatHubFixture
    {
        public ChatHubFixture()
        {
            Configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .Build();

            var webHostBuilder = new WebHostBuilder()
                .UseConfiguration(Configuration)
                .ConfigureLogging(SetupLogger)
                .UseStartup<Startup>();

            Server = new TestServer(webHostBuilder);

            CleanUpDatabase();
        }

        public TestServer Server { get; }

        public IConfiguration Configuration { get; }

        private void SetupLogger(ILoggingBuilder obj)
        {
            const string template = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} {Level:u3} {EventId} {Message}{NewLine}{Exception}";
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Debug(outputTemplate: template)
                .WriteTo.ColoredConsole(outputTemplate: template)
                .CreateLogger();
        }

        private void CleanUpDatabase()
        {
            var sqlConnectionFactory = new SqlConnectionFactory(new SqlConnectionStringBuilder(Configuration["ConnectionStrings:DefaultConnection"]));

            using (var connection = sqlConnectionFactory.CreateConnection())
            {
                var sqlQuery = @"DELETE FROM Attachments
                                 DELETE FROM ChannelMembers
                                 DELETE FROM Clients
                                 DELETE FROM Notifications
                                 DELETE FROM Settings
                                 DELETE FROM ForwardMessages
                                 DELETE FROM Messages
                                 DELETE FROM Channels
                                 DELETE FROM Members";

                connection.ExecuteScalar(sqlQuery);
            }
        }
    }
}