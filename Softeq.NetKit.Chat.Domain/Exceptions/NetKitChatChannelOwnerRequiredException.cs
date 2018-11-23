// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;

namespace Softeq.NetKit.Chat.Domain.Exceptions
{
    [Serializable]
    public class NetKitChatChannelOwnerRequiredException : NetKitChatException
    {
        public NetKitChatChannelOwnerRequiredException(Guid channelId)
            : base(NetKitChatErrorCode.ChannelOwnerRequired)
        {
            ChannelId = channelId;
        }

        public NetKitChatChannelOwnerRequiredException(Guid channelId, string message)
            : base(NetKitChatErrorCode.ChannelOwnerRequired, message)
        {
            ChannelId = channelId;
        }

        public NetKitChatChannelOwnerRequiredException(Guid channelId, string message, Exception innerException)
            : base(NetKitChatErrorCode.ChannelOwnerRequired, message, innerException)
        {
            ChannelId = channelId;
        }

        public Guid ChannelId { get; }
    }
}