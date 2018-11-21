// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Collections.Generic;
using Softeq.NetKit.Chat.Domain.TransportModels.Request.Member;

namespace Softeq.NetKit.Chat.Domain.TransportModels.Request.Channel
{
    public class InviteMembersRequest : UserRequest
    {
        public InviteMembersRequest(string saasUserId) : base(saasUserId)
        {
        }

        public Guid ChannelId { get; set; }
        public List<string> InvitedMembers { get; set; }
    }
}