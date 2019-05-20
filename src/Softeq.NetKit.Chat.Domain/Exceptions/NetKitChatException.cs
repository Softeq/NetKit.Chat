// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;

namespace Softeq.NetKit.Chat.Domain.Exceptions
{
    [Serializable]
    public abstract class NetKitChatException : Exception
    {
        protected NetKitChatException(string errorCode)
        {
            ErrorCode = errorCode;
        }

        protected NetKitChatException(string errorCode, string message)
            : base(message)
        {
            ErrorCode = errorCode;
        }

        protected NetKitChatException(string errorCode, string message, Exception innerException)
            : base(message, innerException)
        {
            ErrorCode = errorCode;
        }

        public string ErrorCode { get; }

        protected static string ModifyMessage(string message, params string[] info)
        {
            return string.Format(message, info);
        }
    }
}