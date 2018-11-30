// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Collections.Generic;

namespace Softeq.NetKit.Chat.Web.TransportModels.Request.Member
{
    public class InviteMultipleMembersRequest
    {
        public InviteMultipleMembersRequest()
        {
            InvitedMembersIds = new List<Guid>();
        }

        public List<Guid> InvitedMembersIds { get; set; }
    }
}