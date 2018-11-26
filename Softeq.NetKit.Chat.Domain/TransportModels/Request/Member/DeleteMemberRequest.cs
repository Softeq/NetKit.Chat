// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;

namespace Softeq.NetKit.Chat.Domain.TransportModels.Request.Member
{
    public class DeleteMemberRequest : UserRequest
    {
        public DeleteMemberRequest(string saasUserId, Guid channelId, Guid memberId) : base(saasUserId)
        {
            ChannelId = channelId;
            MemberId = memberId;
        }

        public Guid ChannelId { get; set; }
        public Guid MemberId { get; set; }
    }
}