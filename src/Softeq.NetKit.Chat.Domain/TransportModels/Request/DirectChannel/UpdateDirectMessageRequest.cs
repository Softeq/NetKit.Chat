// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using Softeq.NetKit.Chat.Domain.DomainModels;

namespace Softeq.NetKit.Chat.Domain.TransportModels.Request.DirectChannel
{
    public class UpdateDirectMessageRequest : UserRequest
    {
        public UpdateDirectMessageRequest(string saasUserId, Guid messageId, Guid directChannelId, MessageType type, string body) 
            : base(saasUserId)
        {
            MessageId = messageId;
            DirectChannelId = directChannelId;
            Body = body;
            Type = type;
        }

        public Guid MessageId { get; set; }
        public Guid DirectChannelId { get; set; }
        public string Body { get; set; }
        public MessageType Type { get; set; }
    }
}
