// Developed by Softeq Development Corporation
// http://www.softeq.com

using Autofac;
using Microsoft.Extensions.Configuration;
using Softeq.CloudStorage.Extension;

namespace Softeq.NetKit.Chat.Web
{
    public class DIModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder
                .Register(x =>
                {
                    var context = x.Resolve<IComponentContext>();
                    var config = context.Resolve<IConfiguration>();
                    var storage = new AzureCloudStorage(config["AzureStorage:ConnectionString"]);
                    return storage;
                })
                .As<IContentStorage>();
        }
    }
}
