// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;

namespace Softeq.NetKit.Chat.Domain.TransportModels.Request.DirectMembers
{
    public class CreateDirectMembersRequest : UserRequest
    {
        public CreateDirectMembersRequest(string saasUserId, Guid firstMemberId, Guid secondMemberId)
            : base(saasUserId)
        {
            FirstMemberId = firstMemberId;
            SecondMemberId = secondMemberId;
        }

        public Guid DirectId { get; set; }
        public Guid FirstMemberId { get; }
        public Guid SecondMemberId { get; }
    }
}
