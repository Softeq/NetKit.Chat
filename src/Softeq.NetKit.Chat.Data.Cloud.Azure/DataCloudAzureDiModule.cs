// Developed by Softeq Development Corporation
// http://www.softeq.com

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

            builder.RegisterType<AzureStorageConfiguration>().AsSelf();

            builder.RegisterType<CloudImageProvider>().As<ICloudImageProvider>();
            builder.RegisterType<CloudTokenProvider>().As<ICloudTokenProvider>();
            builder.RegisterType<CloudAttachmentProvider>().As<ICloudAttachmentProvider>();
        }
    }
}