using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Softeq.NetKit.Chat.Client.SDK.Models.CommonModels.Request.Client;
using Softeq.NetKit.Chat.Client.SDK.Models.SignalRModels.Client;
using Softeq.NetKit.Chat.Domain.TransportModels.Request.Client;
using AddClientRequest = Softeq.NetKit.Chat.Domain.TransportModels.Request.Client.AddClientRequest;


namespace Softeq.NetKit.Chat.Domain.Services.DomainServices
{
    public interface IClientService
    {
        Task<ClientResponse> AddClientAsync(AddClientRequest request);
        Task<ClientResponse> GetClientAsync(GetClientRequest request);
        Task DeleteClientAsync(DeleteClientRequest request);
        Task<IReadOnlyCollection<string>> GetNotMutedChannelClientConnectionIdsAsync(Guid channelId);
        Task<IReadOnlyCollection<string>> GetChannelClientConnectionIdsAsync(Guid channelId);
        Task<IReadOnlyCollection<string>> GetChannelMemberClientConnectionIdsAsync(Guid channelId, Guid memberId);
    }
}
