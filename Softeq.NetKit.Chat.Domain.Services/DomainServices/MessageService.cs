// Developed by Softeq Development Corporation
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
using Softeq.NetKit.Chat.Domain.Services.Mappers;
using Softeq.NetKit.Chat.Domain.TransportModels.Request.Message;
using Softeq.NetKit.Chat.Domain.TransportModels.Request.MessageAttachment;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.Message;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.MessageAttachment;
using Softeq.QueryUtils;

namespace Softeq.NetKit.Chat.Domain.Services.DomainServices
{
    internal class MessageService : BaseService, IMessageService
    {
        private readonly AttachmentConfiguration _attachmentConfiguration;
        private readonly ICloudImageProvider _cloudImageProvider;
        private readonly ICloudAttachmentProvider _cloudAttachmentProvider;

        public MessageService(IUnitOfWork unitOfWork, AttachmentConfiguration attachmentConfiguration, ICloudImageProvider cloudImageProvider, ICloudAttachmentProvider cloudAttachmentProvider)
            : base(unitOfWork)
        {
            Ensure.That(attachmentConfiguration).IsNotNull();
            Ensure.That(cloudImageProvider).IsNotNull();
            Ensure.That(cloudAttachmentProvider).IsNotNull();

            _attachmentConfiguration = attachmentConfiguration;
            _cloudImageProvider = cloudImageProvider;
            _cloudAttachmentProvider = cloudAttachmentProvider;
        }

        public async Task<MessageResponse> CreateMessageAsync(CreateMessageRequest request)
        {
            var channel = await UnitOfWork.ChannelRepository.GetChannelByIdAsync(request.ChannelId);
            if (channel == null)
            {
                throw new NetKitChatNotFoundException($"Unable to create message. Channel {nameof(request.ChannelId)}:{request.ChannelId} not found.");
            }

            var member = await UnitOfWork.MemberRepository.GetMemberBySaasUserIdAsync(request.SaasUserId);
            if (member == null)
            {
                throw new NetKitChatNotFoundException($"Unable to create message. Member {nameof(request.SaasUserId)}:{request.SaasUserId} not found.");
            }

            var message = new Message
            {
                Id = Guid.NewGuid(),
                ChannelId = request.ChannelId,
                OwnerId = member.Id,
                Body = request.Body,
                Created = DateTimeOffset.UtcNow,
                Type = request.Type,
                ImageUrl = request.ImageUrl
            };

            using (var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                if (request.Type == MessageType.Forward)
                {
                    var forwardMessageId = Guid.NewGuid();
                    var forwardMessage = (await UnitOfWork.MessageRepository.GetMessageByIdAsync(request.ForwardedMessageId)).ToForwardMessage(forwardMessageId);
                    await UnitOfWork.ForwardMessageRepository.AddForwardMessageAsync(forwardMessage);
                    message.ForwardMessageId = forwardMessage.Id;
                }

                await UnitOfWork.MessageRepository.AddMessageAsync(message);
                await UnitOfWork.ChannelMemberRepository.SetLastReadMessageAsync(member.Id, request.ChannelId, message.Id);

                transactionScope.Complete();
            }

            message = await UnitOfWork.MessageRepository.GetMessageByIdAsync(message.Id);

            return message.ToMessageResponse(null, _cloudImageProvider);
        }

        public async Task DeleteMessageAsync(string saasUserId, Guid messageId)
        {
            var message = await UnitOfWork.MessageRepository.GetMessageByIdAsync(messageId);
            if (message == null)
            {
                throw new NetKitChatNotFoundException($"Unable to delete message. Message {nameof(messageId)}:{messageId} not found.");
            }

            var member = await UnitOfWork.MemberRepository.GetMemberBySaasUserIdAsync(saasUserId);
            if (member == null)
            {
                throw new NetKitChatNotFoundException($"Unable to delete message. Member {nameof(saasUserId)}:{saasUserId} not found.");
            }

            if (member.Id != message.OwnerId)
            {
                throw new NetKitChatAccessForbiddenException($"Unable to delete message. Message {nameof(messageId)}:{messageId} owner required.");
            }

            var messageAttachments = await UnitOfWork.AttachmentRepository.GetMessageAttachmentsAsync(message.Id);

            using (var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                if (message.Type == MessageType.Forward && message.ForwardMessageId.HasValue)
                {
                    await UnitOfWork.ForwardMessageRepository.DeleteForwardMessageAsync(message.ForwardMessageId.Value);
                }

                // Delete message attachments from database
                await UnitOfWork.AttachmentRepository.DeleteMessageAttachmentsAsync(message.Id);

                // Delete message attachments from cloud
                foreach (var attachment in messageAttachments)
                {
                    await _cloudAttachmentProvider.DeleteMessageAttachmentAsync(attachment.FileName);
                }

                var previousMessage = await UnitOfWork.MessageRepository.GetPreviousMessageAsync(message);
                if (previousMessage != null)
                {
                    await UnitOfWork.ChannelMemberRepository.UpdateLastReadMessageAsync(message.Id, previousMessage.Id);
                }

                await UnitOfWork.MessageRepository.DeleteMessageAsync(message.Id);

                transactionScope.Complete();
            }
        }

        public async Task<MessageResponse> UpdateMessageAsync(UpdateMessageRequest request)
        {
            var message = await UnitOfWork.MessageRepository.GetMessageByIdAsync(request.MessageId);
            if (message == null)
            {
                throw new NetKitChatNotFoundException($"Unable to update message. Message {nameof(request.MessageId)}:{request.MessageId} not found.");
            }

            var member = await UnitOfWork.MemberRepository.GetMemberBySaasUserIdAsync(request.SaasUserId);
            if (member == null)
            {
                throw new NetKitChatNotFoundException($"Unable to update message. Member {nameof(request.SaasUserId)}:{request.SaasUserId} not found.");
            }

            if (member.Id != message.OwnerId)
            {
                throw new NetKitChatAccessForbiddenException($"Unable to update message. Message {nameof(request.MessageId)}:{request.MessageId} owner required.");
            }

            message.Body = request.Body;
            message.Updated = DateTimeOffset.UtcNow;

            await UnitOfWork.MessageRepository.UpdateMessageAsync(message);
            return message.ToMessageResponse(null, _cloudImageProvider);
        }

        public async Task<MessageResponse> GetMessageByIdAsync(Guid messageId)
        {
            var message = await UnitOfWork.MessageRepository.GetMessageByIdAsync(messageId);
            if (message == null)
            {
                throw new NetKitChatNotFoundException($"Unable to get message by {nameof(messageId)}. Message {nameof(messageId)}:{messageId} not found.");
            }

            return message.ToMessageResponse(null, _cloudImageProvider);
        }

        public async Task<AttachmentResponse> AddMessageAttachmentAsync(AddMessageAttachmentRequest request)
        {
            var message = await UnitOfWork.MessageRepository.GetMessageByIdAsync(request.MessageId);
            if (message == null)
            {
                throw new NetKitChatNotFoundException($"Unable to add message attachment. Message {nameof(request.MessageId)}:{request.MessageId} not found.");
            }

            var member = await UnitOfWork.MemberRepository.GetMemberBySaasUserIdAsync(request.SaasUserId);
            if (member == null)
            {
                throw new NetKitChatNotFoundException($"Unable to add message attachment. Member {nameof(request.SaasUserId)}:{request.SaasUserId} not found.");
            }

            if (member.Id != message.OwnerId)
            {
                throw new NetKitChatAccessForbiddenException($"Unable to add message attachment. Message {nameof(request.MessageId)}:{request.MessageId} owner required.");
            }

            var isAttachmentLimitExceeded = await IsAttachmentLimitExceededAsync(message.Id);
            if (isAttachmentLimitExceeded)
            {
                throw new NetKitChatInvalidOperationException($"Unable to add message attachment. Attachment limit {_attachmentConfiguration.MessageAttachmentsLimit} exceeded.");
            }

            var attachment = new Attachment
            {
                Id = Guid.NewGuid(),
                ContentType = request.ContentType,
                Created = DateTimeOffset.UtcNow,
                FileName = Guid.NewGuid() + "." + request.Extension,
                MessageId = request.MessageId,
                Size = request.Size
            };

            // Save attachment in database
            await UnitOfWork.AttachmentRepository.AddAttachmentAsync(attachment);

            // Save attachment in cloud
            await _cloudAttachmentProvider.SaveAttachmentAsync(attachment.FileName, request.Content);

            return attachment.ToAttachmentResponse();
        }

        public async Task DeleteMessageAttachmentAsync(DeleteMessageAttachmentRequest request)
        {
            var message = await UnitOfWork.MessageRepository.GetMessageByIdAsync(request.MessageId);
            if (message == null)
            {
                throw new NetKitChatNotFoundException($"Unable to delete message attachment. Message {nameof(request.MessageId)}:{request.MessageId} not found.");
            }

            var member = await UnitOfWork.MemberRepository.GetMemberBySaasUserIdAsync(request.SaasUserId);
            if (member == null)
            {
                throw new NetKitChatNotFoundException($"Unable to delete message attachment. Member {nameof(request.SaasUserId)}:{request.SaasUserId} not found.");
            }

            if (member.Id != message.OwnerId)
            {
                throw new NetKitChatAccessForbiddenException($"Unable to delete message attachment. Message {nameof(request.MessageId)}:{request.MessageId} owner required.");
            }

            var attachment = await UnitOfWork.AttachmentRepository.GetAttachmentByIdAsync(request.AttachmentId);
            if (attachment == null)
            {
                throw new NetKitChatNotFoundException($"Unable to delete message attachment. Attachment {nameof(request.AttachmentId)}:{request.AttachmentId} not found.");
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

        public async Task<PagedResults<MessageResponse>> GetChannelMessagesAsync(MessageRequest request)
        {
            var member = await UnitOfWork.MemberRepository.GetMemberBySaasUserIdAsync(request.SaasUserId);
            if (member == null)
            {
                throw new NetKitChatNotFoundException($"Unable to get channel messages. Member {nameof(request.SaasUserId)}:{request.SaasUserId} not found.");
            }

            var lastReadMessage = await UnitOfWork.MessageRepository.GetLastReadMessageAsync(member.Id, request.ChannelId);

            var messages = await UnitOfWork.MessageRepository.GetAllChannelMessagesAsync(request.ChannelId);

            return PageUtil.CreatePagedResults(messages, request.Page, request.PageSize, x => x.ToMessageResponse(lastReadMessage, _cloudImageProvider));
        }

        public async Task SetLastReadMessageAsync(SetLastReadMessageRequest request)
        {
            var member = await UnitOfWork.MemberRepository.GetMemberBySaasUserIdAsync(request.SaasUserId);
            if (member == null)
            {
                throw new NetKitChatNotFoundException($"Unable to set last read message. Member {nameof(request.SaasUserId)}:{request.SaasUserId} not found.");
            }

            await UnitOfWork.ChannelMemberRepository.SetLastReadMessageAsync(member.Id, request.ChannelId, request.MessageId);
        }

        public async Task<MessagesResult> GetOlderMessagesAsync(GetMessagesRequest request)
        {
            var member = await UnitOfWork.MemberRepository.GetMemberBySaasUserIdAsync(request.SaasUserId);
            if (member == null)
            {
                throw new NetKitChatNotFoundException($"Unable to get older messages. Member {nameof(request.SaasUserId)}:{request.SaasUserId} not found.");
            }

            var lastMessage = await UnitOfWork.MessageRepository.GetMessageByIdAsync(request.MessageId);
            var lastMessageCreatedDate = lastMessage?.Created ?? request.MessageCreatedDate;

            var lastReadMessage = await UnitOfWork.MessageRepository.GetLastReadMessageAsync(member.Id, request.ChannelId);
            var messages = await UnitOfWork.MessageRepository.GetOlderMessagesAsync(request.ChannelId, lastMessageCreatedDate, request.PageSize);
            var results = messages.Select(x => x.ToMessageResponse(lastReadMessage, _cloudImageProvider)).ToList();

            var result = new MessagesResult
            {
                PageSize = request.PageSize,
                Results = results
            };

            return result;
        }

        public async Task<MessagesResult> GetMessagesAsync(GetMessagesRequest request)
        {
            var member = await UnitOfWork.MemberRepository.GetMemberBySaasUserIdAsync(request.SaasUserId);
            if (member == null)
            {
                throw new NetKitChatNotFoundException($"Unable to get messages. Member {nameof(request.SaasUserId)}:{request.SaasUserId} not found.");
            }

            var lastMessage = await UnitOfWork.MessageRepository.GetMessageByIdAsync(request.MessageId);
            var lastMessageCreatedDate = lastMessage?.Created ?? request.MessageCreatedDate;

            var lastReadMessage = await UnitOfWork.MessageRepository.GetLastReadMessageAsync(member.Id, request.ChannelId);
            var messages = await UnitOfWork.MessageRepository.GetMessagesAsync(request.ChannelId, lastMessageCreatedDate, request.PageSize);
            var results = messages.Select(x => x.ToMessageResponse(lastReadMessage, _cloudImageProvider)).ToList();

            var result = new MessagesResult
            {
                PageSize = request.PageSize,
                Results = results
            };

            return result;
        }

        public async Task<MessagesResult> GetLastMessagesAsync(GetLastMessagesRequest request)
        {
            var member = await UnitOfWork.MemberRepository.GetMemberBySaasUserIdAsync(request.SaasUserId);
            if (member == null)
            {
                throw new NetKitChatNotFoundException($"Unable to get last messages. Member {nameof(request.SaasUserId)}:{request.SaasUserId} not found.");
            }

            var lastReadMessage = await UnitOfWork.MessageRepository.GetLastReadMessageAsync(member.Id, request.ChannelId);
            var messages = await UnitOfWork.MessageRepository.GetLastMessagesAsync(request.ChannelId, lastReadMessage?.Created);
            var results = messages.Select(x => x.ToMessageResponse(lastReadMessage, _cloudImageProvider)).ToList();

            var result = new MessagesResult
            {
                Results = results
            };

            return result;
        }

        public async Task<IReadOnlyCollection<Guid>> FindMessageIdsAsync(Guid channelId, string searchText)
        {
            return await UnitOfWork.MessageRepository.FindMessageIdsAsync(channelId, searchText);
        }

        private async Task<bool> IsAttachmentLimitExceededAsync(Guid messageId)
        {
            var attachmentsCount = await UnitOfWork.AttachmentRepository.GetMessageAttachmentsCountAsync(messageId);
            return attachmentsCount >= _attachmentConfiguration.MessageAttachmentsLimit;
        }
    }
}