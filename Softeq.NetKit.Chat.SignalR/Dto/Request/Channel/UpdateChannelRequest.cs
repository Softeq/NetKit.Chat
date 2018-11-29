// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;

namespace Softeq.NetKit.Chat.SignalR.Dto.Request.Channel
{
    public class UpdateChannelRequest : BaseRequest
    {
        public Guid ChannelId { get; set; }

        public string Name { get; set; }

        public string Topic { get; set; }

        public string WelcomeMessage { get; set; }

        public string PhotoUrl { get; set; }
    }
}