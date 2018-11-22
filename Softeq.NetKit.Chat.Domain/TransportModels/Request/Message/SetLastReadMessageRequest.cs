// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using Softeq.NetKit.Chat.Domain.TransportModels.Request.Member;

namespace Softeq.NetKit.Chat.Domain.TransportModels.Request.Message
{
    public class SetLastReadMessageRequest : UserRequest
    {
        public SetLastReadMessageRequest(Guid channelId, Guid messageId, string saasUserId)
            : base(saasUserId)
        {
            ChannelId = channelId;
            MessageId = messageId;
            SaasUserId = saasUserId;
        }

        public Guid ChannelId { get; }
        public Guid MessageId { get; }
    }
}