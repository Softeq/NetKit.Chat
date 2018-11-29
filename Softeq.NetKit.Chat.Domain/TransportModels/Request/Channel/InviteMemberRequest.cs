// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;

namespace Softeq.NetKit.Chat.Domain.TransportModels.Request.Channel
{
    public class InviteMemberRequest : UserRequest
    {
        public InviteMemberRequest(string saasUserId, Guid channelId, Guid memberId)
            : base(saasUserId)
        {
            ChannelId = channelId;
            MemberId = memberId;
        }

        public Guid ChannelId { get; }

        public Guid MemberId { get; }
    }
}