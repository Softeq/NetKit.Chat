// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;

namespace Softeq.NetKit.Chat.Web.TransportModels.Request.DirectMembers
{
    public class CreateDirectMembersRequest
    {
        public CreateDirectMembersRequest(Guid firstMemberId, Guid secondMemberId)
        {
            FirstMemberId = firstMemberId;
            SecondMemberId = secondMemberId;
        }

        public Guid FirstMemberId { get; }
        public Guid SecondMemberId { get; }
    }
}
