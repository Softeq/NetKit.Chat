// Developed by Softeq Development Corporation
// http://www.softeq.com

using Softeq.NetKit.Chat.Data.Persistent;
using Softeq.NetKit.Chat.Domain.DomainModels;
using Softeq.NetKit.Chat.Domain.Exceptions;
using Softeq.NetKit.Chat.Domain.Services.Mappings;
using Softeq.NetKit.Chat.Domain.Services.Utility;
using Softeq.NetKit.Chat.Domain.TransportModels.Request.DirectChannel;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.DirectMessage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EnsureThat;

namespace Softeq.NetKit.Chat.Domain.Services.DomainServices
{
    internal class DirectChannelService : BaseService, IDirectChannelService
    {
        private readonly IDateTimeProvider _dateTimeProvider;

        public DirectChannelService(
            IUnitOfWork unitOfWork,
            IDomainModelsMapper domainModelsMapper, IDateTimeProvider dateTimeProvider)
            : base(unitOfWork, domainModelsMapper)
        {
            Ensure.That(dateTimeProvider).IsNotNull();

            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<DirectChannelResponse> CreateDirectChannelAsync(CreateDirectChannelRequest request)
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

        public async Task<DirectChannelResponse> GetDirectChannelByIdAsync(Guid id)
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

        public async Task<DirectMessageResponse> AddMessageAsync(CreateDirectMessageRequest request)
        {
            var channel = await UnitOfWork.DirectChannelRepository.GetDirectChannelById(request.DirectChannelId);
            if (channel == null)
            {
                throw new NetKitChatNotFoundException($"Unable to add direct message. Channel { nameof(request.DirectChannelId) }:{ request.DirectChannelId} is not found.");
            }

            var owner = await UnitOfWork.MemberRepository.GetMemberBySaasUserIdAsync(request.SaasUserId);
            if (owner == null)
            {
                throw new NetKitChatNotFoundException($"Unable to get member. Member { nameof(request.SaasUserId) }:{ request.SaasUserId} is not found.");
            }
            var message = new DirectMessage
            {
                Body = request.Body,
                Created = _dateTimeProvider.GetUtcNow(),
                DirectChannelId = request.DirectChannelId,
                Id = Guid.NewGuid(),
                OwnerId = owner.Id,
                Updated = _dateTimeProvider.GetUtcNow()
            };

            await UnitOfWork.DirectMessagesRepository.AddMessageAsync(message);

            return DomainModelsMapper.MapToDirectMessageResponse(message, owner);
        }

        public async Task<DirectMessageResponse> GetMessagesByIdAsync(Guid messageId)
        {
            var directMessage = await UnitOfWork.DirectMessagesRepository.GetMessageByIdAsync(messageId);
            if (directMessage == null)
            {
                throw new NetKitChatNotFoundException($"Unable to get direct message. Message with {nameof(messageId)}:{messageId} is not found.");
            }

            var owner = await UnitOfWork.MemberRepository.GetMemberByIdAsync(directMessage.OwnerId);
            if (owner == null)
            {
                throw new NetKitChatNotFoundException($"Unable to get member. Member { nameof(directMessage.OwnerId) }:{ directMessage.OwnerId} is not found.");
            }

            return DomainModelsMapper.MapToDirectMessageResponse(directMessage, owner);
        }

        public async Task<DirectMessageResponse> DeleteMessageAsync(Guid id, string saasUserId)
        {
            var directMessage = await UnitOfWork.DirectMessagesRepository.GetMessageByIdAsync(id);
            if (directMessage == null)
            {
                throw new NetKitChatNotFoundException($"Unable to get direct message. Message with {nameof(id)}:{id} is not found.");
            }
            var owner = await UnitOfWork.MemberRepository.GetMemberBySaasUserIdAsync(saasUserId);
            if (owner == null)
            {
                throw new NetKitChatNotFoundException($"Unable to get member {nameof(saasUserId)}:{saasUserId} is not found.");
            }

            await UnitOfWork.DirectMessagesRepository.DeleteMessageAsync(id);

            return DomainModelsMapper.MapToDirectMessageResponse(directMessage, owner);
        }

        public async Task<DirectMessageResponse> UpdateMessageAsync(UpdateDirectMessageRequest request)
        {
            var directMessage = await UnitOfWork.DirectMessagesRepository.GetMessageByIdAsync(request.MessageId);
            if (directMessage == null)
            {
                throw new NetKitChatNotFoundException($"Unable to get direct message. Message with {nameof(request.MessageId)}:{request.MessageId} is not found.");
            }
            var owner = await UnitOfWork.MemberRepository.GetMemberBySaasUserIdAsync(request.SaasUserId);
            if (owner == null)
            {
                throw new NetKitChatNotFoundException($"Unable to get member. Member { nameof(directMessage.OwnerId) }:{ directMessage.OwnerId} is not found.");
            }

            var message = new DirectMessage
            {
                Body = request.Body,
                Created = directMessage.Created,
                DirectChannelId = directMessage.DirectChannelId,
                Id = directMessage.Id,
                OwnerId = directMessage.OwnerId,
                Updated = _dateTimeProvider.GetUtcNow()
            };

            await UnitOfWork.DirectMessagesRepository.UpdateMessageAsync(message);

            return DomainModelsMapper.MapToDirectMessageResponse(message, owner);
        }

        public async Task<IList<DirectMessageResponse>> GetMessagesByChannelIdAsync(Guid channelId)
        {
            var channel = await UnitOfWork.DirectChannelRepository.GetDirectChannelById(channelId);
            if (channel == null)
            {
                throw new NetKitChatNotFoundException($"Unable to add direct message. Channel { nameof(channelId) }:{ channelId} is not found.");
            }

            var messages = await UnitOfWork.DirectMessagesRepository.GetMessagesByChannelIdAsync(channelId);

            var uniqueMembersId = messages.GroupBy(x => x.OwnerId).Select(x => x.FirstOrDefault()).Select(y => y.OwnerId);
            var directMessagesResponse = new List<DirectMessageResponse>();

            foreach (var ownerId in uniqueMembersId)
            {
                var owner = await UnitOfWork.MemberRepository.GetMemberByIdAsync(ownerId);

                if (owner == null)
                {
                    throw new NetKitChatNotFoundException($"Unable to get member. Member {nameof(ownerId)}:{ownerId} is not found.");
                }

                directMessagesResponse.AddRange(GetDirectMessageResponses(messages, owner));
            }

            return directMessagesResponse;
        }

        private List<DirectMessageResponse> GetDirectMessageResponses(IReadOnlyList<DirectMessage> messages, DomainModels.Member owner)
        {
            var directMessageResponses = new List<DirectMessageResponse>();

            foreach (var directMessage in messages)
            {
                if (directMessage.OwnerId == owner.Id)
                {
                    directMessageResponses.Add(DomainModelsMapper.MapToDirectMessageResponse(directMessage, owner));
                }
            }

            return directMessageResponses;
        }
    }
}
