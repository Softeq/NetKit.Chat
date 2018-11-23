// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;

namespace Softeq.NetKit.Chat.Domain.Exceptions
{
    [Serializable]
    public abstract class NetKitChatException : Exception
    {
        protected NetKitChatException(NetKitChatErrorCode errorCode)
        {
            ErrorCode = errorCode;
        }

        protected NetKitChatException(NetKitChatErrorCode errorCode, string message)
            : base(message)
        {
            ErrorCode = errorCode;
        }

        protected NetKitChatException(NetKitChatErrorCode errorCode, string message, Exception innerException)
            : base(message, innerException)
        {
            ErrorCode = errorCode;
        }

        public NetKitChatErrorCode ErrorCode { get; }
    }
}