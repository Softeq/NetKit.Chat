// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;

namespace Softeq.NetKit.Chat.Domain.DomainModels
{
   public class DirectMembers : IBaseEntity<Guid>
    {
        public Guid Id { get; set; }
        public Guid FirstMemberId { get; set; }
        public Guid SecondMemberId { get; set; }
    }
}
