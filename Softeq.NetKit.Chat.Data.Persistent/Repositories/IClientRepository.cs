// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Softeq.NetKit.Chat.Domain.Client;

namespace Softeq.NetKit.Chat.Data.Persistent.Repositories
{
    public interface IClientRepository
    {
        Task<List<Client>> GetAllClientsAsync();
        Task<Client> GetClientByIdAsync(Guid clientId);
        Task<Client> GetClientByConnectionIdAsync(string clientConnectionId);
        Task AddClientAsync(Client client);
        Task UpdateClientAsync(Client client);
        Task DeleteClientAsync(Guid clientId);
        Task<List<Client>> GetMemberClientsAsync(Guid memberId);
        Task<List<Client>> GetClientsByMemberIdsAsync(List<Guid> memberIds);
    }
}