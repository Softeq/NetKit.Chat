// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Softeq.NetKit.Chat.Domain.DomainModels;
using Softeq.NetKit.Chat.Domain.TransportModels.Request.Channel;
using Softeq.NetKit.Chat.Domain.TransportModels.Request.Client;
using Softeq.NetKit.Chat.Domain.TransportModels.Request.Member;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.Channel;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.Client;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.Member;

namespace Softeq.NetKit.Chat.Domain.Services.DomainServices
{
    public interface IMemberService
    {
        Task<MemberSummary> GetMemberBySaasUserIdAsync(string saasUserId);
        Task<MemberSummary> GetMemberByIdAsync(Guid memberId);
        Task<IReadOnlyCollection<MemberSummary>> GetChannelMembersAsync(Guid channelId);
        Task<ChannelResponse> InviteMemberAsync(InviteMemberRequest request);
        Task<ClientResponse> GetOrAddClientAsync(AddClientRequest request);
        Task DeleteClientAsync(DeleteClientRequest request);
        Task UpdateActivityAsync(AddClientRequest request);
        Task<IReadOnlyCollection<Client>> GetMemberClientsAsync(Guid memberId);
        Task<MemberSummary> AddMemberAsync(string saasUserId, string email);
        Task UpdateMemberStatusAsync(UpdateMemberStatusRequest request);
        Task<IReadOnlyCollection<ClientResponse>> GetClientsByMemberIds(List<Guid> memberIds);
        Task<PagedMembersResponse> GetPagedMembersAsync(int pageNumber, int pageSize, string nameFilter);
        Task<PagedMembersResponse> GetPotentialChannelMembersAsync(Guid channelId, GetPotentialChannelMembers request);
    }
}