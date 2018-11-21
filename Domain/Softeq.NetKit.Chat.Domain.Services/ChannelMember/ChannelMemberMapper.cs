// Developed by Softeq Development Corporation
// http://www.softeq.com

using Softeq.NetKit.Chat.Domain.ChannelMember;
using Softeq.NetKit.Chat.Domain.ChannelMember.TransportModels;

namespace Softeq.NetKit.Chat.Domain.Services.ChannelMember
{
    internal static class ChannelMemberMapper
    {
        public static ChannelMemberResponse ToChannelMemberResponse(this ChannelMembers channelMember)
        {
            var channelMemberResponse = new ChannelMemberResponse();
            if (channelMember != null)
            {
                channelMemberResponse.MemberId = channelMember.MemberId;
                channelMemberResponse.ChannelId = channelMember.ChannelId;
                channelMemberResponse.IsMuted = channelMember.IsMuted;
                channelMemberResponse.IsPinned = channelMember.IsPinned;
                channelMemberResponse.LastReadMessageId = channelMember.LastReadMessageId;
            }
            return channelMemberResponse;
        }
    }
}