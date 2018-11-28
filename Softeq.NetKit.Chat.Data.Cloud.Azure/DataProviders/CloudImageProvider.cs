using System;
using System.Threading.Tasks;
using EnsureThat;
using Softeq.CloudStorage.Extension;
using Softeq.NetKit.Chat.Data.Cloud.Azure.Configuration;
using Softeq.NetKit.Chat.Data.Cloud.DataProviders;

namespace Softeq.NetKit.Chat.Data.Cloud.Azure.DataProviders
{
    internal class CloudImageProvider : ICloudImageProvider
    {
        private readonly IContentStorage _contentStorage;
        private readonly AzureStorageConfiguration _storageConfiguration;

        public CloudImageProvider(IContentStorage contentStorage, AzureStorageConfiguration storageConfiguration)
        {
            Ensure.That(contentStorage).IsNotNull();
            Ensure.That(storageConfiguration).IsNotNull();

            _contentStorage = contentStorage;
            _storageConfiguration = storageConfiguration;
        }

        public async Task<string> CopyImageToDestinationContainerAsync(string photoUrl)
        {
            string permanentChannelImageUrl = null;

            if (!string.IsNullOrEmpty(photoUrl))
            {
                var fileName = photoUrl.Substring(photoUrl.LastIndexOf("/", StringComparison.Ordinal) + 1);

                permanentChannelImageUrl = await _contentStorage.CopyBlobAsync(fileName, _storageConfiguration.TempContainerName, _storageConfiguration.ChannelImagesContainer);
            }

            return permanentChannelImageUrl;
        }

        public string GetMemberAvatarUrl(string memberPhotoName)
        {
            return string.IsNullOrEmpty(memberPhotoName) ? null : $"{_storageConfiguration.ContentStorageHost}/{_storageConfiguration.MemberAvatarsContainer}/{memberPhotoName}";
        }
    }
}
