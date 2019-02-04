// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;

namespace Softeq.NetKit.Chat.Domain.TransportModels.Request.Message
{
    public class ArchiveMessageRequest : UserRequest
    {
        public ArchiveMessageRequest(string saasUserId, Guid messageId)
            : base(saasUserId)
        {
            MessageId = messageId;
        }

        public Guid MessageId { get; }
    }
}