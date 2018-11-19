// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Softeq.NetKit.Chat.Domain.ChannelMember;

namespace Softeq.NetKit.Chat.Data.Repositories.Repositories
{
    public interface IChannelMemberRepository
    {
        Task AddChannelMemberAsync(ChannelMembers channelMember);
        Task DeleteChannelMemberAsync(Guid memberId, Guid channelId);
        Task UpdateChannelMemberAsync(ChannelMembers channelMember);
        Task<List<ChannelMembers>> GetChannelMembersAsync(Guid channelId);
        Task MuteChannelAsync(Guid memberId, Guid channelId);
        Task AddLastReadMessageAsync(Guid memberId, Guid channelId, Guid messageId);
        Task UpdateLastReadMessageAsync(Guid messageId);
        Task<ChannelMembers> GetChannelMemberAsync(Guid memberId, Guid channelId);
    }
}