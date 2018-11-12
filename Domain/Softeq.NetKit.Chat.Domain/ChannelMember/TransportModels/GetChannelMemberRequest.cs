// // Developed by Softeq Development Corporation
// // http://www.softeq.com

using System;

namespace Softeq.NetKit.Chat.Domain.ChannelMember.TransportModels
{
    public class GetChannelMemberRequest
    {
        public GetChannelMemberRequest(Guid memberId, Guid channelId)
        {
            MemberId = memberId;
            ChannelId = channelId;
        }

        public Guid MemberId { get; set; }
        public Guid ChannelId { get; set; }
    }
}