// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;

namespace Softeq.NetKit.Chat.Domain.DomainModels
{
    public class Notification : IBaseEntity<Guid>
    {
        public Guid Id { get; set; }
        public Guid MemberId { get; set; }
        public Guid MessageId { get; set; }
        public Guid ChannelId { get; set; }
        public DomainModels.Member Member { get; set; }
        public DomainModels.Message Message { get; set; }
        public DomainModels.Channel Channel { get; set; }
        public bool IsRead { get; set; }
    }
}