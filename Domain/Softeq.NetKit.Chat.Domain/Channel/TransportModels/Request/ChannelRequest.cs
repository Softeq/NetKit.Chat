// // Developed by Softeq Development Corporation
// // http://www.softeq.com

using System;
using Softeq.NetKit.Chat.Domain.Member.TransportModels.Request;

namespace Softeq.NetKit.Chat.Domain.Channel.TransportModels.Request
{
    public class ChannelRequest : UserRequest
    {
        public ChannelRequest(string saasUserId, Guid channelId) : base(saasUserId)
        {
            ChannelId = channelId;
        }

        public Guid ChannelId { get; set; }
    }
}