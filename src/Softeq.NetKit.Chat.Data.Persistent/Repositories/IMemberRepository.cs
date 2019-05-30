// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Softeq.NetKit.Chat.Domain.DomainModels;

namespace Softeq.NetKit.Chat.Data.Persistent.Repositories
{
    public interface IMemberRepository
    {
        Task<QueryResult<Member>> GetPagedMembersExceptCurrentAsync(int pageNumber, int pageSize, string nameFilter, string currentUserSaasId);
        Task<QueryResult<Member>> GetPotentialChannelMembersAsync(Guid channelId, int pageNumber, int pageSize, string nameFilter);
        Task<Member> GetMemberByIdAsync(Guid memberId);
        Task AddMemberAsync(Member member);
        Task UpdateMemberAsync(Member member);
        Task ActivateMemberAsync(Member member);
        Task<Member> GetMemberBySaasUserIdAsync(string saasUserId);
        Task<IReadOnlyCollection<Member>> GetAllMembersByChannelIdAsync(Guid channelId);
        Task<List<Member>> GetMembersExceptProvidedAsync(IEnumerable<Guid> memberIds);
    }
}