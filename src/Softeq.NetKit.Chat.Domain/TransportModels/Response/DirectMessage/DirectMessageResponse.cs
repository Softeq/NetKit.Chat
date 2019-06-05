// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using Softeq.NetKit.Chat.Client.SDK.Enums;

namespace Softeq.NetKit.Chat.Domain.TransportModels.Response.DirectMessage
{
    public class DirectMessageResponse
    {
        public Guid Id { get; set; }
        public Guid DirectChannelId { get; set; }
        public DateTimeOffset Created { get; set; }
        public DomainModels.Member Owner { get; set; }
        public string Body { get; set; }
        public DateTimeOffset? Updated { get; set; }
        public MessageType Type { get; set; }
    }
}
