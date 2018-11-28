// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;

namespace Softeq.NetKit.Chat.Domain.TransportModels.Request.Message
{
    public class UpdateMessageRequest
    {
        public UpdateMessageRequest(string saasUserId, Guid messageId, string body)
        {
            SaasUserId = saasUserId;
            MessageId = messageId;
            Body = body;
        }

        public string SaasUserId { get; }

        public Guid MessageId { get; }

        public string Body { get; }
    }
}