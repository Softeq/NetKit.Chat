using System.Threading.Tasks;
using Softeq.NetKit.Chat.Domain.TransportModels.Request.Client;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.Client;

namespace Softeq.NetKit.Chat.Domain.Services.DomainServices
{
    public interface IClientService
    {
        Task<ClientResponse> AddClientAsync(AddClientRequest request);
        Task<ClientResponse> GetClientAsync(string saasUserId, string clientConnectionId);
        Task DeleteClientAsync(string clientConnectionId);
    }
}