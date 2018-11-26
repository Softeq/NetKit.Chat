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
        Task AddChannelMemberAsync(ChannelMembers channelMember);
        Task DeleteChannelMemberAsync(Guid memberId, Guid channelId);
        Task UpdateChannelMemberAsync(ChannelMembers channelMember);
        Task<List<ChannelMembers>> GetChannelMembersAsync(Guid channelId);
        Task MuteChannelAsync(Guid memberId, Guid channelId);
        Task PinChannelAsync(Guid memberId, Guid channelId, bool isPinned);
        Task SetLastReadMessageAsync(Guid memberId, Guid channelId, Guid messageId);
        Task UpdateLastReadMessageAsync(Guid previousLastReadMessageId, Guid currentLastReadMessageId);
        Task<ChannelMembers> GetChannelMemberAsync(Guid memberId, Guid channelId);
    }
}