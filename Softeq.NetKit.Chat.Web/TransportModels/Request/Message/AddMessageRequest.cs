// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using Softeq.NetKit.Chat.Domain.DomainModels;

namespace Softeq.NetKit.Chat.Web.TransportModels.Request.Message
{
    public class AddMessageRequest
    {
        public string ClientConnectionId { get; set; }

        public string Body { get; set; }

        public MessageType Type { get; set; }

        // If Message type is SystemNotification
        public string ImageUrl { get; set; }

        // If Message type is Forward
        public Guid ForwardedMessageId { get; set; }
    }
}