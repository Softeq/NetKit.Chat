// Developed by Softeq Development Corporation
// http://www.softeq.com

using Microsoft.Extensions.Configuration;

namespace Softeq.NetKit.Chat.Domain.Services.App.Configuration
{
    public class CloudStorageConfiguration
    {
        public CloudStorageConfiguration(IConfiguration configuration)
        {
            ContentStorageHost = configuration["AzureStorage:ContentStorageHost"];
            MessageAttachmentsContainer = configuration["AzureStorage:MessageAttachmentsContainer"];
            MemberAvatarsContainer = configuration["AzureStorage:MemberAvatarsContainer"];
            ChannelImagesContainer = configuration["AzureStorage:ChannelImagesContainer"];
            TempContainerName = configuration["AzureStorage:TempContainerName"];
            MessagePhotoSize = configuration.GetValue<int>("AzureStorage:MessagePhotoSize");
        }

        public string ContentStorageHost { get; }

        public string MessageAttachmentsContainer { get; }

        public string MemberAvatarsContainer { get; }

        public string ChannelImagesContainer { get; }

        public string TempContainerName { get; }

        public int MessagePhotoSize { get; }
    }
}