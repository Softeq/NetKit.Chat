// // Developed by Softeq Development Corporation
// // http://www.softeq.com

using System;

namespace Softeq.NetKit.Chat.Domain.Channel.TransportModels.Request
{
    public class JoinToChannelRequest : ChannelRequest
    {
        public JoinToChannelRequest(string saasUserId, Guid channelId) : base(saasUserId, channelId)
        {
        }
    }
}