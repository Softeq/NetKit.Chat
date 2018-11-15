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
        Task<MemberSummary> GetMemberSummaryBySaasUserIdAsync(string saasUserId);
        Task<MemberSummary> GetMemberByIdAsync(Guid memberId);
        Task<ParticipantResponse> GetMemberAsync(UserRequest request);
        Task<IEnumerable<MemberSummary>> GetChannelMembersAsync(ChannelRequest request);
        Task<ChannelResponse> InviteMemberAsync(InviteMemberRequest request);
        Task<ChannelResponse> InviteMultipleMembersAsync(InviteMembersRequest request);
        Task<IEnumerable<ParticipantResponse>> GetOnlineChannelMembersAsync(ChannelRequest request);


        Task<ConnectionResponse> GetOrAddClientAsync(AddConnectionRequest request);
        Task DeleteClientAsync(DeleteConnectionRequest request);
        Task UpdateActivityAsync(AddConnectionRequest request);
        Task<IEnumerable<Client.Connection>> GetMemberClientsAsync(Guid memberId);
        Task<MemberSummary> AddMemberAsync(string saasUserId, string email);
        Task UpdateMemberStatusAsync(UpdateMemberStatusRequest request);
        Task<IEnumerable<ConnectionResponse>> GetClientsByMemberIds(List<Guid> memberIds);
        Task<IEnumerable<MemberSummary>> GetAllMembersAsync();
    }
}