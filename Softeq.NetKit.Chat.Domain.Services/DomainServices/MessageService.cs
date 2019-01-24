﻿// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using EnsureThat;
using Softeq.NetKit.Chat.Data.Cloud.DataProviders;
using Softeq.NetKit.Chat.Data.Persistent;
using Softeq.NetKit.Chat.Domain.DomainModels;
using Softeq.NetKit.Chat.Domain.Exceptions;
using Softeq.NetKit.Chat.Domain.Services.Configuration;
using Softeq.NetKit.Chat.Domain.Services.Mappings;
using Softeq.NetKit.Chat.Domain.Services.Utility;
using Softeq.NetKit.Chat.Domain.TransportModels.Request.Message;
using Softeq.NetKit.Chat.Domain.TransportModels.Request.MessageAttachment;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.Message;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.MessageAttachment;

namespace Softeq.NetKit.Chat.Domain.Services.DomainServices
{
    internal class MessageService : BaseService, IMessageService
    {
        private readonly MessagesConfiguration _messagesConfiguration;
        private readonly ICloudAttachmentProvider _cloudAttachmentProvider;
        private readonly ICloudImageProvider _cloudImageProvider;
        private readonly IDateTimeProvider _dateTimeProvider;

        public MessageService(
            IUnitOfWork unitOfWork,
            IDomainModelsMapper domainModelsMapper,
            MessagesConfiguration messagesConfiguration,
            ICloudAttachmentProvider cloudAttachmentProvider,
            ICloudImageProvider cloudImageProvider,
            IDateTimeProvider dateTimeProvider)
            : base(unitOfWork, domainModelsMapper)
        {
            Ensure.That(messagesConfiguration).IsNotNull();
            Ensure.That(cloudAttachmentProvider).IsNotNull();
            Ensure.That(dateTimeProvider).IsNotNull();

            _messagesConfiguration = messagesConfiguration;
            _cloudAttachmentProvider = cloudAttachmentProvider;
            _cloudImageProvider = cloudImageProvider;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<MessageResponse> CreateMessageAsync(CreateMessageRequest request)
        {
            var isChannelExists = await UnitOfWork.ChannelRepository.IsChannelExistsAsync(request.ChannelId);
            if (!isChannelExists)
            {
                throw new NetKitChatNotFoundException($"Unable to create message. Channel {nameof(request.ChannelId)}:{request.ChannelId} is not found.");
            }

            var member = await UnitOfWork.MemberRepository.GetMemberBySaasUserIdAsync(request.SaasUserId);
            if (member == null)
            {
                throw new NetKitChatNotFoundException($"Unable to create message. Member {nameof(request.SaasUserId)}:{request.SaasUserId} is not found.");
            }

            // move image to persistent container
            if (!string.IsNullOrWhiteSpace(request.ImageUrl))
            {
                request.ImageUrl = await _cloudImageProvider.CopyImageToDestinationContainerAsync(request.ImageUrl);
            }

            var message = new Message
            {
                Id = Guid.NewGuid(),
                ChannelId = request.ChannelId,
                OwnerId = member.Id,
                Body = request.Body,
                Created = _dateTimeProvider.GetUtcNow(),
                Type = request.Type,
                ImageUrl = request.ImageUrl
            };

            using (var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                if (request.Type == MessageType.Forward)
                {
                    var forwardedMessage = await UnitOfWork.MessageRepository.GetMessageWithOwnerAndForwardMessageAsync(request.ForwardedMessageId);
                    if (forwardedMessage == null)
                    {
                        throw new NetKitChatNotFoundException($"Unable to create message. Forward message {nameof(request.ForwardedMessageId)}:{request.ForwardedMessageId} is not found.");
                    }

                    var forwardMessage = DomainModelsMapper.MapToForwardMessage(forwardedMessage);
                    forwardMessage.Id = Guid.NewGuid();
                    await UnitOfWork.ForwardMessageRepository.AddForwardMessageAsync(forwardMessage);
                    message.ForwardMessageId = forwardMessage.Id;
                }

                await UnitOfWork.MessageRepository.AddMessageAsync(message);
                await UnitOfWork.ChannelMemberRepository.SetLastReadMessageAsync(member.Id, request.ChannelId, message.Id);

                transactionScope.Complete();
            }

            message = await UnitOfWork.MessageRepository.GetMessageWithOwnerAndForwardMessageAsync(message.Id);

            return DomainModelsMapper.MapToMessageResponse(message);
        }

        public async Task DeleteMessageAsync(DeleteMessageRequest request)
        {
            var message = await UnitOfWork.MessageRepository.GetMessageWithOwnerAndForwardMessageAsync(request.MessageId);
            if (message == null)
            {
                throw new NetKitChatNotFoundException($"Unable to delete message. Message {nameof(request.MessageId)}:{request.MessageId} is not found.");
            }

            var member = await UnitOfWork.MemberRepository.GetMemberBySaasUserIdAsync(request.SaasUserId);
            if (member == null)
            {
                throw new NetKitChatNotFoundException($"Unable to delete message. Member {nameof(request.SaasUserId)}:{request.SaasUserId} is not found.");
            }

            if (member.Id != message.OwnerId)
            {
                throw new NetKitChatAccessForbiddenException($"Unable to delete message. Message {nameof(request.MessageId)}:{request.MessageId} owner required.");
            }

            var messageAttachments = await UnitOfWork.AttachmentRepository.GetMessageAttachmentsAsync(message.Id);

            using (var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                if (message.Type == MessageType.Forward && message.ForwardMessageId.HasValue)
                {
                    await UnitOfWork.ForwardMessageRepository.DeleteForwardMessageAsync(message.ForwardMessageId.Value);
                }
                
                await UnitOfWork.AttachmentRepository.DeleteMessageAttachmentsAsync(message.Id);
                foreach (var attachment in messageAttachments)
                {
                    await _cloudAttachmentProvider.DeleteMessageAttachmentAsync(attachment.FileName);
                }

                var previousMessage = await UnitOfWork.MessageRepository.GetPreviousMessageAsync(message.ChannelId, message.OwnerId, message.Created);
                await UnitOfWork.ChannelMemberRepository.UpdateLastReadMessageAsync(message.Id, previousMessage?.Id);

                await UnitOfWork.MessageRepository.DeleteMessageAsync(message.Id);

                transactionScope.Complete();
            }
        }

        public async Task<MessageResponse> UpdateMessageAsync(UpdateMessageRequest request)
        {
            var message = await UnitOfWork.MessageRepository.GetMessageWithOwnerAndForwardMessageAsync(request.MessageId);
            if (message == null)
            {
                throw new NetKitChatNotFoundException($"Unable to update message. Message {nameof(request.MessageId)}:{request.MessageId} is not found.");
            }

            var member = await UnitOfWork.MemberRepository.GetMemberBySaasUserIdAsync(request.SaasUserId);
            if (member == null)
            {
                throw new NetKitChatNotFoundException($"Unable to update message. Member {nameof(request.SaasUserId)}:{request.SaasUserId} is not found.");
            }

            if (member.Id != message.OwnerId)
            {
                throw new NetKitChatAccessForbiddenException($"Unable to update message. Message {nameof(request.MessageId)}:{request.MessageId} owner required.");
            }

            message.Body = request.Body;
            message.Updated = _dateTimeProvider.GetUtcNow();

            await UnitOfWork.MessageRepository.UpdateMessageBodyAsync(message.Id, message.Body, message.Updated.Value);

            return DomainModelsMapper.MapToMessageResponse(message);
        }

        public async Task<MessageResponse> GetMessageByIdAsync(Guid messageId)
        {
            var message = await UnitOfWork.MessageRepository.GetMessageWithOwnerAndForwardMessageAsync(messageId);
            if (message == null)
            {
                throw new NetKitChatNotFoundException($"Unable to get message by {nameof(messageId)}. Message {nameof(messageId)}:{messageId} is not found.");
            }

            return DomainModelsMapper.MapToMessageResponse(message);
        }

        public async Task<AttachmentResponse> AddMessageAttachmentAsync(AddMessageAttachmentRequest request)
        {
            var message = await UnitOfWork.MessageRepository.GetMessageWithOwnerAndForwardMessageAsync(request.MessageId);
            if (message == null)
            {
                throw new NetKitChatNotFoundException($"Unable to add message attachment. Message {nameof(request.MessageId)}:{request.MessageId} is not found.");
            }

            var member = await UnitOfWork.MemberRepository.GetMemberBySaasUserIdAsync(request.SaasUserId);
            if (member == null)
            {
                throw new NetKitChatNotFoundException($"Unable to add message attachment. Member {nameof(request.SaasUserId)}:{request.SaasUserId} is not found.");
            }

            if (member.Id != message.OwnerId)
            {
                throw new NetKitChatAccessForbiddenException($"Unable to add message attachment. Message {nameof(request.MessageId)}:{request.MessageId} owner required.");
            }

            var isAttachmentLimitExceeded = await IsAttachmentLimitExceededAsync(message.Id);
            if (isAttachmentLimitExceeded)
            {
                throw new NetKitChatInvalidOperationException($"Unable to add message attachment. Attachment limit {_messagesConfiguration.MessageAttachmentsLimit} exceeded.");
            }

            var attachment = new Attachment
            {
                Id = Guid.NewGuid(),
                ContentType = request.ContentType,
                Created = _dateTimeProvider.GetUtcNow(),
                FileName = $"{Guid.NewGuid()}.{request.Extension}",
                MessageId = request.MessageId,
                Size = request.Size
            };

            // Save attachment in database
            await UnitOfWork.AttachmentRepository.AddAttachmentAsync(attachment);

            // Save attachment in cloud
            await _cloudAttachmentProvider.SaveAttachmentAsync(attachment.FileName, request.Content);

            return DomainModelsMapper.MapToAttachmentResponse(attachment);
        }

        public async Task DeleteMessageAttachmentAsync(DeleteMessageAttachmentRequest request)
        {
            var message = await UnitOfWork.MessageRepository.GetMessageWithOwnerAndForwardMessageAsync(request.MessageId);
            if (message == null)
            {
                throw new NetKitChatNotFoundException($"Unable to delete message attachment. Message {nameof(request.MessageId)}:{request.MessageId} is not found.");
            }

            var member = await UnitOfWork.MemberRepository.GetMemberBySaasUserIdAsync(request.SaasUserId);
            if (member == null)
            {
                throw new NetKitChatNotFoundException($"Unable to delete message attachment. Member {nameof(request.SaasUserId)}:{request.SaasUserId} is not found.");
            }

            if (member.Id != message.OwnerId)
            {
                throw new NetKitChatAccessForbiddenException($"Unable to delete message attachment. Message {nameof(request.MessageId)}:{request.MessageId} owner required.");
            }

            var attachment = await UnitOfWork.AttachmentRepository.GetAttachmentAsync(request.AttachmentId);
            if (attachment == null)
            {
                throw new NetKitChatNotFoundException($"Unable to delete message attachment. Attachment {nameof(request.AttachmentId)}:{request.AttachmentId} is not found.");
            }

            if (attachment.MessageId != message.Id)
            {
                throw new NetKitChatInvalidOperationException($"Unable to delete message attachment. Message {nameof(request.MessageId)}:{request.MessageId} does not contain attachment {nameof(request.AttachmentId)}:{request.AttachmentId}.");
            }

            // Delete message attachment from database
            await UnitOfWork.AttachmentRepository.DeleteAttachmentAsync(attachment.Id);

            // Delete attachment from cloud
            await _cloudAttachmentProvider.DeleteMessageAttachmentAsync(attachment.FileName);
        }

        public async Task SetLastReadMessageAsync(SetLastReadMessageRequest request)
        {
            var member = await UnitOfWork.MemberRepository.GetMemberBySaasUserIdAsync(request.SaasUserId);
            if (member == null)
            {
                throw new NetKitChatNotFoundException($"Unable to set last read message. Member {nameof(request.SaasUserId)}:{request.SaasUserId} is not found.");
            }

            await UnitOfWork.ChannelMemberRepository.SetLastReadMessageAsync(member.Id, request.ChannelId, request.MessageId);
        }

        public async Task<MessagesResult> GetOlderMessagesAsync(GetMessagesRequest request)
        {
            var member = await UnitOfWork.MemberRepository.GetMemberBySaasUserIdAsync(request.SaasUserId);
            if (member == null)
            {
                throw new NetKitChatNotFoundException($"Unable to get older messages. Member {nameof(request.SaasUserId)}:{request.SaasUserId} is not found.");
            }

            var lastMessage = await UnitOfWork.MessageRepository.GetMessageWithOwnerAndForwardMessageAsync(request.MessageId);
            var lastMessageCreatedDate = lastMessage?.Created ?? request.MessageCreatedDate;

            var lastReadMessage = await UnitOfWork.MessageRepository.GetLastReadMessageAsync(member.Id, request.ChannelId);
            var messages = await UnitOfWork.MessageRepository.GetOlderMessagesWithOwnersAsync(request.ChannelId, lastMessageCreatedDate, request.PageSize);
            var results = messages.Select(message => DomainModelsMapper.MapToMessageResponse(message, lastReadMessage?.Created)).ToList();
            
            return new MessagesResult
            {
                PageSize = request.PageSize,
                Results = results
            };
        }

        public async Task<MessagesResult> GetMessagesAsync(GetMessagesRequest request)
        {
            var member = await UnitOfWork.MemberRepository.GetMemberBySaasUserIdAsync(request.SaasUserId);
            if (member == null)
            {
                throw new NetKitChatNotFoundException($"Unable to get messages. Member {nameof(request.SaasUserId)}:{request.SaasUserId} is not found.");
            }

            var lastMessage = await UnitOfWork.MessageRepository.GetMessageWithOwnerAndForwardMessageAsync(request.MessageId);
            var lastMessageCreatedDate = lastMessage?.Created ?? request.MessageCreatedDate;

            var lastReadMessage = await UnitOfWork.MessageRepository.GetLastReadMessageAsync(member.Id, request.ChannelId);
            var messages = await UnitOfWork.MessageRepository.GetMessagesWithOwnersAsync(request.ChannelId, lastMessageCreatedDate, request.PageSize);
            var results = messages.Select(message => DomainModelsMapper.MapToMessageResponse(message, lastReadMessage?.Created)).ToList();

            return new MessagesResult
            {
                PageSize = request.PageSize,
                Results = results
            };
        }

        public async Task<MessagesResult> GetLastMessagesAsync(GetLastMessagesRequest request)
        {
            var member = await UnitOfWork.MemberRepository.GetMemberBySaasUserIdAsync(request.SaasUserId);
            if (member == null)
            {
                throw new NetKitChatNotFoundException($"Unable to get last messages. Member {nameof(request.SaasUserId)}:{request.SaasUserId} is not found.");
            }

            IReadOnlyCollection<Message> messages;

            var lastReadMessage = await UnitOfWork.MessageRepository.GetLastReadMessageAsync(member.Id, request.ChannelId);
            if (lastReadMessage != null)
            {
                messages = await UnitOfWork.MessageRepository.GetLastMessagesWithOwnersAsync(request.ChannelId, lastReadMessage.Created, _messagesConfiguration.LastMessageReadCount);
            }
            else
            {
                messages = await UnitOfWork.MessageRepository.GetAllChannelMessagesWithOwnersAsync(request.ChannelId);
            }
            
            var results = messages.Select(message => DomainModelsMapper.MapToMessageResponse(message, lastReadMessage?.Created)).ToList();

            return new MessagesResult
            {
                Results = results
            };
        }

        public async Task<IReadOnlyCollection<Guid>> FindMessageIdsAsync(Guid channelId, string searchText)
        {
            return await UnitOfWork.MessageRepository.FindMessageIdsAsync(channelId, searchText);
        }

        private async Task<bool> IsAttachmentLimitExceededAsync(Guid messageId)
        {
            var attachmentsCount = await UnitOfWork.AttachmentRepository.GetMessageAttachmentsCountAsync(messageId);
            return attachmentsCount >= _messagesConfiguration.MessageAttachmentsLimit;
        }
    }
}