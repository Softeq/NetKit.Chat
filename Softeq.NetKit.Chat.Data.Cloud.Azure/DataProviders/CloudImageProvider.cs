using System;
using System.Threading.Tasks;
using Softeq.CloudStorage.Extension;
using Softeq.NetKit.Chat.Data.Cloud.Azure.Configuration;
using Softeq.NetKit.Chat.Data.Cloud.DataProviders;

namespace Softeq.NetKit.Chat.Data.Cloud.Azure.DataProviders
{
    internal class CloudImageProvider : BaseCloudProvider, ICloudImageProvider
    {
        public CloudImageProvider(IContentStorage storage, AzureStorageConfiguration configuration)
            : base(storage, configuration)
        {
        }

        public async Task<string> CopyImageToDestinationContainerAsync(string photoUrl)
        {
            Uri permanentChannelImageUrl = null;

            if (!string.IsNullOrEmpty(photoUrl))
            {
                var fileName = photoUrl.Substring(photoUrl.LastIndexOf("/", StringComparison.Ordinal) + 1);

                permanentChannelImageUrl = await Storage.CopyBlobAsync(fileName, Configuration.TempContainerName, Configuration.ChannelImagesContainer);
            }

            return !string.IsNullOrEmpty(permanentChannelImageUrl?.ToString()) ? $"{permanentChannelImageUrl}" : null;
        }

        public string GetMemberAvatarUrl(string memberPhotoName)
        {
            return string.IsNullOrEmpty(memberPhotoName) ? null : $"{Configuration.ContentStorageHost}/{Configuration.MemberAvatarsContainer}/{memberPhotoName}";
        }
    }
}