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
        public Member Member { get; set; }
        public Message Message { get; set; }
        public Channel Channel { get; set; }
        public bool IsRead { get; set; }
    }
}