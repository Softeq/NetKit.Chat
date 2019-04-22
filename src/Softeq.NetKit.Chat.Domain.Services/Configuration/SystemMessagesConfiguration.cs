// Developed by Softeq Development Corporation
// http://www.softeq.com

namespace Softeq.NetKit.Chat.Domain.Services.Configuration
{
    public class SystemMessagesConfiguration
    {
        public SystemMessagesConfiguration(
                        string memberJoined,
                        string memberDeleted,
                        string memberLeft,
                        string channelNameChanged,
                        string channelIconChanged)
        {
            MemberJoined = memberJoined;
            MemberDeleted = memberDeleted;
            MemberLeft = memberLeft;
            ChannelNameChanged = channelNameChanged;
            ChannelIconChanged = channelIconChanged;
        }

        public string MemberJoined { get; }
        public string MemberDeleted { get; }
        public string MemberLeft { get; }
        public string ChannelNameChanged { get; }
        public string ChannelIconChanged { get; }
    }
}
