// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EnsureThat;
using Softeq.NetKit.Chat.Data.Persistent;
using Softeq.NetKit.Chat.Domain.DomainModels;
using Softeq.NetKit.Chat.Domain.Exceptions;
using Softeq.NetKit.Chat.Domain.Services.Mappers;
using Softeq.NetKit.Chat.Domain.TransportModels.Request.Client;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.Client;

namespace Softeq.NetKit.Chat.Domain.Services.DomainServices
{
    internal class ClientService : BaseService, IClientService
    {
        private readonly IMemberService _memberService;

        public ClientService(IUnitOfWork unitOfWork, IMemberService memberService)
            : base(unitOfWork)
        {
            Ensure.That(memberService).IsNotNull();

            _memberService = memberService;
        }

        public async Task<ClientResponse> GetClientAsync(GetClientRequest request)
        {
            var client = await UnitOfWork.ClientRepository.GetClientWithMemberAsync(request.ClientConnectionId);
            if (client == null)
            {
                throw new NetKitChatNotFoundException($"Unable to get client. Client {nameof(request.ClientConnectionId)}:{request.ClientConnectionId} not found.");
            }

            return client.ToClientResponse();
        }

        public async Task<ClientResponse> AddClientAsync(AddClientRequest request)
        {
            var member = await UnitOfWork.MemberRepository.GetMemberBySaasUserIdAsync(request.SaasUserId);
            if (member == null)
            {
                throw new NetKitChatNotFoundException($"Unable to add client. Member {nameof(request.SaasUserId)}:{request.SaasUserId} not found.");
            }

            await _memberService.UpdateMemberStatusAsync(member.SaasUserId, UserStatus.Active);

            var isClientExists = await UnitOfWork.ClientRepository.IsClientExistsAsync(request.ConnectionId);
            if (isClientExists)
            {
                throw new NetKitChatInvalidOperationException($"Unable to add client. Client {nameof(request.ConnectionId)}:{request.ConnectionId} already exists.");
            }

            var client = new Client
            {
                Id = Guid.NewGuid(),
                MemberId = member.Id,
                Member = member,
                ClientConnectionId = request.ConnectionId,
                LastActivity = member.LastActivity,
                LastClientActivity = DateTimeOffset.UtcNow,
                Name = request.UserName,
                UserAgent = request.UserAgent
            };

            await UnitOfWork.ClientRepository.AddClientAsync(client);
            return client.ToClientResponse();
        }

        public async Task DeleteClientAsync(DeleteClientRequest request)
        {
            var client = await UnitOfWork.ClientRepository.GetClientWithMemberAsync(request.ClientConnectionId);
            if (client == null)
            {
                throw new NetKitChatNotFoundException($"Unable to delete client. Client {nameof(request.ClientConnectionId)}:{request.ClientConnectionId} not found.");
            }

            await UnitOfWork.ClientRepository.DeleteClientAsync(client.Id);

            var clients = await _memberService.GetMemberClientsAsync(client.MemberId);
            if (!clients.Any())
            {
                await _memberService.UpdateMemberStatusAsync(client.Member.SaasUserId, UserStatus.Offline);
            }
        }

        public async Task<IReadOnlyCollection<string>> GetNotMutedChannelClientConnectionIdsAsync(Guid channelId)
        {
            return await UnitOfWork.ClientRepository.GetNotMutedChannelClientConnectionIdsAsync(channelId);
        }
    }
}