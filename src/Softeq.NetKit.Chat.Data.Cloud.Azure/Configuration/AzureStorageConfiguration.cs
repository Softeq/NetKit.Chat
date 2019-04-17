// Developed by Softeq Development Corporation
// http://www.softeq.com

namespace Softeq.NetKit.Chat.Data.Cloud.Azure.Configuration
{
    public class AzureStorageConfiguration
    {
        public AzureStorageConfiguration(string storageHost, string messageAttachmentsContainer, string memberAvatarsContainer, string channelImagesContainer, string tempContainerName, int messagePhotoSize)
        {
            ContentStorageHost = storageHost;
            MessageAttachmentsContainer = messageAttachmentsContainer;
            MemberAvatarsContainer = memberAvatarsContainer;
            ChannelImagesContainer = channelImagesContainer;
            TempContainerName = tempContainerName;
            MessagePhotoSize = messagePhotoSize;
        }

        public string ContentStorageHost { get; }

        public string MessageAttachmentsContainer { get; }

        public string MemberAvatarsContainer { get; }

        public string ChannelImagesContainer { get; }

        public string TempContainerName { get; }

        public int MessagePhotoSize { get; }
    }
}