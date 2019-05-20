// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;

namespace Softeq.NetKit.Chat.Domain.Exceptions
{
    [Serializable]
    public class NetKitChatNotFoundException : NetKitChatException
    {
        public NetKitChatNotFoundException()
            : base(NetKitChatErrorCode.NotFound)
        {
        }

        public NetKitChatNotFoundException(string message)
            : base(NetKitChatErrorCode.NotFound, message)
        {
        }

        public NetKitChatNotFoundException(string message, params string[] info)
        : base(NetKitChatErrorCode.NotFound, ModifyMessage(message, info))
        {
        } 

        public NetKitChatNotFoundException(string message, Exception innerException)
            : base(NetKitChatErrorCode.NotFound, message, innerException)
        {
        }
    }
}