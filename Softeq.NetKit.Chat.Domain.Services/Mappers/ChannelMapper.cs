// Developed by Softeq Development Corporation
// http://www.softeq.com

using System.Linq;
using Softeq.NetKit.Chat.Domain.Services.Configuration;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.Channel;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.Member;

namespace Softeq.NetKit.Chat.Domain.Services.Mappers
{
    internal static class ChannelMapper
    {
        public static ChannelResponse ToChannelResponse(this DomainModels.Channel channel, CloudStorageConfiguration configuration)
        {
            var channelResponse = new ChannelResponse();
            if (channel != null)
            {
                channelResponse.Id = channel.Id;
                channelResponse.IsClosed = channel.IsClosed;
                channelResponse.Updated = channel.Updated;
                channelResponse.Created = channel.Created;
                channelResponse.MembersCount = channel.MembersCount;
                channelResponse.Name = channel.Name;
                channelResponse.Description = channel.Description;
                channelResponse.WelcomeMessage = channel.WelcomeMessage;
                channelResponse.Type = channel.Type;
                channelResponse.CreatorId = channel.CreatorId;
                channelResponse.PhotoUrl = channel.PhotoUrl;
            }

            return channelResponse;
        }

        public static ChannelSummaryResponse ToChannelSummaryResponse(this Domain.Channel.Channel channel, 
            ChannelMembers channelMember,
            Domain.Message.Message lastReadMessage,
            MemberSummary creator, 
            CloudStorageConfiguration configuration)
        {
            var channelListResponse = new ChannelSummaryResponse();
            if (channel != null)
            {
                channelListResponse.Id = channel.Id;
                channelListResponse.IsClosed = channel.IsClosed;
                channelListResponse.Created = channel.Created;
                channelListResponse.Updated = channel.Updated;
                channelListResponse.Name = channel.Name;
                channelListResponse.Description = channel.Description;
                channelListResponse.WelcomeMessage = channel.WelcomeMessage;
                channelListResponse.Type = channel.Type;
                channelListResponse.IsMuted = channelMember.IsMuted;
                channelListResponse.IsPinned = channelMember.IsPinned;
                channelListResponse.CreatorId = channel.CreatorId ?? creator?.Id;
                channelListResponse.Creator = channel.Creator?.ToMemberSummary(configuration) ?? creator;
                channelListResponse.CreatorSaasUserId = channel.Creator?.SaasUserId ?? creator?.SaasUserId;
                channelListResponse.LastMessage = channel.Messages?.FirstOrDefault()?.ToMessageResponse(lastReadMessage, configuration);
                channelListResponse.UnreadMessagesCount = lastReadMessage != null ? channel.Messages?.Count(x => x.Created > lastReadMessage.Created) ?? 0 : channel.Messages?.Count ?? 0;
                channelListResponse.PhotoUrl = channel.PhotoUrl;
            }

            return channelListResponse;
        }
    }
}