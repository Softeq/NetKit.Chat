// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;

namespace Softeq.NetKit.Chat.Domain.Exceptions
{
    [Serializable]
    public class NetKitChatAttachmentNotFoundException : NetKitChatException
    {
        public NetKitChatAttachmentNotFoundException(Guid attachmentId)
            : base(NetKitChatErrorCode.AttachmentNotFound)
        {
            AttachmentId = attachmentId;
        }

        public NetKitChatAttachmentNotFoundException(Guid attachmentId, string message)
            : base(NetKitChatErrorCode.AttachmentNotFound, message)
        {
            AttachmentId = attachmentId;
        }

        public NetKitChatAttachmentNotFoundException(Guid attachmentId, string message, Exception innerException)
            : base(NetKitChatErrorCode.AttachmentNotFound, message, innerException)
        {
            AttachmentId = attachmentId;
        }

        public Guid AttachmentId { get; }
    }
}