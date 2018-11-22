// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;

namespace Softeq.NetKit.Chat.Domain.TransportModels.Request.Channel
{
    public class JoinToChannelRequest : ChannelRequest
    {
        public JoinToChannelRequest(string saasUserId, Guid channelId)
            : base(saasUserId, channelId)
        {
        }
    }
}