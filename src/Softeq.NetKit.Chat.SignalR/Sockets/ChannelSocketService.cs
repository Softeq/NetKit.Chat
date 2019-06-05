// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Threading.Tasks;
using EnsureThat;
using Softeq.NetKit.Chat.Domain.DomainModels;
using Softeq.NetKit.Chat.Domain.Exceptions;
using Softeq.NetKit.Chat.Domain.Services.Configuration;
using Softeq.NetKit.Chat.Domain.Services.DomainServices;
using Softeq.NetKit.Chat.Domain.TransportModels.Request.Channel;
using Softeq.NetKit.Chat.Domain.TransportModels.Request.Member;
using Softeq.NetKit.Chat.Domain.TransportModels.Request.Message;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.Channel;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.Message;
using Softeq.NetKit.Chat.Domain.TransportModels.Visitors.Localization;
using Softeq.NetKit.Chat.Notifications;
using Softeq.NetKit.Chat.Notifications.Services;
using Softeq.NetKit.Chat.SignalR.Hubs.Notifications;
using ChannelRequest = Softeq.NetKit.Chat.Domain.TransportModels.Request.Channel.ChannelRequest;
using CreateChannelRequest = Softeq.NetKit.Chat.Domain.TransportModels.Request.Channel.CreateChannelRequest;
using UpdateChannelRequest = Softeq.NetKit.Chat.Domain.TransportModels.Request.Channel.UpdateChannelRequest;
using MuteChannelRequest = Softeq.NetKit.Chat.Domain.TransportModels.Request.Channel.MuteChannelRequest;

namespace Softeq.NetKit.Chat.SignalR.Sockets
{
    internal class ChannelSocketService : IChannelSocketService
    {
        private readonly IChannelService _channelService;
        private readonly IMemberService _memberService;
        private readonly IChannelNotificationService _channelNotificationService;
        private readonly IPushNotificationService _pushNotificationService;
        private readonly IMessageNotificationService _messageNotificationService;
        private readonly IMessageService _messageService;
        private readonly SystemMessagesConfiguration _systemMessagesConfiguration;

        public ChannelSocketService(
            IChannelService channelService,
            IMemberService memberService,
            IChannelNotificationService channelNotificationService,
            IPushNotificationService pushNotificationService,
            IMessageNotificationService messageNotificationService,
            IMessageService messageService,
            SystemMessagesConfiguration systemMessagesConfiguration)
        {
            Ensure.That(channelService).IsNotNull();
            Ensure.That(memberService).IsNotNull();
            Ensure.That(channelNotificationService).IsNotNull();
            Ensure.That(messageService).IsNotNull();

            _channelNotificationService = channelNotificationService;
            _pushNotificationService = pushNotificationService;
            _messageNotificationService = messageNotificationService;
            _messageService = messageService;
            _systemMessagesConfiguration = systemMessagesConfiguration;
            _channelService = channelService;
            _memberService = memberService;
        }

        public async Task<ChannelSummaryResponse> CreateChannelAsync(CreateChannelRequest request)
        {
            var channel = await _channelService.CreateChannelAsync(request);
            var member = await _memberService.GetMemberBySaasUserIdAsync(request.SaasUserId);

            // subscribe creator on channel
            await _pushNotificationService.SubscribeUserOnTagAsync(member.SaasUserId, PushNotificationsTagTemplates.GetChatChannelTag(channel.Id.ToString()));

            if (request.AllowedMembers != null)
            {
                // subscribe invited members
                foreach (var allowedMemberId in request.AllowedMembers)
                {
                    var chatMember = await _memberService.GetMemberByIdAsync(Guid.Parse(allowedMemberId));
                    if (chatMember == null)
                    {
                        throw new NetKitChatNotFoundException($"Specified chat member {allowedMemberId} is not found.");
                    }

                    await _pushNotificationService.SubscribeUserOnTagAsync(chatMember.SaasUserId, PushNotificationsTagTemplates.GetChatChannelTag(channel.Id.ToString()));
                }
            }

            await _channelNotificationService.OnAddChannel(channel);
            //todo filter creator connection id on join channel
            await _channelNotificationService.OnJoinChannel(channel);

            return channel;
        }

        public async Task<ChannelSummaryResponse> CreateDirectChannelAsync(CreateDirectChannelRequest request)
        {
            var channel = await _channelService.CreateDirectChannelAsync(request);

            // subscribe creator on channel
            await _pushNotificationService.SubscribeUserOnTagAsync(request.SaasUserId, PushNotificationsTagTemplates.GetChatChannelTag(channel.Id.ToString()));
            var member = await _memberService.GetMemberByIdAsync(request.MemberId);
            // subscribe member on channel
            await _pushNotificationService.SubscribeUserOnTagAsync(member.SaasUserId, PushNotificationsTagTemplates.GetChatChannelTag(channel.Id.ToString()));

            await _channelNotificationService.OnAddChannel(channel);
            //todo filter creator connection id on join channel
            await _channelNotificationService.OnJoinChannel(channel);

            return channel;
        }

        public async Task<ChannelSummaryResponse> UpdateChannelAsync(UpdateChannelRequest request)
        {
            var updatingChannel = await _channelService.GetChannelSummaryAsync(request.SaasUserId, request.ChannelId);

            await _channelService.UpdateChannelAsync(request);

            var updatedChannel = await _channelService.GetChannelSummaryAsync(request.SaasUserId, request.ChannelId);

            await _channelNotificationService.OnUpdateChannel(updatedChannel);

            if (updatingChannel.Name != request.Name)
            {
                await SendSystemMessageAsync(request.SaasUserId, request.ChannelId, new ChannelNameChangedLocalizationVisitor(updatedChannel), RecipientType.AllChannelConnections, _systemMessagesConfiguration.ChannelNameChanged);
            }
            else if (updatingChannel.PhotoUrl != request.PhotoUrl)
            {
                await SendSystemMessageAsync(request.SaasUserId, request.ChannelId, new ChannelIconChangedLocalizationVisitor(updatedChannel), RecipientType.AllChannelConnections, _systemMessagesConfiguration.ChannelIconChanged);
            }

            return updatedChannel;
        }

        public async Task CloseChannelAsync(ChannelRequest request)
        {
            await _channelService.CloseChannelAsync(request.SaasUserId, request.ChannelId);

            await _channelNotificationService.OnCloseChannel(request.ChannelId);

            var channelMembers = await _memberService.GetChannelMembersAsync(request.ChannelId);

            foreach (var member in channelMembers)
            {
                // unsubscribe user from channel
                await _pushNotificationService.UnsubscribeUserFromTagAsync(member.SaasUserId, PushNotificationsTagTemplates.GetChatChannelTag(request.ChannelId.ToString()));
            }
        }

        public async Task<ChannelResponse> InviteMemberAsync(InviteMemberRequest request)
        {
            var inviteMemberResponse = await _memberService.InviteMemberAsync(request.MemberId, request.ChannelId);

            var invitedMember = await _memberService.GetMemberByIdAsync(request.MemberId);

            var channel = await _channelService.GetChannelSummaryAsync(request.SaasUserId, request.ChannelId);

            if (invitedMember != null && channel != null)
            {
                await _pushNotificationService.SubscribeUserOnTagAsync(invitedMember.SaasUserId, PushNotificationsTagTemplates.GetChatChannelTag(channel.Id.ToString()));
            }

            await _channelNotificationService.OnJoinChannel(channel);
            await SendSystemMessageAsync(request.SaasUserId, request.ChannelId, new MemberJoinedLocalizationVisitor(invitedMember), RecipientType.AllExceptCallerConnectionId);

            return inviteMemberResponse;
        }

        public async Task<ChannelResponse> InviteMultipleMembersAsync(InviteMultipleMembersRequest request)
        {
            var response = default(ChannelResponse);

            foreach (var memberId in request.InvitedMembersIds)
            {
                var inviteMemberRequest = new InviteMemberRequest(request.SaasUserId, request.ChannelId, memberId);
                response = await InviteMemberAsync(inviteMemberRequest);
            }

            return response;
        }

        public async Task LeaveChannelAsync(ChannelRequest request)
        {
            var member = await _memberService.GetMemberBySaasUserIdAsync(request.SaasUserId);

            await _channelService.LeaveFromChannelAsync(request.SaasUserId, request.ChannelId);

            await _pushNotificationService.UnsubscribeUserFromTagAsync(member.SaasUserId, PushNotificationsTagTemplates.GetChatChannelTag(request.ChannelId.ToString()));

            await _channelNotificationService.OnLeaveChannel(member, request.ChannelId);

            await SendSystemMessageAsync(request.SaasUserId, request.ChannelId, new MemberLeftLocalizationVisitor(member), RecipientType.AllExceptMemberConnections, _systemMessagesConfiguration.MemberLeft, member.UserName);
        }

        public async Task DeleteMemberFromChannelAsync(DeleteMemberRequest request)
        {
            var memberToDelete = await _memberService.GetMemberByIdAsync(request.MemberId);

            await _channelService.DeleteMemberFromChannelAsync(request.SaasUserId, request.ChannelId, memberToDelete.Id);
            await _pushNotificationService.UnsubscribeUserFromTagAsync(memberToDelete.SaasUserId, PushNotificationsTagTemplates.GetChatChannelTag(request.ChannelId.ToString()));
            await _channelNotificationService.OnDeletedFromChannel(memberToDelete, request.ChannelId);

            await SendSystemMessageAsync(request.SaasUserId, request.ChannelId, new MemberDeletedLocalizationVisitor(memberToDelete), RecipientType.AllExceptMemberConnections, _systemMessagesConfiguration.MemberDeleted, memberToDelete.UserName);
        }

        public async Task MuteChannelAsync(MuteChannelRequest request)
        {
            if (request.IsMuted)
            {
                await _pushNotificationService.UnsubscribeUserFromTagAsync(request.SaasUserId, PushNotificationsTagTemplates.GetChatChannelTag(request.ChannelId.ToString()));
            }
            else
            {
                await _pushNotificationService.SubscribeUserOnTagAsync(request.SaasUserId, PushNotificationsTagTemplates.GetChatChannelTag(request.ChannelId.ToString()));
            }

            await _channelService.MuteChannelAsync(request.SaasUserId, request.ChannelId, request.IsMuted);
        }

        private async Task SendSystemMessageAsync(
                        string saasUserId,
                        Guid channelId,
                        ILocalizationVisitor<MessageResponse> localizationVisitor,
                        RecipientType recipientType,
                        string localizationFallback = "",
                        params object[] localizationFallbackArgs)
        {
            var systemMessage = await _messageService.CreateSystemMessageAsync(
                new CreateMessageRequest(saasUserId, channelId, MessageType.System, string.Format(localizationFallback, localizationFallbackArgs)));
            systemMessage.Sender = await _memberService.GetMemberBySaasUserIdAsync(saasUserId);
            systemMessage.Accept(localizationVisitor);
            // TODO we should pass callerConnectionId into OnAddMessage to except it in case we are using RecipientType.AllExceptCallerConnectionId
            await _messageNotificationService.OnAddMessage(systemMessage, recipientType);
        }
    }
}