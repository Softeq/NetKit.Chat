// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using Softeq.NetKit.Chat.Domain.Base;

namespace Softeq.NetKit.Chat.Domain.Attachment
{
    public class Attachment : IBaseEntity<Guid>, ICreated
    {
        public Guid Id { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public long Size { get; set; }

        public Guid MessageId { get; set; }
        public Message.Message Message { get; set; }
        public DateTimeOffset Created { get; set; }
    }
}