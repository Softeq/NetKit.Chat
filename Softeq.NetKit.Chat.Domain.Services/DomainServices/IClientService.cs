using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Softeq.NetKit.Chat.Domain.TransportModels.Request.Client;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.Client;

namespace Softeq.NetKit.Chat.Domain.Services.DomainServices
{
    public interface IClientService
    {
        Task<ClientResponse> AddClientAsync(AddClientRequest request);
        Task<ClientResponse> GetClientAsync(GetClientRequest request);
        Task DeleteClientAsync(DeleteClientRequest request);
        Task<IReadOnlyCollection<string>> GetNotMutedChannelClientConnectionIdsAsync(Guid channelId);
    }
}