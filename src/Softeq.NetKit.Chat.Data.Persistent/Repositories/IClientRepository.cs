// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Softeq.NetKit.Chat.Data.Persistent.Repositories
{
    public interface IClientRepository
    {
        Task<Domain.DomainModels.Client> GetClientWithMemberAsync(string clientConnectionId);
        Task AddClientAsync(Domain.DomainModels.Client client);
        Task UpdateClientAsync(Domain.DomainModels.Client client);
        Task DeleteClientAsync(Guid clientId);
        Task DeleteOverThresholdMemberClientsAsync(Guid memberId, int inactiveMinutesThreshold);
        Task<bool> DoesMemberHasClientsAsync(Guid memberId);
        Task<IReadOnlyCollection<Domain.DomainModels.Client>> GetMemberClientsAsync(Guid memberId);
        Task<IReadOnlyCollection<Domain.DomainModels.Client>> GetClientsWithMembersAsync(List<Guid> memberIds);
        Task<bool> IsClientExistsAsync(string clientConnectionId);
        Task<IReadOnlyCollection<string>> GetChannelClientConnectionIdsAsync(Guid channelId);
        Task<IReadOnlyCollection<string>> GetChannelMemberClientConnectionIdsAsync(Guid channelId, Guid memberId);
    }
}