// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;

namespace Softeq.NetKit.Chat.Domain.DomainModels
{
    public class ChannelMember
    {
        public Guid MemberId { get; set; }
        public Member Member { get; set; }

        // Channel info
        public Guid ChannelId { get; set; }
        public Channel Channel { get; set; }
        public bool IsMuted { get; set; }
        public bool IsPinned { get; set; }
        // concrete chat role
        public UserRole Role { get; set; }

        // Last channel message info
        public Guid? LastReadMessageId { get; set; }
        public Message LastReadMessage { get; set; }
    }
}