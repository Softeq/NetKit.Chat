// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Softeq.NetKit.Chat.Domain.Channel.TransportModels.Request;
using Softeq.NetKit.Chat.Domain.Channel.TransportModels.Response;
using Softeq.NetKit.Chat.Domain.Member.TransportModels.Request;
using Softeq.NetKit.Chat.Domain.Settings.TransportModels.Response;

namespace Softeq.NetKit.Chat.Domain.Channel
{
    public interface IChannelService
    {
        Task<ChannelSummaryResponse> CreateChannelAsync(CreateChannelRequest request);
        Task<IEnumerable<ChannelResponse>> GetMyChannelsAsync(UserRequest request);
        Task<ChannelResponse> UpdateChannelAsync(UpdateChannelRequest request);
        Task<ChannelSummaryResponse> GetChannelSummaryAsync(ChannelRequest request);
        Task<ChannelResponse> GetChannelByIdAsync(ChannelRequest request);
        Task<ChannelResponse> CloseChannelAsync(ChannelRequest request);
        Task<IEnumerable<ChannelSummaryResponse>> GetAllowedChannelsAsync(UserRequest request);
        Task<IEnumerable<ChannelResponse>> GetAllChannelsAsync();
        Task<SettingsResponse> GetChannelSettingsAsync(Guid channelId);
        Task JoinToChannelAsync(JoinToChannelRequest request);
        Task RemoveMemberFromChannelAsync(ChannelRequest request);
        Task<bool> CheckIfMemberExistInChannelAsync(InviteMemberRequest request);
        Task MuteChannelAsync(ChannelRequest request);
        Task<int> GetChannelMessageCountAsync(ChannelRequest request);
    }
}