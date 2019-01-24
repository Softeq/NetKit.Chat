// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;

namespace Softeq.NetKit.Chat.Web.TransportModels.Request.DirectMembers
{
    public class CreateDirectMembersRequest
    {
        public CreateDirectMembersRequest(Guid ownerId, Guid memberId)
        {
            OwnerId = ownerId;
            MemberId = memberId;
        }

        public Guid OwnerId { get; }
        public Guid MemberId { get; }
    }
}
