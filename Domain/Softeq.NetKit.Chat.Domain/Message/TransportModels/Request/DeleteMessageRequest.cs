// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using Softeq.NetKit.Chat.Domain.Member.TransportModels.Request;

namespace Softeq.NetKit.Chat.Domain.Message.TransportModels.Request
{
    public class DeleteMessageRequest : UserRequest
    {
        public DeleteMessageRequest(string saasUserId, Guid messageId) : base(saasUserId)
        {
            MessageId = messageId;
        }

        public Guid MessageId { get; set; }
    }
}