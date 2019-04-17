// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Softeq.NetKit.Chat.Domain.DomainModels;

namespace Softeq.NetKit.Chat.Data.Persistent.Repositories
{
    public interface IClientRepository
    {
        Task<Client> GetClientWithMemberAsync(string clientConnectionId);
        Task AddClientAsync(Client client);
        Task UpdateClientAsync(Client client);
        Task DeleteClientAsync(Guid clientId);
        Task<IReadOnlyCollection<Client>> GetMemberClientsAsync(Guid memberId);
        Task<IReadOnlyCollection<Client>> GetClientsWithMembersAsync(List<Guid> memberIds);
        Task<bool> IsClientExistsAsync(string clientConnectionId);
        Task<IReadOnlyCollection<string>> GetNotMutedChannelClientConnectionIdsAsync(Guid channelId);
        Task<IReadOnlyCollection<string>> GetChannelClientConnectionIdsAsync(Guid channelId);
        Task<IReadOnlyCollection<string>> GetChannelMemberClientConnectionIdsAsync(Guid channelId, Guid memberId);
    }
}