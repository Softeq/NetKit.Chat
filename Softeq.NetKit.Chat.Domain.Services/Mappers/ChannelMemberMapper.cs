// Developed by Softeq Development Corporation
// http://www.softeq.com

using Softeq.NetKit.Chat.Domain.DomainModels;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.ChannelMember;

namespace Softeq.NetKit.Chat.Domain.Services.Mappers
{
    internal static class ChannelMemberMapper
    {
        public static ChannelMemberResponse ToChannelMemberResponse(this ChannelMember channelMember)
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