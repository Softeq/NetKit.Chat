// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Softeq.NetKit.Chat.Domain.Channel.TransportModels.Request;
using Softeq.NetKit.Chat.Domain.Channel.TransportModels.Response;
using Softeq.NetKit.Chat.Domain.Member.TransportModels.Request;
using Softeq.NetKit.Chat.Domain.Settings.TransportModels.Response;

namespace Softeq.NetKit.Chat.Infrastructure.SignalR.Sockets
{
    public interface IChannelSocketService
    {
        Task<ChannelSummaryResponse> CreateChannelAsync(CreateChannelRequest createChannelRequest);
        Task<ChannelSummaryResponse> UpdateChannelAsync(UpdateChannelRequest request);
        Task CloseChannelAsync(ChannelRequest request);
        Task JoinToChannelAsync(JoinToChannelRequest request);
        Task LeaveChannelAsync(ChannelRequest request);
        Task<ChannelResponse> InviteMemberAsync(InviteMemberRequest request);
        Task<ChannelResponse> InviteMembersAsync(InviteMembersRequest request);
        Task MuteChannelAsync(ChannelRequest request);
        Task<int> GetChannelMessagesCountAsync(Guid channelId);
        Task<SettingsResponse> GetChannelSettingsAsync(Guid channelId);
        Task<IReadOnlyCollection<ChannelSummaryResponse>> GetAllowedChannelsAsync(UserRequest request);
        Task<IReadOnlyCollection<ChannelResponse>> GetUserChannelsAsync(UserRequest request);
        Task<IReadOnlyCollection<ChannelResponse>> GetAllChannelsAsync();
        Task<ChannelResponse> GetChannelByIdAsync(Guid channelId);
    }
}