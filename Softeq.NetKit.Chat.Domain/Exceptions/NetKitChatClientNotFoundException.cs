// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;

namespace Softeq.NetKit.Chat.Domain.Exceptions
{
    [Serializable]
    public class NetKitChatClientNotFoundException : NetKitChatException
    {
        public NetKitChatClientNotFoundException(string clientConnectionId)
            : base(NetKitChatErrorCode.ClientNotFound)
        {
            ClientConnectionId = clientConnectionId;
        }

        public NetKitChatClientNotFoundException(string clientConnectionId, string message)
            : base(NetKitChatErrorCode.ClientNotFound, message)
        {
            ClientConnectionId = clientConnectionId;
        }

        public NetKitChatClientNotFoundException(string clientConnectionId, string message, Exception innerException)
            : base(NetKitChatErrorCode.ClientNotFound, message, innerException)
        {
            ClientConnectionId = clientConnectionId;
        }

        public string ClientConnectionId { get; }
    }
}