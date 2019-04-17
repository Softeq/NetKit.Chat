// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using Autofac;
using Microsoft.Extensions.Configuration;
using Softeq.CloudStorage.Extension;
using Softeq.NetKit.Chat.Data.Cloud.Azure.Configuration;
using Softeq.NetKit.Chat.Data.Cloud.Azure.DataProviders;
using Softeq.NetKit.Chat.Data.Cloud.DataProviders;

namespace Softeq.NetKit.Chat.Data.Cloud.Azure
{
    public class DataCloudAzureDiModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(x =>
                {
                    var context = x.Resolve<IComponentContext>();
                    var config = context.Resolve<IConfiguration>();
                    return new AzureCloudStorage(config["AzureStorage:ConnectionString"]);
                })
                .As<IContentStorage>();

            builder.Register(x =>
            {
                var context = x.Resolve<IComponentContext>();
                var config = context.Resolve<IConfiguration>();
                var cfg = new AzureStorageConfiguration(
                    config["AzureStorage:ContentStorageHost"],
                    config["AzureStorage:MessageAttachmentsContainer"],
                    config["AzureStorage:MemberAvatarsContainer"],
                    config["AzureStorage:ChannelImagesContainer"],
                    config["AzureStorage:TempContainerName"],
                    Convert.ToInt32(config["AzureStorage:MessagePhotoSize"]));
                return cfg;

            }).AsSelf();

            builder.RegisterType<CloudImageProvider>().As<ICloudImageProvider>();
            builder.RegisterType<CloudTokenProvider>().As<ICloudTokenProvider>();
            builder.RegisterType<CloudAttachmentProvider>().As<ICloudAttachmentProvider>();
        }
    }
}