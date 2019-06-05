// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using Softeq.NetKit.Chat.Client.SDK.Enums;

namespace Softeq.NetKit.Chat.Domain.TransportModels.Request.Message
{
    public class CreateMessageRequest : UserRequest
    {
        public CreateMessageRequest(string saasUserId, Guid channelId, MessageType type, string body)
            : base(saasUserId)
        {
            ChannelId = channelId;
            Type = type;
            Body = body;
        }

        public Guid ChannelId { get; }

        public MessageType Type { get; }

        public string Body { get; }

        // If Message type is Notification
        public string ImageUrl { get; set; }

        // If Message type is Forward
        public Guid ForwardedMessageId { get; set; }
    }
}