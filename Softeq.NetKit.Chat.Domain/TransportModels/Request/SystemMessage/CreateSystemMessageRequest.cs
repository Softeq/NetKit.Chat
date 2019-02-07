// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;

namespace Softeq.NetKit.Chat.Domain.TransportModels.Request.SystemMessage
{
    public class CreateSystemMessageRequest
    {
        public Guid ChannelId { get; set; }
        public string Body { get; set; }
    }
}
