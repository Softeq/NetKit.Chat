// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Collections.Generic;
using Softeq.NetKit.Chat.Domain.Member.TransportModels.Request;

namespace Softeq.NetKit.Chat.Domain.Channel.TransportModels.Request
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