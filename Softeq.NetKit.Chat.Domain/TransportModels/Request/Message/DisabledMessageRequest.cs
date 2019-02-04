// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;

namespace Softeq.NetKit.Chat.Domain.TransportModels.Request.Message
{
    public class DisabledMessageRequest : UserRequest
    {
        public DisabledMessageRequest(string saasUserId, Guid messageId)
            : base(saasUserId)
        {
            MessageId = messageId;
        }

        public Guid MessageId { get; }
    }
}