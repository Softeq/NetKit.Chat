// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Softeq.NetKit.Chat.Domain.DomainModels;

namespace Softeq.NetKit.Chat.Data.Persistent.Repositories
{
    public interface IChannelMemberRepository
    {
        Task AddChannelMemberAsync(ChannelMember channelMember);
        Task DeleteChannelMemberAsync(Guid memberId, Guid channelId);
        Task<IReadOnlyCollection<ChannelMember>> GetChannelMembersAsync(Guid channelId);
        Task<ChannelMemberAggregate> GetChannelMemberWithLastReadMessageAndCounterAsync(Guid channelId, Guid memberId);
        Task<ChannelMember> GetChannelMemberWithMemberDetailsAsync(Guid memberId, Guid channelId);
        Task<IReadOnlyCollection<ChannelMember>> GetChannelMembersWithMemberDetailsAsync(Guid channelId);
        Task MuteChannelAsync(Guid memberId, Guid channelId, bool isMuted);
        Task PinChannelAsync(Guid memberId, Guid channelId, bool isPinned);
        Task SetLastReadMessageAsync(Guid memberId, Guid channelId, Guid messageId);
        Task UpdateLastReadMessageAsync(Guid previousLastReadMessageId, Guid? currentLastReadMessageId);
        Task<ChannelMember> GetChannelMemberAsync(Guid memberId, Guid channelId);
        Task<IList<string>> GetSaasUserIdsWithDisabledChannelNotificationsAsync(Guid channelId);
    }
}