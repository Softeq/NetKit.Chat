using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnsureThat;
using Softeq.NetKit.Chat.Common.Cache;
using Softeq.NetKit.Chat.Domain.Client;
using Softeq.NetKit.Chat.Domain.Client.TransportModels;
using Softeq.NetKit.Chat.Domain.Client.TransportModels.Request;
using Softeq.NetKit.Chat.Domain.Client.TransportModels.Response;
using Softeq.NetKit.Chat.Domain.Member;
using Softeq.NetKit.Chat.Domain.Services.Exceptions;
using Softeq.NetKit.Chat.Domain.Services.Exceptions.ErrorHandling;

namespace Softeq.NetKit.Chat.Domain.Services.Client
{
    class CacheClientService : IClientService
    {
        private readonly IDistributedCacheClient _distributedCacheClient;
        private readonly IMemberService _memberService;
        public CacheClientService(IDistributedCacheClient distributedCacheClient, IMemberService memberService)
        {
            _distributedCacheClient = distributedCacheClient;
            _memberService = memberService;
        }
        public async Task DeleteClientAsync(DeleteClientRequest request)
        {
            var userCache = await GetUserConnectionCache(request.SaasUserId, request.ClientConnectionId);
            Ensure.That(userCache).WithException(x => new NotFoundException(new ErrorDto(ErrorCode.NotFound, "Client does not exist.")));
            userCache.Clients.Remove(userCache.Clients.First(i => i.ClientConnectionId == request.ClientConnectionId));
            await SaveUserConnectionCache(userCache, request.ClientConnectionId);
        }
        public async Task<ClientResponse> GetOrAddClientAsync(AddClientRequest request)
        {
            var userCache = await GetUserConnectionCache(request.SaasUserId, request.ConnectionId);
            var client = userCache.Clients.FirstOrDefault(i => i.ClientConnectionId == request.ConnectionId);
            if (client != null)
            {
                return client.ToClientResponse(request.SaasUserId);
            }
            client = new Domain.Client.Client
            {
                Id = Guid.NewGuid(),
                ClientConnectionId = request.ConnectionId,
                Name = request.UserName,
                UserAgent = request.UserAgent
            };
            userCache.Clients.Add(client);
            await SaveUserConnectionCache(userCache, request.SaasUserId);
            return client.ToClientResponse(request.SaasUserId);
        }

        public async Task UpdateActivityAsync(AddClientRequest request)
        {
            var userCache = await GetUserConnectionCache(request.SaasUserId, request.ConnectionId);
            var client = userCache.Clients.FirstOrDefault(i => i.ClientConnectionId == request.ConnectionId);
            Ensure.That(client).WithException(x => new NotFoundException(new ErrorDto(ErrorCode.NotFound, "Client does not exist.")));
            client.UserAgent = request.UserAgent;
            client.LastClientActivity = DateTimeOffset.Now;
            await SaveUserConnectionCache(userCache, request.ConnectionId);
        }
        public async Task<IEnumerable<Domain.Client.Client>> GetMemberClientsAsync(String saasUserId)
        {
            var userCache = await GetUserConnectionCache(saasUserId, saasUserId);
            var clients = userCache.Clients;
            return clients;
        }
        private async Task<ClientCache> GetUserConnectionCache(string saasUserId, String connectionId)
        {
            var cache = new ClientCache();
            var userConnections = await _distributedCacheClient.HashGetAsync<List<Domain.Client.Client>>(saasUserId, connectionId);
            if (userConnections == null)
            {
                userConnections = new List<Domain.Client.Client>();
            }
            cache.SaasUserId = saasUserId;
            cache.Clients = userConnections;
            return cache;
        }
        private async Task SaveUserConnectionCache(ClientCache userClients, String connectionId)
        {
            await _distributedCacheClient.HashSetAsync<List<Domain.Client.Client>>(userClients.SaasUserId, userClients.SaasUserId, userClients.Clients);
        }
    }
}
