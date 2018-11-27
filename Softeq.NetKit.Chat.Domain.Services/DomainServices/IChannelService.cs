// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Softeq.NetKit.Chat.Domain.TransportModels.Request.Channel;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.Channel;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.Settings;

namespace Softeq.NetKit.Chat.Domain.Services.DomainServices
{
    public interface IChannelService
    {
        Task<ChannelSummaryResponse> CreateChannelAsync(CreateChannelRequest request);
        Task<IReadOnlyCollection<ChannelResponse>> GetMemberChannelsAsync(string saasUserId);
        Task<ChannelResponse> UpdateChannelAsync(UpdateChannelRequest request);
        Task<ChannelSummaryResponse> GetChannelSummaryAsync(ChannelRequest request);
        Task<ChannelResponse> GetChannelByIdAsync(Guid channelId);
        Task<ChannelResponse> CloseChannelAsync(ChannelRequest request);
        Task<IReadOnlyCollection<ChannelSummaryResponse>> GetAllowedChannelsAsync(string saasUserId);
        Task<IReadOnlyCollection<ChannelResponse>> GetAllChannelsAsync();
        Task<SettingsResponse> GetChannelSettingsAsync(Guid channelId);
        Task JoinToChannelAsync(JoinToChannelRequest request);
        Task LeaveFromChannelAsync(string saasUserId, Guid channelId);
        Task DeleteMemberFromChannelAsync(string saasUserId, Guid channelId, Guid memberToDeleteId);
        Task MuteChannelAsync(string saasUserId, Guid channelId, bool isMuted);
        Task PinChannelAsync(string saasUserId, Guid channelId, bool isPinned);
        Task<int> GetChannelMessagesCountAsync(Guid channelId);
    }
}