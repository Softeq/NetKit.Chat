// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Threading.Tasks;
using Autofac;
using Dapper;
using Microsoft.Extensions.Configuration;
using Softeq.NetKit.Chat.Application.Services;
using Softeq.NetKit.Chat.Data.Cloud.Azure;
using Softeq.NetKit.Chat.Data.Persistent;
using Softeq.NetKit.Chat.Data.Persistent.Database;
using Softeq.NetKit.Chat.Data.Persistent.Sql;
using Softeq.NetKit.Chat.Data.Persistent.Sql.Database;
using Softeq.NetKit.Chat.Domain.Services;
using Softeq.NetKit.Chat.Web;

namespace Softeq.NetKit.Chat.Tests.Integration
{
    public abstract class BaseTest : IDisposable
    {
        protected readonly ILifetimeScope LifetimeScope;
        protected readonly IUnitOfWork UnitOfWork;

        protected BaseTest()
        {
            var builder = new ContainerBuilder();

            var configurationRoot = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            builder.RegisterInstance(configurationRoot)
                .As<IConfigurationRoot>()
                .As<IConfiguration>();

            builder.RegisterModule<ApplicationServicesDiModule>();
            builder.RegisterModule<DataPersistentSqlDiModule>();
            builder.RegisterModule<DataCloudAzureDiModule>();
            builder.RegisterModule<DomainServicesDiModule>();
            builder.RegisterModule<WebDiModule>();

            LifetimeScope = builder.Build();
            UnitOfWork = LifetimeScope.Resolve<IUnitOfWork>();

            var databaseManager = LifetimeScope.Resolve<IDatabaseManager>();
            databaseManager.CreateEmptyDatabaseIfNotExistsAsync();
            databaseManager.MigrateToLatestVersion();

            CleanUpDatabase(LifetimeScope).GetAwaiter().GetResult();
        }

        public void Dispose()
        {
            CleanUpDatabase(LifetimeScope).GetAwaiter().GetResult();
            LifetimeScope.Dispose();
        }

        private async Task CleanUpDatabase(ILifetimeScope lifetimeScope)
        {
            var sqlConnectionFactory = lifetimeScope.Resolve<ISqlConnectionFactory>();

            using (var connection = sqlConnectionFactory.CreateConnection())
            {
                await connection.OpenAsync();

                var sqlQuery = @"   
                    DELETE FROM DirectChannelSettings;
                    DELETE FROM DirectChannel;
                    DELETE FROM Attachments;
                    DELETE FROM ChannelMembers;
                    DELETE FROM Clients;
                    DELETE FROM Notifications;
                    DELETE FROM Settings;
                    DELETE FROM ForwardMessages;
                    DELETE FROM Messages;
                    DELETE FROM Channels;
                    DELETE FROM Members;
                    DELETE FROM NotificationSettings;";

                await connection.ExecuteScalarAsync(sqlQuery);
            }
        }
    }
}