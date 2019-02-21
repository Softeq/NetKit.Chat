// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;

namespace Softeq.NetKit.Chat.Domain.DomainModels
{
    public class Attachment : IBaseEntity<Guid>, ICreated
    {
        public Guid Id { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public long Size { get; set; }

        public Guid MessageId { get; set; }
        public Message Message { get; set; }
        public DateTimeOffset Created { get; set; }
    }
}