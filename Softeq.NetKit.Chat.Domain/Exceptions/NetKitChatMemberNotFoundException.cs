// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;

namespace Softeq.NetKit.Chat.Domain.Exceptions
{
    [Serializable]
    public class NetKitChatMemberNotFoundException : NetKitChatException
    {
        public NetKitChatMemberNotFoundException(Guid memberId)
            : base(NetKitChatErrorCode.MemberNotFound)
        {
            MemberId = memberId;
        }

        public NetKitChatMemberNotFoundException(Guid memberId, string message)
            : base(NetKitChatErrorCode.MemberNotFound, message)
        {
            MemberId = memberId;
        }

        public NetKitChatMemberNotFoundException(Guid memberId, string message, Exception innerException)
            : base(NetKitChatErrorCode.MemberNotFound, message, innerException)
        {
            MemberId = memberId;
        }

        public NetKitChatMemberNotFoundException(string memberSaasId)
            : base(NetKitChatErrorCode.MemberNotFound)
        {
            MemberSaasId = memberSaasId;
            MemberId = Guid.Empty;
        }

        public NetKitChatMemberNotFoundException(string memberSaasId, string message)
            : base(NetKitChatErrorCode.MemberNotFound, message)
        {
            MemberSaasId = memberSaasId;
            MemberId = Guid.Empty;
        }

        public NetKitChatMemberNotFoundException(string memberSaasId, string message, Exception innerException)
            : base(NetKitChatErrorCode.MemberNotFound, message, innerException)
        {
            MemberSaasId = memberSaasId;
            MemberId = Guid.Empty;
        }

        public Guid MemberId { get; }

        public string MemberSaasId { get; }
    }
}