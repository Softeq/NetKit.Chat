// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using Softeq.NetKit.Chat.Domain.Member.TransportModels.Request;

namespace Softeq.NetKit.Chat.Domain.Message.TransportModels.Request
{
    public class DeleteMessageAttachmentRequest : UserRequest
    {
        public DeleteMessageAttachmentRequest(string saasUserId, Guid messageId, Guid attachmentId) : base(saasUserId)
        {
            MessageId = messageId;
            AttachmentId = attachmentId;
        }

        public Guid MessageId { get; set; }
        public Guid AttachmentId { get; set; }
    }
}