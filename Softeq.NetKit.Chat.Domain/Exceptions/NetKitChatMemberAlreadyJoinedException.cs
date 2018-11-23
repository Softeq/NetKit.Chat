// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;

namespace Softeq.NetKit.Chat.Domain.Exceptions
{
    [Serializable]
    public class NetKitChatMemberAlreadyJoinedException : NetKitChatException
    {
        public NetKitChatMemberAlreadyJoinedException(Guid memberId, Guid channelId)
            : base(NetKitChatErrorCode.MemberAlreadyJoined)
        {
            MemberId = memberId;
            ChannelId = channelId;
        }

        public NetKitChatMemberAlreadyJoinedException(Guid memberId, Guid channelId, string message)
            : base(NetKitChatErrorCode.MemberAlreadyJoined, message)
        {
            MemberId = memberId;
            ChannelId = channelId;
        }

        public NetKitChatMemberAlreadyJoinedException(Guid memberId, Guid channelId, string message, Exception innerException)
            : base(NetKitChatErrorCode.MemberAlreadyJoined, message, innerException)
        {
            MemberId = memberId;
            ChannelId = channelId;
        }

        public Guid MemberId { get; }

        public Guid ChannelId { get; }
    }
}