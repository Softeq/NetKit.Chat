// // Developed by Softeq Development Corporation
// // http://www.softeq.com

using System;
using Softeq.NetKit.Chat.Domain.Base;

namespace Softeq.NetKit.Chat.Domain.Notification
{
    public class Notification : IBaseEntity<Guid>
    {
        public Guid MemberId { get; set; }
        public Guid MessageId { get; set; }
        public Guid ChannelId { get; set; }
        public Member.Member Member { get; set; }
        public Message.Message Message { get; set; }
        public Channel.Channel Channel { get; set; }
        public bool IsRead { get; set; }
        public Guid Id { get; set; }
    }
}