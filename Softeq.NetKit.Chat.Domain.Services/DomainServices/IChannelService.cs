// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Softeq.NetKit.Chat.Domain.TransportModels.Request.Channel;
using Softeq.NetKit.Chat.Domain.TransportModels.Request.Member;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.Channel;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.Settings;

namespace Softeq.NetKit.Chat.Domain.Services.DomainServices
{
    public interface IChannelService
    {
        Task<ChannelSummaryResponse> CreateChannelAsync(CreateChannelRequest request);
        Task<IReadOnlyCollection<ChannelResponse>> GetUserChannelsAsync(UserRequest request);
        Task<ChannelResponse> UpdateChannelAsync(UpdateChannelRequest request);
        Task<ChannelSummaryResponse> GetChannelSummaryAsync(ChannelRequest request);
        Task<ChannelResponse> GetChannelByIdAsync(Guid channelId);
        Task<ChannelResponse> CloseChannelAsync(ChannelRequest request);
        Task<IReadOnlyCollection<ChannelSummaryResponse>> GetAllowedChannelsAsync(UserRequest request);
        Task<IReadOnlyCollection<ChannelResponse>> GetAllChannelsAsync();
        Task<SettingsResponse> GetChannelSettingsAsync(Guid channelId);
        Task JoinToChannelAsync(JoinToChannelRequest request);
        Task LeaveChannelAsync(ChannelRequest request);
        Task<bool> CheckIfMemberExistInChannelAsync(InviteMemberRequest request);
        Task MuteChannelAsync(ChannelRequest request);
        Task<int> GetChannelMessagesCountAsync(Guid channelId);
    }
}