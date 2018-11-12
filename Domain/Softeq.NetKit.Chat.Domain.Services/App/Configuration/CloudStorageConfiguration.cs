// // Developed by Softeq Development Corporation
// // http://www.softeq.com

namespace Softeq.NetKit.Chat.Domain.Services.App.Configuration
{
    public class CloudStorageConfiguration
    {
        public CloudStorageConfiguration(
            string contentStorageHost,
            string messageAttachmentsContainer,
            string memberAvatarsContainer,
            string channelImagesContainer,
            string tempContainerName,
            int messagePhotoSize)
        {
            ContentStorageHost = contentStorageHost;
            MessageAttachmentsContainer = messageAttachmentsContainer;
            MemberAvatarsContainer = memberAvatarsContainer;
            MessagePhotoSize = messagePhotoSize;
            TempContainerName = tempContainerName;
            ChannelImagesContainer = channelImagesContainer;
        }

        public string ContentStorageHost { get; set; }
        public string MessageAttachmentsContainer { get; set; }
        public string MemberAvatarsContainer { get; set; }
        public string ChannelImagesContainer { get; set; }
        public string TempContainerName { get; set; }
        public int MessagePhotoSize { get; set; }

        public string GetUrl(string fileName, string container)
        {
            return string.IsNullOrWhiteSpace(fileName) ? null : ContentStorageHost + '/' + container + '/' + fileName;
        }
    }
}