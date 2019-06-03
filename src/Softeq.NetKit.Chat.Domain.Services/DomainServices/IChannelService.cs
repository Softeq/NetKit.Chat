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
        Task<ChannelSummaryResponse> CreateDirectChannelAsync(CreateDirectChannelRequest request);
        Task<IReadOnlyCollection<ChannelResponse>> GetMemberChannelsAsync(string saasUserId);
        Task<ChannelResponse> UpdateChannelAsync(UpdateChannelRequest request);
        Task<ChannelSummaryResponse> GetChannelSummaryAsync(string saasUserId, Guid channelId);
        Task<ChannelResponse> GetChannelByIdAsync(Guid channelId);
        Task<ChannelResponse> CloseChannelAsync(string saasUserId, Guid channelId);
        Task<IReadOnlyCollection<ChannelSummaryResponse>> GetAllowedChannelsAsync(string saasUserId);
        Task<IReadOnlyCollection<ChannelResponse>> GetAllChannelsAsync();
        Task<SettingsResponse> GetChannelSettingsAsync(Guid channelId);
        Task JoinToChannelAsync(string saasUserId, Guid channelId);
        Task LeaveFromChannelAsync(string saasUserId, Guid channelId);
        Task DeleteMemberFromChannelAsync(string saasUserId, Guid channelId, Guid memberToDeleteId);
        Task MuteChannelAsync(string saasUserId, Guid channelId, bool isMuted);
        Task PinChannelAsync(string saasUserId, Guid channelId, bool isPinned);
        Task<int> GetChannelMessagesCountAsync(Guid channelId);
    }
}