using System;
using Softeq.NetKit.Chat.Domain.DomainModels;

namespace Softeq.NetKit.Chat.SignalR.TransportModels.Request.Channel
{
    public class CreateDirectChannelRequest : BaseRequest
    {
        public Guid MemberId { get; set; }

        public ChannelType Type { get; } = ChannelType.Direct;
    }
}
