// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;

namespace Softeq.NetKit.Chat.Domain.ChannelMember
{
    public class ChannelMembers
    {
        public Guid MemberId { get; set; }
        public String SaasUserId { get; set; }
        public Member.Member Member { get; set; }

        // Channel info
        public Guid ChannelId { get; set; }
        public Channel.Channel Channel { get; set; }
        public bool IsMuted { get; set; }

        // Last channel message info
        public Guid? LastReadMessageId { get; set; }
        public Message.Message LastReadMessage { get; set; }
    }
}