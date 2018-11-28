// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EnsureThat;
using Softeq.NetKit.Chat.Domain.Services.DomainServices;
using Softeq.NetKit.Chat.Domain.TransportModels.Request.Client;
using Softeq.NetKit.Chat.Domain.TransportModels.Request.Member;
using Softeq.NetKit.Chat.Domain.TransportModels.Request.Message;
using Softeq.NetKit.Chat.Domain.TransportModels.Request.MessageAttachment;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.Message;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.MessageAttachment;
using Softeq.NetKit.Chat.SignalR.Hubs.Notifications;

namespace Softeq.NetKit.Chat.SignalR.Sockets
{
    internal class MessageSocketService : IMessageSocketService
    {
        private readonly IMemberService _memberService;
        private readonly IMessageService _messageService;
        private readonly IMessageNotificationService _messageNotificationService;

        public MessageSocketService(IMemberService memberService, IMessageService messageService, IMessageNotificationService messageNotificationService)
        {
            Ensure.That(memberService).IsNotNull();
            Ensure.That(messageService).IsNotNull();
            Ensure.That(messageNotificationService).IsNotNull();

            _memberService = memberService;
            _messageService = messageService;
            _messageNotificationService = messageNotificationService;
        }

        public async Task<MessageResponse> AddMessageAsync(CreateMessageRequest request)
        {
            var message = await _messageService.CreateMessageAsync(request);

            await _messageNotificationService.OnAddMessage(request.SaasUserId, message, request.ClientConnectionId);

            await _memberService.UpdateActivityAsync(new UpdateMemberActivityRequest(request.SaasUserId, request.ClientConnectionId, null));

            return message;
        }

        public async Task DeleteMessageAsync(DeleteMessageRequest request)
        {
            var message = await _messageService.GetMessageByIdAsync(request.MessageId);

            await _messageService.DeleteMessageAsync(request);

            await _messageNotificationService.OnDeleteMessage(request.SaasUserId, message);
        }

        public async Task<MessageResponse> UpdateMessageAsync(UpdateMessageRequest request)
        {
            var updatedMessage = await _messageService.UpdateMessageAsync(request);

            await _messageNotificationService.OnUpdateMessage(request.SaasUserId, updatedMessage);

            return updatedMessage;
        }

        public async Task<AttachmentResponse> AddMessageAttachmentAsync(AddMessageAttachmentRequest request)
        {
            var attachmentResponse = await _messageService.AddMessageAttachmentAsync(request);

            var message = await _messageService.GetMessageByIdAsync(request.MessageId);

            await _messageNotificationService.OnAddMessageAttachment(request.SaasUserId, message.ChannelId);

            return attachmentResponse;
        }

        public async Task DeleteMessageAttachmentAsync(DeleteMessageAttachmentRequest request)
        {
            var message = await _messageService.GetMessageByIdAsync(request.MessageId);

            await _messageService.DeleteMessageAttachmentAsync(request);

            await _messageNotificationService.OnDeleteMessageAttachment(request.SaasUserId, message);
        }

        public async Task SetLastReadMessageAsync(SetLastReadMessageRequest request)
        {
            var member = await _memberService.GetMemberBySaasUserIdAsync(request.SaasUserId);
            var message = await _messageService.GetMessageByIdAsync(request.MessageId);

            await _messageService.SetLastReadMessageAsync(request);

            var members = new List<Guid> { member.Id, message.Sender.Id };
            await _messageNotificationService.OnChangeLastReadMessage(members, message);
        }
    }
}