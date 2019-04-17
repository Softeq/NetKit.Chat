// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Data.SqlClient;
using Autofac;
using Microsoft.Extensions.Configuration;
using Softeq.NetKit.Chat.Data.Persistent.Database;
using Softeq.NetKit.Chat.Data.Persistent.Sql.Database;

namespace Softeq.NetKit.Chat.Data.Persistent.Sql
{
    public class DataPersistentSqlDiModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(x =>
            {
                var context = x.Resolve<IComponentContext>();
                var config = context.Resolve<IConfiguration>();
                return new SqlConnectionFactory(new SqlConnectionStringBuilder(config["Database:ConnectionString"]));
            }).As<ISqlConnectionFactory>();

            builder.Register(x =>
            {
                var context = x.Resolve<IComponentContext>();
                var config = context.Resolve<IConfiguration>();

                return new TransactionConfiguration
                {
                    TransactionTimeoutInMinutes = Convert.ToInt32(config["Database:TransactionConfiguration:TransactionTimeoutInMinutes"])
                };
            }).As<TransactionConfiguration>();

            builder.RegisterType<DatabaseManager>()
                .As<IDatabaseManager>();

            builder.Register(x => new UnitOfWork(x.Resolve<ISqlConnectionFactory>(), x.Resolve<TransactionConfiguration>()))
                .AsImplementedInterfaces();

            builder.RegisterType<DatabaseConfig>()
                .AsSelf()
                .SingleInstance();
        }
    }
}