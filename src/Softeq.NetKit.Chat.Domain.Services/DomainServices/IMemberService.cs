// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Softeq.NetKit.Chat.Domain.DomainModels;
using Softeq.NetKit.Chat.Domain.TransportModels.Request.Member;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.Channel;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.Client;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.Member;

namespace Softeq.NetKit.Chat.Domain.Services.DomainServices
{
    public interface IMemberService
    {
        Task<MemberSummaryResponse> GetMemberBySaasUserIdAsync(string saasUserId);
        Task<MemberSummaryResponse> GetMemberByIdAsync(Guid memberId);
        Task<IReadOnlyCollection<MemberSummaryResponse>> GetChannelMembersAsync(Guid channelId);
        Task<ChannelResponse> InviteMemberAsync(Guid memberId, Guid channelId);
        Task ActivateMemberAsync(string saasUserId);
        Task UpdateActivityAsync(UpdateMemberActivityRequest request);
        Task<IReadOnlyCollection<DomainModels.Client>> GetMemberClientsAsync(Guid memberId);
        Task<MemberSummaryResponse> AddMemberAsync(string saasUserId, string email);
        Task UpdateMemberStatusAsync(string saasUserId, UserStatus status);
        Task<IReadOnlyCollection<ClientResponse>> GetClientsByMemberIds(List<Guid> memberIds);
        Task<PagedMembersResponse> GetPagedMembersAsync(int pageNumber, int pageSize, string nameFilter, string currentUserSaasId);
        Task<PagedMembersResponse> GetPotentialChannelMembersAsync(Guid channelId, GetPotentialChannelMembersRequest request);
    }
}
