// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;

namespace Softeq.NetKit.Chat.Domain.Exceptions
{
    [Serializable]
    public class NetKitChatInvalidOperationException : NetKitChatException
    {
        public NetKitChatInvalidOperationException()
            : base(NetKitChatErrorCode.InvalidOperation)
        {
        }

        public NetKitChatInvalidOperationException(string message)
            : base(NetKitChatErrorCode.InvalidOperation, message)
        {
        }

        public NetKitChatInvalidOperationException(string message, Exception innerException)
            : base(NetKitChatErrorCode.InvalidOperation, message, innerException)
        {
        }
    }
}