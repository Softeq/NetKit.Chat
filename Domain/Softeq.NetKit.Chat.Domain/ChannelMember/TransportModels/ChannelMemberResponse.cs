// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;

namespace Softeq.NetKit.Chat.Domain.ChannelMember.TransportModels
{
    public class ChannelMemberResponse
    {
        public Guid MemberId { get; set; }
        public Guid ChannelId { get; set; }
        public bool IsMuted { get; set; }
        public Guid? LastReadMessageId { get; set; }
        public bool IsRead { get; set; }
    }
}