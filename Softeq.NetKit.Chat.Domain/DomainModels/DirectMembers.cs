// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;

namespace Softeq.NetKit.Chat.Domain.DomainModels
{
   public class DirectMembers : IBaseEntity<Guid>
    {
        public Guid Id { get; set; }
        public Guid OwnerId { get; set; }
        public Guid MemberId { get; set; }
    }
}
