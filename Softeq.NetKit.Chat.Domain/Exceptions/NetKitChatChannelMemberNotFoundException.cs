// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;

namespace Softeq.NetKit.Chat.Domain.Exceptions
{
    [Serializable]
    public class NetKitChatChannelMemberNotFoundException : NetKitChatException
    {
        public NetKitChatChannelMemberNotFoundException(Guid channelId, Guid memberId)
            : base(NetKitChatErrorCode.ChannelMemberNotFound)
        {
            ChannelId = channelId;
            MemberId = memberId;
        }

        public NetKitChatChannelMemberNotFoundException(Guid channelId, Guid memberId, string message)
            : base(NetKitChatErrorCode.ChannelMemberNotFound, message)
        {
            ChannelId = channelId;
            MemberId = memberId;
        }

        public NetKitChatChannelMemberNotFoundException(Guid channelId, Guid memberId, string message, Exception innerException)
            : base(NetKitChatErrorCode.ChannelMemberNotFound, message, innerException)
        {
            ChannelId = channelId;
            MemberId = memberId;
        }

        public Guid ChannelId { get; }

        public Guid MemberId { get; }
    }
}