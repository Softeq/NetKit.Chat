// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Softeq.NetKit.Chat.Domain.Member;

namespace Softeq.NetKit.Chat.Infrastructure.Storage.Sql.Repositories
{
    public interface IMemberRepository
    {
        Task<List<Member>> GetAllMembersAsync();
        Task<List<Member>> GetOnlineMembersInChannelAsync(Guid channelId);
        Task<List<Member>> GetAllOnlineMembersAsync();
        Task<List<Member>> SearchMembersByNameAsync(string name);
        Task<Member> GetMemberByIdAsync(Guid memberId);
        Task<Member> GetMemberByNameAsync(string name);
        Task<Member> GetMemberByClientIdAsync(Guid clientId);
        Task AddMemberAsync(Member member);
        Task UpdateMemberAsync(Member member);
        Task DeleteMemberAsync(Guid memberId);
        Task<Member> GetMemberBySaasUserIdAsync(string saasUserId);
        Task<List<Member>> GetAllMembersByChannelIdAsync(Guid channelId);
    }
}