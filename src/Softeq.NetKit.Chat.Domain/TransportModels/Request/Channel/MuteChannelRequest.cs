using System;

namespace Softeq.NetKit.Chat.Domain.TransportModels.Request.Channel
{
    public class MuteChannelRequest
    {
        public string SaasUserId { get; set; }
        public Guid ChannelId { get; set; }
        public bool IsMuted { get; set; }
    }
}
