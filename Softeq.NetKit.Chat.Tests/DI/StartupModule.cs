// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Softeq.CloudStorage.Extension;
using Softeq.NetKit.Chat.Domain.Services.Configuration;

namespace Softeq.NetKit.Chat.Tests.DI
{
    public class StartupModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var configurationRoot = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            builder.RegisterInstance(configurationRoot)
                .As<IConfigurationRoot>()
                .As<IConfiguration>();

            builder.Register(x =>
                {
                    var storage = new AzureCloudStorage(configurationRoot["AzureStorage:ConnectionString"]);
                    return storage;
                })
                .As<IContentStorage>();

            builder.RegisterType<CloudStorageConfiguration>().AsSelf();
            builder.RegisterType<AttachmentConfiguration>().AsSelf();

            builder.Register(context =>
                {
                    var lifetimeScope = context.Resolve<ILifetimeScope>();
                    return new AutofacServiceProvider(lifetimeScope);
                })
                .As<IServiceProvider>()
                .SingleInstance();
        }
    }
}