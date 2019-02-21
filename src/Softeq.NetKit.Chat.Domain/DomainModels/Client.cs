// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;

namespace Softeq.NetKit.Chat.Domain.DomainModels
{
    public class Client : IBaseEntity<Guid>
    {
        public Guid Id { get; set; }

        public string ClientConnectionId { get; set; }

        public Guid MemberId { get; set; }
        public Member Member { get; set; }

        public string UserAgent { get; set; }
        public string Name { get; set; }

        public DateTimeOffset LastActivity { get; set; }
        public DateTimeOffset LastClientActivity { get; set; }
    }
}