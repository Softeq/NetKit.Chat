// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using Softeq.NetKit.Chat.Domain.TransportModels.Request.Member;

namespace Softeq.NetKit.Chat.Domain.TransportModels.Request.Message
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