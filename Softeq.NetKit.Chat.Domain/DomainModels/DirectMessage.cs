// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;

namespace Softeq.NetKit.Chat.Domain.DomainModels
{
    public class DirectMessage : IBaseEntity<Guid>, ICreated, IUpdated
    {
        public Guid Id { get; set; }
        public Guid DirectChannelId { get; set; }
        public DateTimeOffset Created { get; set; }
        public Guid OwnerId { get; set; }
        public string Body { get; set; }
        public DateTimeOffset Updated { get; set; }
    }
}
