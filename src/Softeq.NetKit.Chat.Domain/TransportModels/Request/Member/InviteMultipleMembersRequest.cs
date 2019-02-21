// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Collections.Generic;

namespace Softeq.NetKit.Chat.Domain.TransportModels.Request.Member
{
    public class InviteMultipleMembersRequest : UserRequest
    {
        public InviteMultipleMembersRequest(string saasUserId, Guid channelId, List<Guid> invitedMembersIds)
            : base(saasUserId)
        {
            ChannelId = channelId;
            InvitedMembersIds = invitedMembersIds;
        }

        public Guid ChannelId { get; }

        public List<Guid> InvitedMembersIds { get; }
    }
}