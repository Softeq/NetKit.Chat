// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using Softeq.NetKit.Chat.Domain.Member.TransportModels.Request;

namespace Softeq.NetKit.Chat.Domain.Message.TransportModels.Request
{
    public class AddLastReadMessageRequest : UserRequest
    {
        public AddLastReadMessageRequest(Guid channelId, Guid messageId, string saasUserId) : base(saasUserId)
        {
            ChannelId = channelId;
            MessageId = messageId;
            SaasUserId = saasUserId;
        }

        public Guid ChannelId { get; }
        public Guid MessageId { get; }
    }
}