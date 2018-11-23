// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;

namespace Softeq.NetKit.Chat.Domain.Exceptions
{
    [Serializable]
    public class NetKitChatMemberNotJoinedException : NetKitChatException
    {
        public NetKitChatMemberNotJoinedException(Guid memberId, Guid channelId)
            : base(NetKitChatErrorCode.MemberNotJoined)
        {
            MemberId = memberId;
            ChannelId = channelId;
        }

        public NetKitChatMemberNotJoinedException(Guid memberId, Guid channelId, string message)
            : base(NetKitChatErrorCode.MemberNotJoined, message)
        {
            MemberId = memberId;
            ChannelId = channelId;
        }

        public NetKitChatMemberNotJoinedException(Guid memberId, Guid channelId, string message, Exception innerException)
            : base(NetKitChatErrorCode.MemberNotJoined, message, innerException)
        {
            MemberId = memberId;
            ChannelId = channelId;
        }

        public Guid MemberId { get; }

        public Guid ChannelId { get; }
    }
}