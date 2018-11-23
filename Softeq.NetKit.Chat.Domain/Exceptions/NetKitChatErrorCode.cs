// Developed by Softeq Development Corporation
// http://www.softeq.com

namespace Softeq.NetKit.Chat.Domain.Exceptions
{
    public enum NetKitChatErrorCode
    {
        MemberNotFound = 1,
        ChannelNotFound = 2,
        ClientNotFound = 3,
        MessageNotFound = 4,
        AttachmentNotFound = 5,
        AttachmentLimitExceeded = 6,
        MessageOwnerRequired = 7,
        ChannelOwnerRequired = 8,
        ChannelMemberNotFound = 9,
        MemberAlreadyJoined = 10,
        MemberNotJoined = 11,
        InsufficientRights = 12
    }
}