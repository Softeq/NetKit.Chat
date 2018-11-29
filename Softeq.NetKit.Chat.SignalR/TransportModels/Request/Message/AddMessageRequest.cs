// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using Softeq.NetKit.Chat.Domain.DomainModels;

namespace Softeq.NetKit.Chat.SignalR.TransportModels.Request.Message
{
    public abstract class AddMessageRequest : BaseRequest
    {
        public Guid ChannelId { get; set; }

        public string Body { get; set; }

        public MessageType Type { get; }

        // If Message type is Notification
        public string ImageUrl { get; set; }

        // If Message type is Forward
        public Guid ForwardedMessageId { get; set; }
    }
}