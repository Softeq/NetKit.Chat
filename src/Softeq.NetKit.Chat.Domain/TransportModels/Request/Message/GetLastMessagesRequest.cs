// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;

namespace Softeq.NetKit.Chat.Domain.TransportModels.Request.Message
{
    public class GetLastMessagesRequest : UserRequest
    {
        public GetLastMessagesRequest(string saasUserId, Guid channelId)
            : base(saasUserId)
        {
            ChannelId = channelId;
        }

        public Guid ChannelId { get; }
    }
}