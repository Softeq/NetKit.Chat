// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Collections.Generic;
using System.Text;
using Softeq.NetKit.Chat.Domain.Member.TransportModels.Request;

namespace Softeq.NetKit.Chat.Domain.Channel.TransportModels.Request
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
