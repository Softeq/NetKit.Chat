// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;

namespace Softeq.NetKit.Chat.Domain.Exceptions
{
    [Serializable]
    public class NetKitChatMessageOwnerRequiredException : NetKitChatException
    {
        public NetKitChatMessageOwnerRequiredException(Guid messageId)
            : base(NetKitChatErrorCode.MessageOwnerRequired)
        {
            MessageId = messageId;
        }

        public NetKitChatMessageOwnerRequiredException(Guid messageId, string message)
            : base(NetKitChatErrorCode.MessageOwnerRequired, message)
        {
            MessageId = messageId;
        }

        public NetKitChatMessageOwnerRequiredException(Guid messageId, string message, Exception innerException)
            : base(NetKitChatErrorCode.MessageOwnerRequired, message, innerException)
        {
            MessageId = messageId;
        }

        public Guid MessageId { get; }
    }
}