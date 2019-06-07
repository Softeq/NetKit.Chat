namespace Softeq.NetKit.Chat.Domain.DomainModels
{
    public class ChannelMemberAggregate
    {
        public ChannelMember ChannelMember { get; set; }
        public Message Message { get; set; }
        public int UnreadMessagesCount { get; set; }
    }
}
