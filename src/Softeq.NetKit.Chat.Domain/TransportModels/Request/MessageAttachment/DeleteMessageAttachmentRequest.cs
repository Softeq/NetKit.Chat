// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;

namespace Softeq.NetKit.Chat.Domain.TransportModels.Request.MessageAttachment
{
    public class DeleteMessageAttachmentRequest : UserRequest
    {
        public DeleteMessageAttachmentRequest(string saasUserId, Guid messageId, Guid attachmentId)
            : base(saasUserId)
        {
            MessageId = messageId;
            AttachmentId = attachmentId;
        }

        public Guid MessageId { get; }

        public Guid AttachmentId { get; }
    }
}