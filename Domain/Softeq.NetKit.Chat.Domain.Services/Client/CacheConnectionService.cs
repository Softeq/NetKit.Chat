using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Softeq.NetKit.Chat.Common.Cache;
using Softeq.NetKit.Chat.Domain.Member;
using Softeq.NetKit.Chat.Data.Interfaces.SocketConnection;
using Softeq.NetKit.Chat.Data.Interfaces.UnitOfWork;
using Softeq.NetKit.Chat.Domain.Client.TransportModels.Request;
using Softeq.NetKit.Chat.Domain.Client.TransportModels.Response;
using EnsureThat;
using Softeq.NetKit.Chat.Domain.Client.TransportModels;
using Softeq.NetKit.Chat.Domain.Services.Exceptions;
using Softeq.NetKit.Chat.Domain.Services.Exceptions.ErrorHandling;

namespace Softeq.NetKit.Chat.Domain.Services.Client
{
    public class CacheConnectionService : IClientService
    {
        private readonly IDistributedCacheClient _distributedCacheClient;
        private readonly IMemberService _memberService;
        public CacheConnectionService(IDistributedCacheClient distributedCacheClient, IMemberService memberService)
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
                MemberId = Guid.NewGuid(),
                ClientConnectionId = request.ConnectionId,
                Name = request.UserName,
                UserAgent = request.UserAgent
            };
            userCache.Clients.Add(client);
            await SaveUserConnectionCache(userCache, request.ConnectionId);
            return client.ToClientResponse(request.SaasUserId);
        }

        public async Task UpdateActivityAsync(AddClientRequest request)
        {
            var userCache = await GetUserConnectionCache(request.SaasUserId, request.ConnectionId);

            var client = userCache.Clients.FirstOrDefault(i => i.ClientConnectionId == request.ConnectionId);
            Ensure.That(client).WithException(x => new NotFoundException(new ErrorDto(ErrorCode.NotFound, "Client does not exist.")));
            client.UserAgent = request.UserAgent;

            await SaveUserConnectionCache(userCache, request.ConnectionId);
        }

        private async Task<UserConnectionCache> GetUserConnectionCache(string saasUserId, String connectionId)
        {
            var userConnectionCache = await _distributedCacheClient.HashGetAsync<UserConnectionCache>(saasUserId, connectionId);
            if (userConnectionCache==null)
            {
                userConnectionCache = new UserConnectionCache()
                {
                    SaasUserId = saasUserId,
                    Clients = new List<Domain.Client.Client>()
                };
            }
            return userConnectionCache;
        }

        private async Task SaveUserConnectionCache(UserConnectionCache userClients, String connectionId)
        {
            await _distributedCacheClient.HashSetAsync<List<Domain.Client.Client>>(userClients.SaasUserId, connectionId, userClients.Clients);
        }
    }
}