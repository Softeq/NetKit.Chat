// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;

namespace Softeq.NetKit.Chat.Domain.Exceptions
{
    [Serializable]
    public class NetKitChatAccessForbiddenException : NetKitChatException
    {
        public NetKitChatAccessForbiddenException()
            : base(NetKitChatErrorCode.AccessForbidden)
        {
        }

        public NetKitChatAccessForbiddenException(string message)
            : base(NetKitChatErrorCode.AccessForbidden, message)
        {
        }

        public NetKitChatAccessForbiddenException(string message, Exception innerException)
            : base(NetKitChatErrorCode.AccessForbidden, message, innerException)
        {
        }
    }
}