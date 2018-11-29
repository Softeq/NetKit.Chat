// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;

namespace Softeq.NetKit.Chat.SignalR.Dto.Request.Channel
{
    public class PinChannelRequest : BaseRequest
    {
        public Guid ChannelId { get; set; }

        public bool IsPinned { get; set; }
    }
}