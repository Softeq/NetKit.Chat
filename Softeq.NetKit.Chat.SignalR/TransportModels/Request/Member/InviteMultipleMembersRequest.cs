// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Collections.Generic;

namespace Softeq.NetKit.Chat.SignalR.TransportModels.Request.Member
{
    public class InviteMultipleMembersRequest : BaseRequest
    {
        public Guid ChannelId { get; set; }

        public List<Guid> InvitedMembersIds { get; set; }
    }
}