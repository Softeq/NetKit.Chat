// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Threading.Tasks;
using EnsureThat;
using Softeq.CloudStorage.Extension;
using Softeq.NetKit.Chat.Data.Cloud.Azure.Configuration;
using Softeq.NetKit.Chat.Data.Cloud.DataProviders;

namespace Softeq.NetKit.Chat.Data.Cloud.Azure.DataProviders
{
    internal class CloudTokenProvider : ICloudTokenProvider
    {
        private readonly IContentStorage _contentStorage;
        private readonly AzureStorageConfiguration _storageConfiguration;

        public CloudTokenProvider(IContentStorage contentStorage, AzureStorageConfiguration storageConfiguration)
        {
            Ensure.That(contentStorage).IsNotNull();
            Ensure.That(storageConfiguration).IsNotNull();

            _contentStorage = contentStorage;
            _storageConfiguration = storageConfiguration;
        }

        public async Task<string> GetTemporaryStorageAccessTokenAsync(int expirationTimeMinutes)
        {
            return await _contentStorage.GetContainerSasTokenAsync(_storageConfiguration.TempContainerName, expirationTimeMinutes);
        }
    }
}