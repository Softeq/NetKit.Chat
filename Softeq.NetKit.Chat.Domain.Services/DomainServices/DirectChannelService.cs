// Developed by Softeq Development Corporation
// http://www.softeq.com

using EnsureThat;
using Softeq.NetKit.Chat.Data.Persistent;
using Softeq.NetKit.Chat.Data.Persistent.Repositories;
using Softeq.NetKit.Chat.Domain.Exceptions;
using Softeq.NetKit.Chat.Domain.Services.Mappings;
using Softeq.NetKit.Chat.Domain.Services.Utility;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Softeq.NetKit.Chat.Domain.DomainModels;
using Softeq.NetKit.Chat.Domain.TransportModels.Request.DirectChannel;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.DirectMessage;

namespace Softeq.NetKit.Chat.Domain.Services.DomainServices
{
    internal class DirectChannelService : BaseService, IDirectChannelService
    {
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IDirectChannelRepository _directChannelRepository;

        public DirectChannelService(
            IUnitOfWork unitOfWork,
            IDomainModelsMapper domainModelsMapper,
            IDateTimeProvider dateTimeProvider)
            : base(unitOfWork, domainModelsMapper)
        {
            Ensure.That(dateTimeProvider).IsNotNull();

            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<DirectChannelResponse> CreateDirectChannel(CreateDirectChannelRequest request)
        {
            var owner = await UnitOfWork.MemberRepository.GetMemberBySaasUserIdAsync(request.SaasUserId);
            if (owner == null)
            {
                throw new NetKitChatNotFoundException($"Unable to create direct channel. Member { nameof(request.SaasUserId) }:{ request.SaasUserId} is not found.");
            }

            var member = await UnitOfWork.MemberRepository.GetMemberByIdAsync(request.MemberId);
            if (member == null)
            {
                throw new NetKitChatNotFoundException($"Unable to create direct channel. Member { nameof(request.MemberId) }:{ request.MemberId} is not found.");
            }

            await UnitOfWork.DirectChannelRepository.CreateDirectChannel(request.DirectChannelId, request.OwnerId, request.MemberId);

            return DomainModelsMapper.MapToDirectChannelResponse(request.DirectChannelId, owner, member);
        }

        public async Task<DirectChannelResponse> GetDirectChannelById(Guid id)
        {
            var channel = await UnitOfWork.DirectChannelRepository.GetDirectChannelById(id);
            if (channel == null)
            {
                throw new NetKitChatNotFoundException($"Unable to get direct channel. Chat with {nameof(id)}:{id} is not found.");
            }

            var owner = await UnitOfWork.MemberRepository.GetMemberByIdAsync(channel.OwnerId);
            if (owner == null)
            {
                throw new NetKitChatNotFoundException($"Unable to get member {nameof(channel.OwnerId)}:{channel.OwnerId} is not found.");
            }

            var member = await UnitOfWork.MemberRepository.GetMemberByIdAsync(channel.MemberId);
            if (member == null)
            {
                throw new NetKitChatNotFoundException($"Unable to get member {nameof(channel.MemberId)}:{channel.MemberId} is not found.");
            }

            return DomainModelsMapper.MapToDirectChannelResponse(channel.Id, owner, member);
        }

        public async Task<DirectMessageResponse> AddMessageAsync(DirectMessage message)
        {
            var channel = await UnitOfWork.DirectChannelRepository.GetDirectChannelById(message.DirectChannelId);
            if (channel == null)
            {
                throw new NetKitChatNotFoundException($"Unable to add direct message. Channel { nameof(message.DirectChannelId) }:{ message.DirectChannelId} is not found.");
            }

            var owner = await UnitOfWork.MemberRepository.GetMemberByIdAsync(message.OwnerId);
            if (owner == null)
            {
                throw new NetKitChatNotFoundException($"Unable to create direct channel. Member { nameof(message.OwnerId) }:{ message.OwnerId} is not found.");
            }

            await UnitOfWork.DirectMessagesRepository.AddMessageAsync(message);

            return DomainModelsMapper.MapToDirectMessageResponse(message, owner);
        }

        public Task DeleteMessageAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task UpdateMessageAsync(DirectMessage message)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<DirectMessage>> GetMessagesByChannelIdAsync(Guid channelId)
        {
            throw new NotImplementedException();
        }

        public Task<DirectMessage> GetMessagesByIdAsync(Guid messageId)
        {
            throw new NotImplementedException();
        }
    }
}
