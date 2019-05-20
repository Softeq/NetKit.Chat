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
        private const string ConnectionString = "AzureStorage:ConnectionString";
        private const string ContentStorageHost = "AzureStorage:ContentStorageHost";
        private const string MessageAttachmentsContainer = "AzureStorage:MessageAttachmentsContainer";
        private const string MemberAvatarsContainer = "AzureStorage:MemberAvatarsContainer";
        private const string ChannelImagesContainer = "AzureStorage:ChannelImagesContainer";
        private const string TempContainerName = "AzureStorage:TempContainerName";
        private const string MessagePhotoSize = "AzureStorage:MessagePhotoSize";

        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(x =>
                {
                    var context = x.Resolve<IComponentContext>();
                    var config = context.Resolve<IConfiguration>();
                    return new AzureCloudStorage(config[ConnectionString]);
                })
                .As<IContentStorage>();

            builder.Register(x =>
            {
                var context = x.Resolve<IComponentContext>();
                var config = context.Resolve<IConfiguration>();
                var cfg = new AzureStorageConfiguration(
                    config[ContentStorageHost],
                    config[MessageAttachmentsContainer],
                    config[MemberAvatarsContainer],
                    config[ChannelImagesContainer],
                    config[TempContainerName],
                    Convert.ToInt32(config[MessagePhotoSize]));
                return cfg;

            }).AsSelf();

            builder.RegisterType<CloudImageProvider>().As<ICloudImageProvider>();
            builder.RegisterType<CloudTokenProvider>().As<ICloudTokenProvider>();
            builder.RegisterType<CloudAttachmentProvider>().As<ICloudAttachmentProvider>();
        }
    }
}