using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Softeq.NetKit.Chat.Domain.Client.TransportModels.Request;
using Softeq.NetKit.Chat.Domain.Client.TransportModels.Response;

namespace Softeq.NetKit.Chat.Domain.Client
{
    public interface IClientService
    {
        Task<ClientResponse> GetOrAddClientAsync(AddClientRequest request);
        Task DeleteClientAsync(DeleteClientRequest request);
        Task UpdateActivityAsync(AddClientRequest request);
        Task<IEnumerable<Client>> GetMemberClientsAsync(String saasUserId);
    }
}
