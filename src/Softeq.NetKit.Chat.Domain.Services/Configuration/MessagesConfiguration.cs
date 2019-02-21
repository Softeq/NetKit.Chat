// Developed by Softeq Development Corporation
// http://www.softeq.com

using Microsoft.Extensions.Configuration;

namespace Softeq.NetKit.Chat.Domain.Services.Configuration
{
    internal class MessagesConfiguration
    {
        public MessagesConfiguration(IConfiguration configuration)
        {
            MessageAttachmentsLimit = configuration.GetValue<int>("Message:MessageAttachmentsLimit");
            LastMessageReadCount = configuration.GetValue<int>("Message:LastMessageReadCount");
        }

        public int MessageAttachmentsLimit { get; }

        public int LastMessageReadCount { get; }
    }
}