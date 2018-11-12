// // Developed by Softeq Development Corporation
// // http://www.softeq.com

using System;
using Softeq.NetKit.Chat.Domain.Member.TransportModels.Request;

namespace Softeq.NetKit.Chat.Domain.Message.TransportModels.Request
{
    public class UpdateMessageRequest : UserRequest
    {
        public UpdateMessageRequest(string saasUserId, Guid messageId, string body) : base(saasUserId)
        {
            MessageId = messageId;
            Body = body;
        }

        public Guid MessageId { get; set; }
        public string Body { get; set; }
    }
}