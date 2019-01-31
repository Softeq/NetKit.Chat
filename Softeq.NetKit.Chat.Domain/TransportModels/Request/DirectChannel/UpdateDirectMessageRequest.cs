// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;

namespace Softeq.NetKit.Chat.Domain.TransportModels.Request.DirectChannel
{
    public class UpdateDirectMessageRequest : UserRequest
    {
        public UpdateDirectMessageRequest(string saasUserId, Guid messageId, Guid directChannelId, string body) 
            : base(saasUserId)
        {
            MessageId = messageId;
            DirectChannelId = directChannelId;
            Body = body;
        }

        public Guid MessageId { get; set; }
        public Guid DirectChannelId { get; set; }
        public string Body { get; set; }
    }
}
