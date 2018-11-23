// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;

namespace Softeq.NetKit.Chat.Domain.Exceptions
{
    [Serializable]
    public class NetKitChatInsufficientRightsException : NetKitChatException
    {
        public NetKitChatInsufficientRightsException(Guid memberId, Guid channelId)
            : base(NetKitChatErrorCode.InsufficientRights)
        {
            MemberId = memberId;
            ChannelId = channelId;
        }

        public NetKitChatInsufficientRightsException(Guid memberId, Guid channelId, string message)
            : base(NetKitChatErrorCode.InsufficientRights, message)
        {
            MemberId = memberId;
            ChannelId = channelId;
        }

        public NetKitChatInsufficientRightsException(Guid memberId, Guid channelId, string message, Exception innerException)
            : base(NetKitChatErrorCode.InsufficientRights, message, innerException)
        {
            MemberId = memberId;
            ChannelId = channelId;
        }

        public Guid MemberId { get; }

        public Guid ChannelId { get; }
    }
}