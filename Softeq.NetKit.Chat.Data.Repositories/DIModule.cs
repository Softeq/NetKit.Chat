// // Developed by Softeq Development Corporation
// // http://www.softeq.com

using System.Data.SqlClient;
using Autofac;
using Microsoft.Extensions.Configuration;
using Softeq.NetKit.Chat.Data.Repositories.Infrastructure;

namespace Softeq.NetKit.Chat.Data.Repositories
{
    public class DIModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(x =>
            {
                var context = x.Resolve<IComponentContext>();
                var config = context.Resolve<IConfiguration>();
                return new SqlConnectionFactory(
                    new SqlConnectionStringBuilder(config["ConnectionStrings:DefaultConnection"]));
            }).As<ISqlConnectionFactory>();


            builder.RegisterType<DatabaseManager>()
                .As<IDatabaseManager>();

            builder.Register(x => new UnitOfWork(x.Resolve<ISqlConnectionFactory>()))
                .AsImplementedInterfaces();

            builder.RegisterType<DatabaseConfig>()
                .AsSelf()
                .SingleInstance();
        }
    }
}