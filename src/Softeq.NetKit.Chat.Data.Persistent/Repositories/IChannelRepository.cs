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
        Task<IReadOnlyCollection<Channel>> GetAllChannelsAsync();
        Task<Channel> GetChannelWithCreatorAsync(Guid channelId);
        Task<IReadOnlyCollection<Channel>> GetAllowedChannelsWithLastMessageAsync(Guid memberId);
        Task<bool> IsMemberExistsInChannelAsync(Guid memberId, Guid channelId);
        Task AddChannelAsync(Channel channel);
        Task UpdateChannelAsync(Channel channel);
        Task<IReadOnlyCollection<Channel>> GetAllowedChannelsAsync(Guid memberId);
        Task IncrementChannelMembersCountAsync(Guid channelId);
        Task DecrementChannelMembersCountAsync(Guid channelId);
        Task<bool> IsChannelExistsAsync(Guid channelId);
        Task<bool> IsChannelExistsAndOpenAsync(Guid channelId);
        Task<Channel> GetChannelAsync(Guid channelId);
        Task<Guid> GetDirectChannelForMembersAsync(Guid member1Id, Guid member2Id);
    }
}