// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;

namespace Softeq.NetKit.Chat.Domain.Exceptions
{
    [Serializable]
    public class NetKitChatInsufficientRightsException : NetKitChatException
    {
        public NetKitChatInsufficientRightsException()
            : base(NetKitChatErrorCode.InsufficientRights)
        {
        }

        public NetKitChatInsufficientRightsException(string message)
            : base(NetKitChatErrorCode.InsufficientRights, message)
        {
        }

        public NetKitChatInsufficientRightsException(string message, Exception innerException)
            : base(NetKitChatErrorCode.InsufficientRights, message, innerException)
        {
        }
    }
}