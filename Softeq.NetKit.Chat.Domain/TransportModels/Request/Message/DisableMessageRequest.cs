// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;

namespace Softeq.NetKit.Chat.Domain.TransportModels.Request.Message
{
    public class DisableMessageRequest : UserRequest
    {
        public DisableMessageRequest(string saasUserId, Guid messageId)
            : base(saasUserId)
        {
            MessageId = messageId;
        }

        public Guid MessageId { get; }
    }
}