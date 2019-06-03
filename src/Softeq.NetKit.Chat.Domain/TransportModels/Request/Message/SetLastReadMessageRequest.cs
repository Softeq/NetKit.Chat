// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;

namespace Softeq.NetKit.Chat.Domain.TransportModels.Request.Message
{
    public class SetLastReadMessageRequest : UserRequest
    {
        public SetLastReadMessageRequest(string saasUserId, Guid channelId, Guid messageId)
            : base(saasUserId)
        {
            ChannelId = channelId;
            MessageId = messageId;
        }

        public Guid ChannelId { get; }

        public Guid MessageId { get; }
    }
}