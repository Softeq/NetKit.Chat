// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Softeq.NetKit.Chat.Domain.Client;

namespace Softeq.NetKit.Chat.Data.Interfaces.Repository
{
    public interface IClientRepository
    {
        Task<List<Connection>> GetAllClientsAsync();
        Task<Connection> GetClientByIdAsync(Guid clientId);
        Task<Connection> GetClientByConnectionIdAsync(string clientConnectionId);
        Task AddClientAsync(Connection client);
        Task UpdateClientAsync(Connection client);
        Task DeleteClientAsync(Guid clientId);
        Task<List<Connection>> GetMemberClientsAsync(Guid memberId);
        Task<List<Connection>> GetClientsByMemberIdsAsync(List<Guid> memberIds);
    }
}