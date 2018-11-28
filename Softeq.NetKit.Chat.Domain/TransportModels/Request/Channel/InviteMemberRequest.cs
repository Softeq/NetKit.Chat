// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;

namespace Softeq.NetKit.Chat.Domain.TransportModels.Request.Channel
{
    public class InviteMemberRequest
    {
        public InviteMemberRequest(string saasUserId, Guid channelId, Guid memberId)
        {
            SaasUserId = saasUserId;
            ChannelId = channelId;
            MemberId = memberId;
        }

        public string SaasUserId { get; }

        public Guid ChannelId { get; }

        public Guid MemberId { get; }
    }
}