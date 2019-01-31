// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;

namespace Softeq.NetKit.Chat.Domain.TransportModels.Request.DirectChannel
{
   public class CreateDirectMessageRequest : UserRequest
    {
        public CreateDirectMessageRequest(string saasUserId, Guid directChannelId, string body) : base(saasUserId)
        {
            DirectChannelId = directChannelId;
            Body = body;
        }

        public Guid DirectChannelId { get; }
        public string Body { get; }
    }
}
