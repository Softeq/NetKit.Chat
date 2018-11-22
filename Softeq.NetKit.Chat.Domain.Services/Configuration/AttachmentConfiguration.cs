// Developed by Softeq Development Corporation
// http://www.softeq.com

using Microsoft.Extensions.Configuration;

namespace Softeq.NetKit.Chat.Domain.Services.Configuration
{
    internal class AttachmentConfiguration
    {
        public AttachmentConfiguration(IConfiguration configuration)
        {
            MessageAttachmentsLimit = configuration.GetValue<int>("MessageAttachments:Limit");
        }

        public int MessageAttachmentsLimit { get; }
    }
}