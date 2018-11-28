// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;

namespace Softeq.NetKit.Chat.Domain.TransportModels.Request.MessageAttachment
{
    public class DeleteMessageAttachmentRequest
    {
        public DeleteMessageAttachmentRequest(string saasUserId, Guid messageId, Guid attachmentId)
        {
            SaasUserId = saasUserId;
            MessageId = messageId;
            AttachmentId = attachmentId;
        }

        public string SaasUserId { get; }

        public Guid MessageId { get; }

        public Guid AttachmentId { get; }
    }
}