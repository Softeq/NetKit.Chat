// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;

namespace Softeq.NetKit.Chat.Domain.Exceptions
{
    [Serializable]
    public class NetKitChatAttachmentLimitExceededException : NetKitChatException
    {
        public NetKitChatAttachmentLimitExceededException(int messageAttachmentsLimit)
            : base(NetKitChatErrorCode.AttachmentLimitExceeded)
        {
            MessageAttachmentsLimit = messageAttachmentsLimit;
        }

        public NetKitChatAttachmentLimitExceededException(int messageAttachmentsLimit, string message)
            : base(NetKitChatErrorCode.AttachmentLimitExceeded, message)
        {
            MessageAttachmentsLimit = messageAttachmentsLimit;
        }

        public NetKitChatAttachmentLimitExceededException(int messageAttachmentsLimit, string message, Exception innerException)
            : base(NetKitChatErrorCode.AttachmentLimitExceeded, message, innerException)
        {
            MessageAttachmentsLimit = messageAttachmentsLimit;
        }

        public int MessageAttachmentsLimit { get; }
    }
}