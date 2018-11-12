// // Developed by Softeq Development Corporation
// // http://www.softeq.com

using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Softeq.CloudStorage.Extension;
using Softeq.NetKit.Chat.Domain.Services.App.Configuration;

namespace Softeq.NetKit.Chat.Tests.DI
{
    public class StartupModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var configurationRoot = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            builder.RegisterInstance(
                    configurationRoot)
                .As<IConfigurationRoot>();

            builder.Register(x =>
                {
                    var storage = new AzureCloudStorage(configurationRoot["AzureStorage:ConnectionString"]);
                    return storage;
                })
                .As<IContentStorage>();

            builder.Register(x =>
            {
                var cfg = new CloudStorageConfiguration(
                    configurationRoot["AzureStorage:ContentStorageHost"],
                    configurationRoot["AzureStorage:MessageAttachmentsContainer"],
                    configurationRoot["AzureStorage:MemberAvatarsContainer"],
                    configurationRoot["AzureStorage:ChannelImagesContainer"],
                    configurationRoot["AzureStorage:TempContainerName"],
                    Convert.ToInt32(configurationRoot["AzureStorage:MessagePhotoSize"]));
                return cfg;
            });

            builder.Register(x =>
            {
                var cfg = new AttachmentConfiguration(Convert.ToInt32(configurationRoot["MessageAttachments:Limit"]));
                return cfg;
            });

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