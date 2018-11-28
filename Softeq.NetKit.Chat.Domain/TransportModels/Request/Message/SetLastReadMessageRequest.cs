// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;

namespace Softeq.NetKit.Chat.Domain.TransportModels.Request.Message
{
    public class SetLastReadMessageRequest
    {
        public SetLastReadMessageRequest(string saasUserId, Guid channelId, Guid messageId)
        {
            ChannelId = channelId;
            MessageId = messageId;
            SaasUserId = saasUserId;
        }

        public string SaasUserId { get; }

        public Guid ChannelId { get; }

        public Guid MessageId { get; }
    }
}