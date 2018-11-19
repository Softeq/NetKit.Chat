// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using Softeq.NetKit.Chat.Domain.Member.TransportModels.Request;

namespace Softeq.NetKit.Chat.Domain.Message.TransportModels.Request
{
    public class CreateMessageRequest : UserRequest
    {
        public CreateMessageRequest(string saasUserId) : base(saasUserId)
        {
        }

        public string Body { get; set; }
        public Guid ChannelId { get; set; }
        public MessageType Type { get; set; }

        // If Message type is Notification
        public string ImageUrl { get; set; }
    }
}