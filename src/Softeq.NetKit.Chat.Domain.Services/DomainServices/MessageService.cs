// Developed by Softeq Development Corporation
// http://www.softeq.com

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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        public async Task<MessageResponse> CreateSystemMessageAsync(CreateMessageRequest request)
        {
            var isChannelExists = await UnitOfWork.ChannelRepository.IsChannelExistsAsync(request.ChannelId);
            if (!isChannelExists)
            {
                throw new NetKitChatNotFoundException(NetKitChatNotFoundErrorMessages.UnableCreateMessageCauseNoChannel, $"{nameof(request.ChannelId)}:{request.ChannelId}");
            }

            var message = new Message
            {
                Id = Guid.NewGuid(),
                ChannelId = request.ChannelId,
                Body = request.Body,
                Created = _dateTimeProvider.GetUtcNow(),
                Type = request.Type
            };

            await UnitOfWork.MessageRepository.AddMessageAsync(message);
            message = await UnitOfWork.MessageRepository.GetAsync(message.Id);

            return DomainModelsMapper.MapToMessageResponse(message);
        }

        public async Task<MessageResponse> CreateMessageAsync(CreateMessageRequest request)
        {
            var member = await UnitOfWork.MemberRepository.GetMemberBySaasUserIdAsync(request.SaasUserId);
            if (member == null)
            {

                throw new NetKitChatNotFoundException(NetKitChatNotFoundErrorMessages.UnableCreateMessageNoMember, $"{nameof(request.SaasUserId)}:{request.SaasUserId}");
            }

            var isChannelOpen = await UnitOfWork.ChannelRepository.IsChannelExistsAndOpenAsync(request.ChannelId);
            if (!isChannelOpen)
            {
                throw new NetKitChatNotFoundException(NetKitChatNotFoundErrorMessages.UnableCreateMessageCauseNoChannel, $"{nameof(request.ChannelId)}:{request.ChannelId}");
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
                ImageUrl = request.ImageUrl,
                AccessibilityStatus = AccessibilityStatus.Present
            };

            await UnitOfWork.ExecuteTransactionAsync(async () =>
            {
                if (request.Type == MessageType.Forward)
                {
                    var forwardedMessage = await UnitOfWork.MessageRepository.GetMessageWithOwnerAndForwardMessageAsync(request.ForwardedMessageId);
                    if (forwardedMessage == null)
                    {
                        throw new NetKitChatNotFoundException(NetKitChatNotFoundErrorMessages.UnableCreateMessageCauseNoForwardMessasge, $"{nameof(request.ForwardedMessageId)}:{request.ForwardedMessageId}");
                    }

                    var forwardMessage = DomainModelsMapper.MapToForwardMessage(forwardedMessage);
                    forwardMessage.Id = Guid.NewGuid();
                    await UnitOfWork.ForwardMessageRepository.AddForwardMessageAsync(forwardMessage);
                    message.ForwardMessageId = forwardMessage.Id;
                }

                await UnitOfWork.MessageRepository.AddMessageAsync(message);
                await UnitOfWork.ChannelMemberRepository.SetLastReadMessageAsync(member.Id, request.ChannelId, message.Id);
            });

            message = await UnitOfWork.MessageRepository.GetMessageWithOwnerAndForwardMessageAsync(message.Id);

            return DomainModelsMapper.MapToMessageResponse(message);
        }

        public async Task ArchiveMessageAsync(ArchiveMessageRequest request)
        {
            var message = await UnitOfWork.MessageRepository.GetMessageWithOwnerAndForwardMessageAsync(request.MessageId);
            if (message == null)
            {
                throw new NetKitChatNotFoundException(NetKitChatNotFoundErrorMessages.UnableDeleteMessage, $"{nameof(request.MessageId)}:{request.MessageId}");
            }

            var member = await UnitOfWork.MemberRepository.GetMemberBySaasUserIdAsync(request.SaasUserId);
            if (member == null)
            {
                throw new NetKitChatNotFoundException(NetKitChatNotFoundErrorMessages.UnableDeleteMessageCauseNoMember, $"{nameof(request.SaasUserId)}:{request.SaasUserId}");
            }

            if (member.Id != message.OwnerId)
            {
                throw new NetKitChatAccessForbiddenException($"Unable to delete message. Message {nameof(request.MessageId)}:{request.MessageId} owner required.");
            }

            await UnitOfWork.ExecuteTransactionAsync(async () =>
            {
                //TODO: [ek] Get previous message for direct channel
                var previousMessage = await UnitOfWork.MessageRepository.GetPreviousMessageAsync(message.ChannelId, message.Created);
                //TODO: [ek]: Save last read message for direct members
                await UnitOfWork.ChannelMemberRepository.UpdateLastReadMessageAsync(message.Id, previousMessage?.Id);

                await UnitOfWork.MessageRepository.ArchiveMessageAsync(message.Id);
            });
        }

        public async Task<MessageResponse> UpdateMessageAsync(UpdateMessageRequest request)
        {
            var message = await UnitOfWork.MessageRepository.GetMessageWithOwnerAndForwardMessageAsync(request.MessageId);
            if (message == null)
            {
                throw new NetKitChatNotFoundException(NetKitChatNotFoundErrorMessages.UnableUpdateMessage, $"{nameof(request.MessageId)}:{request.MessageId}");
            }

            if (message.Type == MessageType.System)
            {
                throw new NetKitChatInvalidOperationException($"Unable to update message. Message {nameof(request.MessageId)}:{request.MessageId} update is forbidden.");
            }

            var member = await UnitOfWork.MemberRepository.GetMemberBySaasUserIdAsync(request.SaasUserId);
            if (member == null)
            {
                throw new NetKitChatNotFoundException(NetKitChatNotFoundErrorMessages.UnableUpdateMessageCauseNoMember, $"{nameof(request.SaasUserId)}:{request.SaasUserId}");
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
                throw new NetKitChatNotFoundException(NetKitChatNotFoundErrorMessages.UnableGetMessage, $"{nameof(messageId)}", $"{nameof(messageId)}:{messageId}");
            }

            return DomainModelsMapper.MapToMessageResponse(message);
        }

        public async Task<AttachmentResponse> AddMessageAttachmentAsync(AddMessageAttachmentRequest request)
        {
            var message = await UnitOfWork.MessageRepository.GetMessageWithOwnerAndForwardMessageAsync(request.MessageId);
            if (message == null)
            {
                throw new NetKitChatNotFoundException(NetKitChatNotFoundErrorMessages.UnableAddMessageAttachmentCauseNoMessage, $"{nameof(request.MessageId)}:{request.MessageId}");
            }

            if (message.Type == MessageType.System)
            {
                throw new NetKitChatInvalidOperationException($"Unable to add attachment to system message. Message {nameof(request.MessageId)}:{request.MessageId}.");
            }

            var member = await UnitOfWork.MemberRepository.GetMemberBySaasUserIdAsync(request.SaasUserId);
            if (member == null)
            {
                throw new NetKitChatNotFoundException(NetKitChatNotFoundErrorMessages.UnableAddMessageAttachmentCauseNoMember, $"{nameof(request.SaasUserId)}:{request.SaasUserId}");
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
                throw new NetKitChatNotFoundException(NetKitChatNotFoundErrorMessages.UnableDeleteMessageAttachmentCauseNoMessage, $"{nameof(request.MessageId)}:{request.MessageId}");
            }

            var member = await UnitOfWork.MemberRepository.GetMemberBySaasUserIdAsync(request.SaasUserId);
            if (member == null)
            {
                throw new NetKitChatNotFoundException(NetKitChatNotFoundErrorMessages.UnableDeleteMessageAttachmentCauseNoMember, $"{nameof(request.SaasUserId)}:{request.SaasUserId}");
            }

            if (member.Id != message.OwnerId)
            {
                throw new NetKitChatAccessForbiddenException($"Unable to delete message attachment. Message {nameof(request.MessageId)}:{request.MessageId} owner required.");
            }

            var attachment = await UnitOfWork.AttachmentRepository.GetAttachmentAsync(request.AttachmentId);
            if (attachment == null)
            {
                throw new NetKitChatNotFoundException(NetKitChatNotFoundErrorMessages.UnableDeleteMessageAttachment, $"{nameof(request.AttachmentId)}:{request.AttachmentId}");
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
