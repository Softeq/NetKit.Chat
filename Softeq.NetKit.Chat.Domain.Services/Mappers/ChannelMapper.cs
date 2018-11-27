// Developed by Softeq Development Corporation
// http://www.softeq.com

using System.Linq;
using Softeq.NetKit.Chat.Domain.DomainModels;
using Softeq.NetKit.Chat.Domain.Services.Configuration;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.Channel;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.Member;

namespace Softeq.NetKit.Chat.Domain.Services.Mappers
{
    internal static class ChannelMapper
    {
        public static ChannelResponse ToChannelResponse(this Channel channel, CloudStorageConfiguration configuration)
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

        public static ChannelSummaryResponse ToChannelSummaryResponse(this Channel channel, 
            ChannelMembers channelMember,
            Message lastReadMessage, 
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
                channelListResponse.CreatorId = channel.CreatorId;
                channelListResponse.Creator = channel.Creator.ToMemberSummary(configuration);
                channelListResponse.CreatorSaasUserId = channel.Creator.SaasUserId;
                channelListResponse.LastMessage = channel.Messages?.FirstOrDefault()?.ToMessageResponse(lastReadMessage, configuration);
                channelListResponse.UnreadMessagesCount = lastReadMessage != null ? channel.Messages?.Count(x => x.Created > lastReadMessage.Created) ?? 0 : channel.Messages?.Count ?? 0;
                channelListResponse.PhotoUrl = channel.PhotoUrl;
            }

            return channelListResponse;
        }
    }
}