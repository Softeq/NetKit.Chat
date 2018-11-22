// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using Softeq.NetKit.Chat.Domain.TransportModels.Request.Member;

namespace Softeq.NetKit.Chat.Domain.TransportModels.Request.Channel
{
    public class ChannelRequest : UserRequest
    {
        public ChannelRequest(string saasUserId, Guid channelId)
            : base(saasUserId)
        {
            ChannelId = channelId;
        }

        public Guid ChannelId { get; set; }
    }
}