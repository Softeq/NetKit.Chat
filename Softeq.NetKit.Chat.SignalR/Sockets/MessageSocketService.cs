// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EnsureThat;
using Softeq.NetKit.Chat.Domain.Services.DomainServices;
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
        private readonly IChannelService _channelService;
        private readonly IMessageNotificationService _messageNotificationService;

        public MessageSocketService(IMemberService memberService, IMessageService messageService, IChannelService channelService, IMessageNotificationService messageNotificationService)
        {
            Ensure.That(memberService).IsNotNull();
            Ensure.That(messageService).IsNotNull();
            Ensure.That(channelService).IsNotNull();
            Ensure.That(messageNotificationService).IsNotNull();

            _memberService = memberService;
            _messageService = messageService;
            _channelService = channelService;
            _messageNotificationService = messageNotificationService;
        }

        public async Task<MessageResponse> AddMessageAsync(CreateMessageRequest request, string clientConnectionId)
        {
            var message = await _messageService.CreateMessageAsync(request);

            await _messageNotificationService.OnAddMessage(message);

            await _memberService.UpdateActivityAsync(new UpdateMemberActivityRequest(request.SaasUserId, clientConnectionId, null));

            return message;
        }

        public async Task DeleteMessageAsync(DeleteMessageRequest request)
        {
            var message = await _messageService.GetMessageByIdAsync(request.MessageId);

            await _messageService.DeleteMessageAsync(request);

            var channelSummary = await _channelService.GetChannelSummaryAsync(request.SaasUserId, message.ChannelId);

            await _messageNotificationService.OnDeleteMessage(channelSummary, message);
        }

        public async Task<MessageResponse> UpdateMessageAsync(UpdateMessageRequest request)
        {
            var updatedMessage = await _messageService.UpdateMessageAsync(request);

            await _messageNotificationService.OnUpdateMessage(updatedMessage);

            return updatedMessage;
        }

        public async Task<AttachmentResponse> AddMessageAttachmentAsync(AddMessageAttachmentRequest request)
        {
            var attachmentResponse = await _messageService.AddMessageAttachmentAsync(request);

            var message = await _messageService.GetMessageByIdAsync(request.MessageId);

            await _messageNotificationService.OnAddMessageAttachment(message.ChannelId);

            return attachmentResponse;
        }

        public async Task DeleteMessageAttachmentAsync(DeleteMessageAttachmentRequest request)
        {
            var message = await _messageService.GetMessageByIdAsync(request.MessageId);

            await _messageService.DeleteMessageAttachmentAsync(request);

            await _messageNotificationService.OnDeleteMessageAttachment(message);
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