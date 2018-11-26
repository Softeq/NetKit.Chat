// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Resources;
using Softeq.NetKit.Chat.Domain.Services.DomainServices;
using Softeq.NetKit.Chat.Domain.TransportModels.Request.Client;
using Softeq.NetKit.Chat.Domain.TransportModels.Request.Message;
using Softeq.NetKit.Chat.Domain.TransportModels.Request.MessageAttachment;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.Member;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.Message;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.MessageAttachment;
using Softeq.NetKit.Chat.SignalR.Hubs.Notifications;

namespace Softeq.NetKit.Chat.SignalR.Sockets
{
    internal class MessageSocketService : IMessageSocketService
    {
        private readonly IChannelService _channelService;
        private readonly IMemberService _memberService;
        private readonly IMessageService _messageService;
        private readonly IMessageNotificationService _messageNotificationService;

        public MessageSocketService(
            IChannelService channelService,
            IMemberService memberService,
            IMessageService messageService,
            IMessageNotificationService messageNotificationService)
        {
            _channelService = channelService;
            _memberService = memberService;
            _messageService = messageService;
            _messageNotificationService = messageNotificationService;
        }

        public async Task<MessageResponse> AddMessageAsync(CreateMessageRequest request)
        {
            var channel = await _channelService.GetChannelByIdAsync(request.ChannelId);
            var member = await _memberService.GetMemberBySaasUserIdAsync(request.SaasUserId);

            if (string.IsNullOrEmpty(request.Body))
            {
                throw new Exception(string.Format(LanguageResources.Msg_MessageRequired, channel.Name));
            }

            var message = await _messageService.CreateMessageAsync(request);

            await _messageNotificationService.OnAddMessage(member, message, request.ClientConnectionId);

            await _memberService.UpdateActivityAsync(new AddClientRequest
            {
                SaasUserId = member.SaasUserId,
                UserName = member.UserName,
                ConnectionId = request.ClientConnectionId,
                UserAgent = null
            });

            return message;
        }

        public async Task DeleteMessageAsync(DeleteMessageRequest request)
        {
            var member = await _memberService.GetMemberBySaasUserIdAsync(request.SaasUserId);
            var message = await _messageService.GetMessageByIdAsync(request.MessageId);
            if (message.Sender.Id != member.Id)
            {
                throw new Exception(string.Format(LanguageResources.Msg_AccessPermission, message.Id));
            }
            await _messageService.DeleteMessageAsync(request);

            await _messageNotificationService.OnDeleteMessage(member, message);
        }

        public async Task<MessageResponse> UpdateMessageAsync(UpdateMessageRequest request)
        {
            var member = await _memberService.GetMemberBySaasUserIdAsync(request.SaasUserId);
            var message = await _messageService.GetMessageByIdAsync(request.MessageId);
            if (message.Sender.Id != member.Id)
            {
                throw new Exception(string.Format(LanguageResources.Msg_AccessPermission, message.Id));
            }
            if (string.IsNullOrEmpty(request.Body))
            {
                throw new Exception(LanguageResources.Msg_MessageRequired);
            }

            var updatedMessage = await _messageService.UpdateMessageAsync(request);

            await _messageNotificationService.OnUpdateMessage(member, updatedMessage);

            return updatedMessage;
        }

        public async Task<AttachmentResponse> AddMessageAttachmentAsync(AddMessageAttachmentRequest request)
        {
            var member = await _memberService.GetMemberBySaasUserIdAsync(request.SaasUserId);
            var message = await _messageService.GetMessageByIdAsync(request.MessageId);
            if (message.Sender.Id != member.Id)
            {
                throw new Exception(string.Format(LanguageResources.Msg_AccessPermission, message.Id));
            }

            var attachmentResponse = await _messageService.AddMessageAttachmentAsync(request);

            await _messageNotificationService.OnAddMessageAttachment(member, message);

            return attachmentResponse;
        }

        public async Task DeleteMessageAttachmentAsync(DeleteMessageAttachmentRequest request)
        {
            var member = await _memberService.GetMemberBySaasUserIdAsync(request.SaasUserId);
            var message = await _messageService.GetMessageByIdAsync(request.MessageId);
            if (message.Sender.Id != member.Id)
            {
                throw new Exception(string.Format(LanguageResources.Msg_AccessPermission, message.Id));
            }

            await _messageService.DeleteMessageAttachmentAsync(request);

            await _messageNotificationService.OnDeleteMessageAttachment(member, message);
        }

        public async Task SetLastReadMessageAsync(SetLastReadMessageRequest request)
        {
            var member = await _memberService.GetMemberBySaasUserIdAsync(request.SaasUserId);
            var message = await _messageService.GetMessageByIdAsync(request.MessageId);

            await _messageService.SetLastReadMessageAsync(request);

            var messageOwner = await _memberService.GetMemberByIdAsync(message.Sender.Id);

            var members = new List<MemberSummary> { member, messageOwner };
            await _messageNotificationService.OnChangeLastReadMessage(members, message);
        }
    }
}