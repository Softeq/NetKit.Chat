// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;

namespace Softeq.NetKit.Chat.Domain.DomainModels
{
   public class DirectMembers : IBaseEntity<Guid>
    {
        public Guid Id { get; set; }
        public Guid Member01Id { get; set; }
        public Guid Member02Id { get; set; }
    }
}
