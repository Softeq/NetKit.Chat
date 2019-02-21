// Developed by Softeq Development Corporation
// http://www.softeq.com

using EnsureThat;
using Softeq.CloudStorage.Extension;
using Softeq.NetKit.Chat.Data.Cloud.Azure.Configuration;

namespace Softeq.NetKit.Chat.Data.Cloud.Azure.DataProviders
{
    internal abstract class BaseCloudProvider
    {
        protected BaseCloudProvider(IContentStorage storage, AzureStorageConfiguration configuration)
        {
            Ensure.That(storage).IsNotNull();
            Ensure.That(configuration).IsNotNull();

            Storage = storage;
            Configuration = configuration;
        }

        protected IContentStorage Storage { get; }

        protected AzureStorageConfiguration Configuration { get; }
    }
}