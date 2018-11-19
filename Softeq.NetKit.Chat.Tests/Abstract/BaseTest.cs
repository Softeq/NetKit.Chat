// Developed by Softeq Development Corporation
// http://www.softeq.com

using System.Threading.Tasks;
using Autofac;
using Dapper;
using Softeq.NetKit.Chat.Data.Repositories;
using Softeq.NetKit.Chat.Data.Repositories.Infrastructure;
using Softeq.NetKit.Chat.Domain.Base;

namespace Softeq.NetKit.Chat.Tests.Abstract
{
    public abstract class BaseTest : Disposable
    {
        protected readonly ILifetimeScope LifetimeScope;
        protected readonly IUnitOfWork UnitOfWork;

        protected BaseTest()
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterAssemblyModules(typeof(BaseTest).Assembly);
            LifetimeScope = containerBuilder.Build();
            UnitOfWork = LifetimeScope.Resolve<IUnitOfWork>();

            var databaseManager = LifetimeScope.Resolve<IDatabaseManager>();
            databaseManager.CreateEmptyDatabaseIfNotExistsAsync();
            databaseManager.MigrateToLatestVersion();

            CleanUpDatabase(LifetimeScope).GetAwaiter().GetResult();
        }

        protected override void DisposeCore()
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
                    DELETE FROM Attachments;
                    DELETE FROM ChannelMembers;
                    DELETE FROM Clients;
                    DELETE FROM Notifications;
                    DELETE FROM Settings;
                    DELETE FROM Messages;
                    DELETE FROM Channels;
                    DELETE FROM Members;";

                await connection.ExecuteScalarAsync(sqlQuery);
            }
        }
    }
}