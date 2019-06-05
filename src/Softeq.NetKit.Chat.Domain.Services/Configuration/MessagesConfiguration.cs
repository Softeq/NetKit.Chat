// Developed by Softeq Development Corporation
// http://www.softeq.com

namespace Softeq.NetKit.Chat.Domain.Services.Configuration
{
    internal class MessagesConfiguration
    {
        public MessagesConfiguration(int messageAttachmentsLimit, int lastMessageReadCount)
        {
            MessageAttachmentsLimit = messageAttachmentsLimit;
            LastMessageReadCount = lastMessageReadCount;
        }

        public int MessageAttachmentsLimit { get; }

        public int LastMessageReadCount { get; }
    }
}
