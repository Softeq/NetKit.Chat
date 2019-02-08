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
using Softeq.NetKit.Chat.Domain.TransportModels.Request.DirectChannel;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.DirectMessage;

namespace Softeq.NetKit.Chat.Domain.Services.DomainServices
{
    internal class DirectChannelService : BaseService, IDirectChannelService
    {
        private readonly IDateTimeProvider _dateTimeProvider;

        public DirectChannelService(
            IUnitOfWork unitOfWork,
            IDomainModelsMapper domainModelsMapper,
            IDateTimeProvider dateTimeProvider)
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

            await UnitOfWork.DirectChannelRepository.CreateDirectChannelAsync(request.DirectChannelId, request.OwnerId, request.MemberId);

            return DomainModelsMapper.MapToDirectChannelResponse(request.DirectChannelId, owner, member);
        }

        public async Task<DirectChannelResponse> GetDirectChannelByIdAsync(Guid channelId)
        {
            var channel = await UnitOfWork.DirectChannelRepository.GetDirectChannelAsync(channelId);
            if (channel == null)
            {
                throw new NetKitChatNotFoundException($"Unable to get direct channel. Chat with {nameof(channelId)}:{channelId} is not found.");
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
            var channel = await UnitOfWork.DirectChannelRepository.GetDirectChannelAsync(request.DirectChannelId);
            if (channel == null)
            {
                throw new NetKitChatNotFoundException($"Unable to add direct message. Channel { nameof(request.DirectChannelId) }:{ request.DirectChannelId} is not found.");
            }

            var owner = await UnitOfWork.MemberRepository.GetMemberBySaasUserIdAsync(request.SaasUserId);
            if (owner == null)
            {
                throw new NetKitChatNotFoundException($"Unable to get member. Member { nameof(request.SaasUserId) }:{ request.SaasUserId} is not found.");
            }

            var message = new Message
            {
                Id = Guid.NewGuid(),
                DirectChannelId = request.DirectChannelId,
                OwnerId = owner.Id,
                Body = request.Body,
                Created = _dateTimeProvider.GetUtcNow(),
                Type = request.Type,
                AccessibilityStatus = AccessibilityStatus.Present,
                Updated = _dateTimeProvider.GetUtcNow(),
                ChannelType = ChannelTypes.Direct
            };

            await UnitOfWork.MessageRepository.AddMessageAsync(message);

            return DomainModelsMapper.MapToDirectMessageResponse(message);
        }

        public async Task<DirectMessageResponse> GetMessageAsync(Guid messageId)
        {
            var message = await UnitOfWork.MessageRepository.GetMessageWithOwnerAndForwardMessageAsync(messageId);
            if (message == null)
            {
                throw new NetKitChatNotFoundException($"Unable to get direct message. Message with {nameof(messageId)}:{messageId} is not found.");
            }

            var owner = await UnitOfWork.MemberRepository.GetMemberByIdAsync(message.OwnerId);
            if (owner == null)
            {
                throw new NetKitChatNotFoundException($"Unable to get member. Member { nameof(message.OwnerId) }:{ message.OwnerId} is not found.");
            }

            return DomainModelsMapper.MapToDirectMessageResponse(message);
        }

        public async Task<DirectMessageResponse> ArchiveMessageAsync(Guid messageId, string saasUserId)
        {
            var directMessage = await UnitOfWork.MessageRepository.GetMessageWithOwnerAndForwardMessageAsync(messageId);
            if (directMessage == null)
            {
                throw new NetKitChatNotFoundException($"Unable to get direct message. Message with {nameof(messageId)}:{messageId} is not found.");
            }
            var owner = await UnitOfWork.MemberRepository.GetMemberBySaasUserIdAsync(saasUserId);
            if (owner == null)
            {
                throw new NetKitChatNotFoundException($"Unable to get member {nameof(saasUserId)}:{saasUserId} is not found.");
            }

            await UnitOfWork.MessageRepository.ArchiveMessageAsync(messageId);

            return DomainModelsMapper.MapToDirectMessageResponse(directMessage);
        }

        public async Task<DirectMessageResponse> UpdateMessageAsync(UpdateDirectMessageRequest request)
        {
            var directMessage = await UnitOfWork.MessageRepository.GetMessageWithOwnerAndForwardMessageAsync(request.MessageId);
            if (directMessage == null)
            {
                throw new NetKitChatNotFoundException($"Unable to get direct message. Message with {nameof(request.MessageId)}:{request.MessageId} is not found.");
            }
            var owner = await UnitOfWork.MemberRepository.GetMemberBySaasUserIdAsync(request.SaasUserId);
            if (owner == null)
            {
                throw new NetKitChatNotFoundException($"Unable to get member. Member { nameof(directMessage.OwnerId) }:{ directMessage.OwnerId} is not found.");
            }

            var message = new Message
            {
                Body = request.Body,
                Created = directMessage.Created,
                DirectChannelId = directMessage.DirectChannelId,
                Id = directMessage.Id,
                OwnerId = directMessage.OwnerId,
                Updated = _dateTimeProvider.GetUtcNow()
            };

            await UnitOfWork.MessageRepository.UpdateMessageBodyAsync(request.MessageId, request.Body, _dateTimeProvider.GetUtcNow());

            return DomainModelsMapper.MapToDirectMessageResponse(message);
        }

        public async Task<IList<DirectMessageResponse>> GetChannelMessagesAsync(Guid channelId)
        {
            var channel = await UnitOfWork.DirectChannelRepository.GetDirectChannelAsync(channelId);
            if (channel == null)
            {
                throw new NetKitChatNotFoundException($"Unable to add direct message. Channel { nameof(channelId) }:{ channelId} is not found.");
            }

            var messages = await UnitOfWork.MessageRepository.GetAllDirectChannelMessagesWithOwnersAsync(channelId);

            var directMessagesResponse = new List<DirectMessageResponse>();

            if (messages != null)
            {
                var uniqueMembersId = messages.GroupBy(x => x.OwnerId).Select(x => x.FirstOrDefault()).Select(y => y.OwnerId);
                foreach (var ownerId in uniqueMembersId)
                {
                    var owner = await UnitOfWork.MemberRepository.GetMemberByIdAsync(ownerId);
                    if (owner == null)
                    {
                        throw new NetKitChatNotFoundException($"Unable to get member. Member {nameof(ownerId)}:{ownerId} is not found.");
                    }

                    directMessagesResponse.AddRange(messages.Where(message => message.OwnerId == ownerId)
                        .Select(message => DomainModelsMapper.MapToDirectMessageResponse(message)));
                }
            }

            return directMessagesResponse;
        }
    }
}
