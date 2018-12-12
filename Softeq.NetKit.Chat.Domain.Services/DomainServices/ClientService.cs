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
using Softeq.NetKit.Chat.Domain.Services.Mappings;
using Softeq.NetKit.Chat.Domain.Services.Utility;
using Softeq.NetKit.Chat.Domain.TransportModels.Request.Client;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.Client;

namespace Softeq.NetKit.Chat.Domain.Services.DomainServices
{
    internal class ClientService : BaseService, IClientService
    {
        private readonly IMemberService _memberService;
        private readonly IDateTimeProvider _dateTimeProvider;

        public ClientService(
            IUnitOfWork unitOfWork,
            IDomainModelsMapper domainModelsMapper,
            IMemberService memberService,
            IDateTimeProvider dateTimeProvider)
            : base(unitOfWork, domainModelsMapper)
        {
            Ensure.That(memberService).IsNotNull();
            Ensure.That(dateTimeProvider).IsNotNull();

            _memberService = memberService;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<ClientResponse> GetClientAsync(GetClientRequest request)
        {
            var client = await UnitOfWork.ClientRepository.GetClientWithMemberAsync(request.ClientConnectionId);
            if (client == null)
            {
                throw new NetKitChatNotFoundException($"Unable to get client. Client {nameof(request.ClientConnectionId)}:{request.ClientConnectionId} not found.");
            }

            return DomainModelsMapper.MapToClientResponse(client);
        }

        public async Task<ClientResponse> AddClientAsync(AddClientRequest request)
        {
            var member = await UnitOfWork.MemberRepository.GetMemberBySaasUserIdAsync(request.SaasUserId);
            if (member == null)
            {
                await _memberService.AddMemberAsync(request.SaasUserId, request.Email);
                member = await UnitOfWork.MemberRepository.GetMemberBySaasUserIdAsync(request.SaasUserId);
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
                LastClientActivity = _dateTimeProvider.GetUtcNow(),
                Name = request.UserName,
                UserAgent = request.UserAgent
            };

            await UnitOfWork.ClientRepository.AddClientAsync(client);
            return DomainModelsMapper.MapToClientResponse(client);
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