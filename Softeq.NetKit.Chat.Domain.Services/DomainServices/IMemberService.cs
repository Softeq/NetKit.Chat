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
        Task<MemberSummary> GetMemberBySaasUserIdAsync(string saasUserId);
        Task<MemberSummary> GetMemberByIdAsync(Guid memberId);
        Task<IReadOnlyCollection<MemberSummary>> GetChannelMembersAsync(Guid channelId);
        Task<ChannelResponse> InviteMemberAsync(Guid memberId, Guid channelId);
        Task UpdateActivityAsync(UpdateMemberActivityRequest request);
        Task<IReadOnlyCollection<Client>> GetMemberClientsAsync(Guid memberId);
        Task<MemberSummary> AddMemberAsync(string saasUserId, string email);
        Task UpdateMemberStatusAsync(string saasUserId, UserStatus status);
        Task<IReadOnlyCollection<ClientResponse>> GetClientsByMemberIds(List<Guid> memberIds);
        Task<IReadOnlyCollection<MemberSummary>> GetAllMembersAsync();
    }
}