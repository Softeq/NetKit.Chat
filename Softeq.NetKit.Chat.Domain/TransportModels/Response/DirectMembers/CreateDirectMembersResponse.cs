// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.Member;

namespace Softeq.NetKit.Chat.Domain.TransportModels.Response.DirectMembers
{
    public class CreateDirectMembersResponse
    {
        public Guid DirectMembersId { get; set; }
        public MemberSummary FirstDirectMember { get; set; }
        public MemberSummary SecondDirectMember { get; set; }
    }
}
