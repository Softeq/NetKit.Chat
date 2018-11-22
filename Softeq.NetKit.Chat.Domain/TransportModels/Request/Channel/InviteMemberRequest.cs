// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using Softeq.NetKit.Chat.Domain.TransportModels.Request.Member;

namespace Softeq.NetKit.Chat.Domain.TransportModels.Request.Channel
{
    public class InviteMemberRequest : UserRequest
    {
        public InviteMemberRequest(string saasUserId, Guid channelId, Guid memberId) : base(saasUserId)
        {
            ChannelId = channelId;
            MemberId = memberId;
        }

        public Guid ChannelId { get; set; }
        public Guid MemberId { get; set; }
    }
}