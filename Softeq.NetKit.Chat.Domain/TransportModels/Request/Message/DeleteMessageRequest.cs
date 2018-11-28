// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;

namespace Softeq.NetKit.Chat.Domain.TransportModels.Request.Message
{
    public class DeleteMessageRequest
    {
        public DeleteMessageRequest(string saasUserId, Guid messageId)
        {
            SaasUserId = saasUserId;
            MessageId = messageId;
        }

        public string SaasUserId { get; }

        public Guid MessageId { get; }
    }
}