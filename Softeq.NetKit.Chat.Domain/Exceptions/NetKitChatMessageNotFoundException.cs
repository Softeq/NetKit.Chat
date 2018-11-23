// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;

namespace Softeq.NetKit.Chat.Domain.Exceptions
{
    [Serializable]
    public class NetKitChatMessageNotFoundException : NetKitChatException
    {
        public NetKitChatMessageNotFoundException(Guid messageId)
            : base(NetKitChatErrorCode.MessageNotFound)
        {
            MessageId = messageId;
        }

        public NetKitChatMessageNotFoundException(Guid messageId, string message)
            : base(NetKitChatErrorCode.MessageNotFound, message)
        {
            MessageId = messageId;
        }

        public NetKitChatMessageNotFoundException(Guid messageId, string message, Exception innerException)
            : base(NetKitChatErrorCode.MessageNotFound, message, innerException)
        {
            MessageId = messageId;
        }

        public Guid MessageId { get; }
    }
}