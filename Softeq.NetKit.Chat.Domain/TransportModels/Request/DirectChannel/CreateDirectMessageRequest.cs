// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using Softeq.NetKit.Chat.Domain.DomainModels;

namespace Softeq.NetKit.Chat.Domain.TransportModels.Request.DirectChannel
{
   public class CreateDirectMessageRequest : UserRequest
    {
        public CreateDirectMessageRequest(string saasUserId, Guid directChannelId, MessageType type, string body) : base(saasUserId)
        {
            DirectChannelId = directChannelId;
            Type = type;
            Body = body;
        }

        public Guid DirectChannelId { get; }
        public string Body { get; }
        public MessageType Type { get; }
    }
}
