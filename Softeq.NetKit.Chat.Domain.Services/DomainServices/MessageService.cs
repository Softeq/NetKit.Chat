// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using EnsureThat;
using Softeq.CloudStorage.Extension;
using Softeq.NetKit.Chat.Data.Persistent;
using Softeq.NetKit.Chat.Domain.DomainModels;
using Softeq.NetKit.Chat.Domain.Exceptions;
using Softeq.NetKit.Chat.Domain.Exceptions_OLD;
using Softeq.NetKit.Chat.Domain.Exceptions_OLD.ErrorHandling;
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
        private readonly IContentStorage _contentStorage;
        private readonly CloudStorageConfiguration _cloudStorageConfiguration;
        private readonly AttachmentConfiguration _attachmentConfiguration;

        public MessageService(IUnitOfWork unitOfWork,
            IContentStorage contentStorage,
            CloudStorageConfiguration cloudStorageConfiguration,
            AttachmentConfiguration attachmentConfiguration)
            : base(unitOfWork)
        {
            _contentStorage = contentStorage;
            _cloudStorageConfiguration = cloudStorageConfiguration;
            _attachmentConfiguration = attachmentConfiguration;
        }

        public async Task<MessageResponse> CreateMessageAsync(CreateMessageRequest request)
        {
            var channel = await UnitOfWork.ChannelRepository.GetChannelByIdAsync(request.ChannelId);
            Ensure.That(channel).WithException(x => new NetKitChatChannelNotFoundException(request.ChannelId, "Unable to create message")).IsNotNull();

            var member = await UnitOfWork.MemberRepository.GetMemberBySaasUserIdAsync(request.SaasUserId);
            Ensure.That(member).WithException(x => new NetKitChatMemberNotFoundException(request.SaasUserId, "Unable to create message")).IsNotNull();

            var message = new Message
            {
                Id = Guid.NewGuid(),
                ChannelId = request.ChannelId,
                OwnerId = member.Id,
                Owner = member,
                Body = request.Body,
                Created = DateTimeOffset.UtcNow,
                Type = request.Type,
                ImageUrl = request.ImageUrl
            };

            using (var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                await UnitOfWork.MessageRepository.AddMessageAsync(message);

                await SetLastReadMessageAsync(new SetLastReadMessageRequest(request.ChannelId, message.Id, request.SaasUserId));

                transactionScope.Complete();
            }

            return message.ToMessageResponse(null, _cloudStorageConfiguration);
        }

        public async Task DeleteMessageAsync(DeleteMessageRequest request)
        {
            var message = await UnitOfWork.MessageRepository.GetMessageByIdAsync(request.MessageId);
            Ensure.That(message).WithException(x => new NetKitChatMessageNotFoundException(request.MessageId, "Unable to delete message")).IsNotNull();

            var owner = await UnitOfWork.MemberRepository.GetMemberBySaasUserIdAsync(request.SaasUserId);
            if (owner.Id != message.OwnerId)
            {
                throw new NetKitChatMessageOwnerRequiredException(request.MessageId, "Unable to delete message");
            }

            var messageAttachments = await UnitOfWork.AttachmentRepository.GetMessageAttachmentsAsync(message.Id);

            using (var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                // Delete message attachments from database
                await UnitOfWork.AttachmentRepository.DeleteMessageAttachmentsAsync(message.Id);

                // Delete message attachments from blob storage
                foreach (var attachment in messageAttachments)
                {
                    await _contentStorage.DeleteContentAsync(attachment.FileName, _cloudStorageConfiguration.MessageAttachmentsContainer);
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
            Ensure.That(message).WithException(x => new NetKitChatMessageNotFoundException(request.MessageId, "Unable to update message")).IsNotNull();

            var owner = await UnitOfWork.MemberRepository.GetMemberBySaasUserIdAsync(request.SaasUserId);
            if (owner.Id != message.OwnerId)
            {
                throw new NetKitChatMessageOwnerRequiredException(request.MessageId, "Unable to update message");
            }

            message.Body = request.Body;
            message.Updated = DateTimeOffset.UtcNow;

            await UnitOfWork.MessageRepository.UpdateMessageAsync(message);
            return message.ToMessageResponse(null, _cloudStorageConfiguration);
        }

        public async Task<MessageResponse> GetMessageByIdAsync(Guid messageId)
        {
            var message = await UnitOfWork.MessageRepository.GetMessageByIdAsync(messageId);
            Ensure.That(message).WithException(x => new NetKitChatMessageNotFoundException(messageId, "Unable to get message by id")).IsNotNull();

            return message.ToMessageResponse(null, _cloudStorageConfiguration);
        }

        public async Task<AttachmentResponse> AddMessageAttachmentAsync(AddMessageAttachmentRequest request)
        {
            var message = await UnitOfWork.MessageRepository.GetMessageByIdAsync(request.MessageId);
            Ensure.That(message).WithException(x => new NetKitChatMessageNotFoundException(request.MessageId, "Unable to add message attachment")).IsNotNull();

            var isAttachmentLimitExceeded = await IsAttachmentLimitExceededAsync(message.Id);
            Ensure.That(isAttachmentLimitExceeded).WithException(x => new NetKitChatAttachmentLimitExceededException(_attachmentConfiguration.MessageAttachmentsLimit, "Unable to add message attachment")).IsFalse();

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

            // Save attachment in blob storage
            await _contentStorage.SaveContentAsync(attachment.FileName, request.Content, _cloudStorageConfiguration.MessageAttachmentsContainer);

            return attachment.ToAttachmentResponse();
        }

        public async Task DeleteMessageAttachmentAsync(DeleteMessageAttachmentRequest request)
        {
            var message = await UnitOfWork.MessageRepository.GetMessageByIdAsync(request.MessageId);
            Ensure.That(message).WithException(x => new NetKitChatMessageNotFoundException(request.MessageId, "Unable to delete message attachment")).IsNotNull();

            var attachment = await UnitOfWork.AttachmentRepository.GetAttachmentByIdAsync(request.AttachmentId);
            Ensure.That(attachment).WithException(x => new NetKitChatAttachmentNotFoundException(request.AttachmentId, "Unable to delete message attachment")).IsNotNull();

            if (attachment.MessageId != message.Id)
            {
                throw new ConflictException(new ErrorDto(ErrorCode.ConflictError, "Message does not contain this attachment."));
                //throw new NetKitChatInvalidOperationException(message.Id, attachment.Id, "Unable to delete message attachment");
            }

            // Delete message attachment from database
            await UnitOfWork.AttachmentRepository.DeleteAttachmentAsync(attachment.Id);

            // Delete attachment from blob storage
            await _contentStorage.DeleteContentAsync(attachment.FileName, _cloudStorageConfiguration.MessageAttachmentsContainer);
        }

        public async Task<PagedResults<MessageResponse>> GetChannelMessagesAsync(MessageRequest request)
        {
            var member = await UnitOfWork.MemberRepository.GetMemberBySaasUserIdAsync(request.SaasUserId);
            Ensure.That(member).WithException(x => new NetKitChatMemberNotFoundException(request.SaasUserId, "Unable to get channel messages")).IsNotNull();

            var lastReadMessage = await UnitOfWork.MessageRepository.GetLastReadMessageAsync(member.Id, request.ChannelId);
            var messages = await UnitOfWork.MessageRepository.GetAllChannelMessagesAsync(request.ChannelId);
            return PageUtil.CreatePagedResults(messages, request.Page, request.PageSize, x => x.ToMessageResponse(lastReadMessage, _cloudStorageConfiguration));
        }

        public async Task SetLastReadMessageAsync(SetLastReadMessageRequest request)
        {
            var member = await UnitOfWork.MemberRepository.GetMemberBySaasUserIdAsync(request.SaasUserId);
            Ensure.That(member).WithException(x => new NetKitChatMemberNotFoundException(request.SaasUserId, "Unable to set last read message")).IsNotNull();

            await UnitOfWork.ChannelMemberRepository.SetLastReadMessageAsync(member.Id, request.ChannelId, request.MessageId);
        }

        public async Task<MessagesResult> GetOlderMessagesAsync(GetMessagesRequest request)
        {
            var member = await UnitOfWork.MemberRepository.GetMemberBySaasUserIdAsync(request.SaasUserId);
            Ensure.That(member).WithException(x => new NetKitChatMemberNotFoundException(request.SaasUserId, "Unable to get older messages")).IsNotNull();

            var lastMessage = await UnitOfWork.MessageRepository.GetMessageByIdAsync(request.MessageId);
            var lastMessageCreatedDate = lastMessage?.Created ?? request.MessageCreatedDate;

            var lastReadMessage = await UnitOfWork.MessageRepository.GetLastReadMessageAsync(member.Id, request.ChannelId);
            var messages = await UnitOfWork.MessageRepository.GetOlderMessagesAsync(request.ChannelId, lastMessageCreatedDate, request.PageSize);
            var results = messages.Select(x => x.ToMessageResponse(lastReadMessage, _cloudStorageConfiguration)).ToList();

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
            Ensure.That(member).WithException(x => new NetKitChatMemberNotFoundException(request.SaasUserId, "Unable to get messages")).IsNotNull();

            var lastMessage = await UnitOfWork.MessageRepository.GetMessageByIdAsync(request.MessageId);
            var lastMessageCreatedDate = lastMessage?.Created ?? request.MessageCreatedDate;

            var lastReadMessage = await UnitOfWork.MessageRepository.GetLastReadMessageAsync(member.Id, request.ChannelId);
            var messages = await UnitOfWork.MessageRepository.GetMessagesAsync(request.ChannelId, lastMessageCreatedDate, request.PageSize);
            var results = messages.Select(x => x.ToMessageResponse(lastReadMessage, _cloudStorageConfiguration)).ToList();

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
            Ensure.That(member).WithException(x => new NetKitChatMemberNotFoundException(request.SaasUserId, "Unable to get last messages")).IsNotNull();

            var lastReadMessage = await UnitOfWork.MessageRepository.GetLastReadMessageAsync(member.Id, request.ChannelId);
            var messages = await UnitOfWork.MessageRepository.GetLastMessagesAsync(request.ChannelId, lastReadMessage?.Created);
            var results = messages.Select(x => x.ToMessageResponse(lastReadMessage, _cloudStorageConfiguration)).ToList();

            var result = new MessagesResult
            {
                Results = results
            };

            return result;
        }

        private async Task<bool> IsAttachmentLimitExceededAsync(Guid messageId)
        {
            var attachmentsCount = await UnitOfWork.AttachmentRepository.GetMessageAttachmentsCountAsync(messageId);
            return attachmentsCount >= _attachmentConfiguration.MessageAttachmentsLimit;
        }
    }
}