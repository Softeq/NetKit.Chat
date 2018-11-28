// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Collections.Generic;

namespace Softeq.NetKit.Chat.Domain.TransportModels.Request.Channel
{
    public class InviteMultipleMembersRequest
    {
        public InviteMultipleMembersRequest(string saasUserId, Guid channelId, List<Guid> invitedMembersIds)
        {
            SaasUserId = saasUserId;
            ChannelId = channelId;
            InvitedMembersIds = invitedMembersIds;
        }

        public string SaasUserId { get; }

        public Guid ChannelId { get; }

        public List<Guid> InvitedMembersIds { get; }
    }
}