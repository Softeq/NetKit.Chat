// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EnsureThat;
using Softeq.NetKit.Chat.Client.SDK.Enums;
using Softeq.NetKit.Chat.Client.SDK.Models.CommonModels.Request.Client;
using Softeq.NetKit.Chat.Client.SDK.Models.SignalRModels.Client;
using Softeq.NetKit.Chat.Data.Persistent;
using Softeq.NetKit.Chat.Domain.Exceptions;
using Softeq.NetKit.Chat.Domain.Services.Mappings;
using Softeq.NetKit.Chat.Domain.Services.Utility;
using Softeq.NetKit.Chat.Domain.TransportModels.Request.Client;
using AddClientRequest = Softeq.NetKit.Chat.Domain.TransportModels.Request.Client.AddClientRequest;

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
                throw new NetKitChatNotFoundException($"Unable to get client. Client {nameof(request.ClientConnectionId)}:{request.ClientConnectionId} is not found.");
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

            await _memberService.UpdateMemberStatusAsync(member.SaasUserId, UserStatus.Online);

            var isClientExists = await UnitOfWork.ClientRepository.IsClientExistsAsync(request.ConnectionId);
            if (isClientExists)
            {
                throw new NetKitChatInvalidOperationException($"Unable to add client. Client {nameof(request.ConnectionId)}:{request.ConnectionId} already exists.");
            }

            var client = new DomainModels.Client
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
                throw new NetKitChatNotFoundException($"Unable to delete client. Client {nameof(request.ClientConnectionId)}:{request.ClientConnectionId} is not found.");
            }

            await UnitOfWork.ClientRepository.DeleteClientAsync(client.Id);

            //TODO made for remove broken or not closed connections
            await RemoveInactiveConnectionsAsync(client.MemberId);

            var memberHasClients = await UnitOfWork.ClientRepository.DoesMemberHasClientsAsync(client.MemberId);
            if (!memberHasClients)
            {
                await _memberService.UpdateMemberStatusAsync(client.Member.SaasUserId, UserStatus.Offline);
            }
        }

        private async Task RemoveInactiveConnectionsAsync(Guid memberId)
        {
            const int inactiveMinutesThreshold = 60;
            await UnitOfWork.ClientRepository.DeleteOverThresholdMemberClientsAsync(memberId, inactiveMinutesThreshold);
        }

        public async Task<IReadOnlyCollection<string>> GetNotMutedChannelClientConnectionIdsAsync(Guid channelId)
        {
            return await UnitOfWork.ClientRepository.GetNotMutedChannelClientConnectionIdsAsync(channelId);
        }

        public async Task<IReadOnlyCollection<string>> GetChannelClientConnectionIdsAsync(Guid channelId)
        {
            return await UnitOfWork.ClientRepository.GetChannelClientConnectionIdsAsync(channelId);
        }

        public async Task<IReadOnlyCollection<string>> GetChannelMemberClientConnectionIdsAsync(Guid channelId, Guid mermberId)
        {
            return await UnitOfWork.ClientRepository.GetChannelClientConnectionIdsAsync(channelId);
        }
    }
}
