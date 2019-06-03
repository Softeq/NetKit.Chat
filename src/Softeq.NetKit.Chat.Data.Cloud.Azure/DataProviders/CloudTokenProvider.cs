// Developed by Softeq Development Corporation
// http://www.softeq.com

using System.Threading.Tasks;
using Softeq.CloudStorage.Extension;
using Softeq.NetKit.Chat.Data.Cloud.Azure.Configuration;
using Softeq.NetKit.Chat.Data.Cloud.DataProviders;

namespace Softeq.NetKit.Chat.Data.Cloud.Azure.DataProviders
{
    internal class CloudTokenProvider : BaseCloudProvider, ICloudTokenProvider
    {
        public CloudTokenProvider(IContentStorage contentStorage, AzureStorageConfiguration storageConfiguration)
            : base(contentStorage, storageConfiguration)
        {
        }

        public async Task<string> GetTemporaryStorageAccessTokenAsync(int expirationTimeMinutes)
        {
            return await Storage.GetContainerSasTokenAsync(Configuration.TempContainerName, expirationTimeMinutes);
        }
    }
}