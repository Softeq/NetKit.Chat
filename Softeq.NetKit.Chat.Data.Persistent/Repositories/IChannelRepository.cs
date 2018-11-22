// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Softeq.NetKit.Chat.Domain.DomainModels;

namespace Softeq.NetKit.Chat.Data.Persistent.Repositories
{
    public interface IChannelRepository
    {
        Task<List<Channel>> GetAllChannelsAsync();
        Task<List<Channel>> GetAllowedChannelsAsync(Guid memberId);
        Task<Channel> GetChannelByNameAsync(string channelName);
        Task<Channel> GetChannelByIdAsync(Guid channelId);
        Task<bool> CheckIfMemberExistInChannelAsync(Guid memberId, Guid channelId);
        Task AddChannelAsync(Channel channel);
        Task DeleteChannelAsync(Guid channelId);
        Task UpdateChannelAsync(Channel channel);
        Task<List<Channel>> GetChannelsByMemberId(Guid memberId);
        Task IncrementChannelMembersCount(Guid channelId);
        Task DecrementChannelMembersCount(Guid channelId);
    }
}