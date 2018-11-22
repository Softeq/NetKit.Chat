// Developed by Softeq Development Corporation
// http://www.softeq.com

using System.Data.SqlClient;
using Autofac;
using Microsoft.Extensions.Configuration;
using Softeq.NetKit.Chat.Data.Persistent;
using Softeq.NetKit.Chat.Data.Persistent.Database;
using Softeq.NetKit.Chat.Data.Persistent.Sql;
using Softeq.NetKit.Chat.Data.Persistent.Sql.Database;

namespace Softeq.NetKit.Chat.Tests.DI
{
    public class DbModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var configurationRoot = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            builder.RegisterInstance(
                    configurationRoot)
                .As<IConfigurationRoot>()
                .As<IConfiguration>();

            builder.RegisterType<DatabaseConfig>()
                .AsSelf()
                .SingleInstance();

            builder.Register(x => new SqlConnectionFactory(new SqlConnectionStringBuilder(configurationRoot["ConnectionStrings:DefaultConnection"])))
                .AsImplementedInterfaces();

            builder.RegisterType<DatabaseManager>()
                .As<IDatabaseManager>();

            builder.Register(x => new UnitOfWork(x.Resolve<ISqlConnectionFactory>()))
                .AsImplementedInterfaces();
        }
    }
}