// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;

namespace Softeq.NetKit.Chat.Domain.Exceptions
{
    [Serializable]
    public class NetKitChatChannelNotFoundException : NetKitChatException
    {
        public NetKitChatChannelNotFoundException(Guid channelId)
            : base(NetKitChatErrorCode.ChannelNotFound)
        {
            ChannelId = channelId;
        }

        public NetKitChatChannelNotFoundException(Guid channelId, string message)
            : base(NetKitChatErrorCode.ChannelNotFound, message)
        {
            ChannelId = channelId;
        }

        public NetKitChatChannelNotFoundException(Guid channelId, string message, Exception innerException)
            : base(NetKitChatErrorCode.ChannelNotFound, message, innerException)
        {
            ChannelId = channelId;
        }

        public Guid ChannelId { get; }
    }
}