// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Softeq.NetKit.Chat.Domain.Channel.TransportModels.Request;
using Softeq.NetKit.Chat.Domain.Channel.TransportModels.Response;
using Softeq.NetKit.Chat.Domain.Client.TransportModels.Request;
using Softeq.NetKit.Chat.Domain.Client.TransportModels.Response;
using Softeq.NetKit.Chat.Domain.Member.TransportModels.Request;
using Softeq.NetKit.Chat.Domain.Member.TransportModels.Response;

namespace Softeq.NetKit.Chat.Domain.Member
{
    public interface IMemberService
    {
        Task<ParticipantResponse> SurelyGetMemberBySaasUserIdAsync(string saasUserId);
        Task<MemberSummary> GetMemberBySaasUserIdAsync(string saasUserId);
        Task<MemberSummary> GetMemberByIdAsync(Guid memberId);
        Task<ParticipantResponse> GetMemberAsync(UserRequest request);
        Task<IReadOnlyCollection<MemberSummary>> GetChannelMembersAsync(Guid channelId);
        Task<ChannelResponse> InviteMemberAsync(InviteMemberRequest request);
        Task<IReadOnlyCollection<ParticipantResponse>> GetOnlineChannelMembersAsync(ChannelRequest request);

        Task<ClientResponse> GetOrAddClientAsync(AddClientRequest request);
        Task DeleteClientAsync(DeleteClientRequest request);
        Task UpdateActivityAsync(AddClientRequest request);
        Task<IReadOnlyCollection<Client.Client>> GetMemberClientsAsync(Guid memberId);
        Task<MemberSummary> AddMemberAsync(string saasUserId, string email);
        Task UpdateMemberStatusAsync(UpdateMemberStatusRequest request);
        Task<IReadOnlyCollection<ClientResponse>> GetClientsByMemberIds(List<Guid> memberIds);
        Task<IReadOnlyCollection<MemberSummary>> GetAllMembersAsync();
    }
}