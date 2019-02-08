// Developed by Softeq Development Corporation
// http://www.softeq.com

using EnsureThat;
using Softeq.NetKit.Chat.Application.Services.Services.SystemMessages;
using Softeq.NetKit.Chat.Domain.Services.DomainServices;
using Softeq.NetKit.Chat.Domain.TransportModels.Request.Member;
using Softeq.NetKit.Chat.Domain.TransportModels.Request.SystemMessage;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.Channel;
using Softeq.NetKit.Chat.Notifications;
using Softeq.NetKit.Chat.Notifications.Services;
using Softeq.NetKit.Chat.SignalR.Hubs.Notifications;
using System;
using System.Threading.Tasks;
using ChannelRequest = Softeq.NetKit.Chat.Domain.TransportModels.Request.Channel.ChannelRequest;
using CreateChannelRequest = Softeq.NetKit.Chat.Domain.TransportModels.Request.Channel.CreateChannelRequest;
using MuteChannelRequest = Softeq.NetKit.Chat.Domain.TransportModels.Request.Channel.MuteChannelRequest;
using UpdateChannelRequest = Softeq.NetKit.Chat.Domain.TransportModels.Request.Channel.UpdateChannelRequest;

namespace Softeq.NetKit.Chat.SignalR.Sockets
{
    internal class ChannelSocketService : IChannelSocketService
    {
        private readonly IChannelService _channelService;
        private readonly IMemberService _memberService;
        private readonly IChannelNotificationService _channelNotificationService;
        private readonly IPushNotificationService _pushNotificationService;
        private readonly IMessageService _messageService;
        private readonly IChatSystemMessagesService _chatSystemMessagesService;

        public ChannelSocketService(
            IChannelService channelService,
            IMemberService memberService,
            IChannelNotificationService channelNotificationService,
            IPushNotificationService pushNotificationService,
            IMessageService messageService,
            IChatSystemMessagesService chatSystemMessagesService)
        {
            Ensure.That(channelService).IsNotNull();
            Ensure.That(memberService).IsNotNull();
            Ensure.That(channelNotificationService).IsNotNull();
            Ensure.That(messageService).IsNotNull();
            Ensure.That(pushNotificationService).IsNotNull();
            Ensure.That(chatSystemMessagesService).IsNotNull();

            _channelNotificationService = channelNotificationService;
            _pushNotificationService = pushNotificationService;
            _messageService = messageService;
            _chatSystemMessagesService = chatSystemMessagesService;
            _channelService = channelService;
            _memberService = memberService;
        }

        public async Task<ChannelSummaryResponse> CreateChannelAsync(CreateChannelRequest request)
        {
            var channel = await _channelService.CreateChannelAsync(request);
            var member = await _memberService.GetMemberBySaasUserIdAsync(request.SaasUserId);

            // subscribe creator on channel
            await _pushNotificationService.SubscribeUserOnTagAsync(member.SaasUserId, PushNotificationsTagTemplates.GetChatChannelTag(channel.Id.ToString()));

            // subscribe invited members
            foreach (var allowedMemberId in request.AllowedMembers)
            {
                var chatMember = await _memberService.GetMemberByIdAsync(Guid.Parse(allowedMemberId));

                if (chatMember != null)
                {
                    await _pushNotificationService.SubscribeUserOnTagAsync(chatMember.SaasUserId, PushNotificationsTagTemplates.GetChatChannelTag(channel.Id.ToString()));
                }
            }

            await _channelNotificationService.OnAddChannel(channel);
            //todo filter creator connection id on join channel
            await _channelNotificationService.OnJoinChannel(member, channel);

            return channel;
        }

        public async Task<ChannelSummaryResponse> UpdateChannelAsync(UpdateChannelRequest request)
        {
            await _channelService.UpdateChannelAsync(request);

            var channelSummary = await _channelService.GetChannelSummaryAsync(request.SaasUserId, request.ChannelId);

            await _channelNotificationService.OnUpdateChannel(channelSummary);

            var systemMessageRequest = await GetSystemMessageRequestAsync("ChannelUpdated", request.SaasUserId, request.ChannelId);
            var systemMessageResponse = await _messageService.CreateSystemMessageAsync(systemMessageRequest);

            await _channelNotificationService.OnAddSystemMessage(systemMessageResponse);

            return channelSummary;
        }

        public async Task CloseChannelAsync(ChannelRequest request)
        {
            await _channelService.CloseChannelAsync(request.SaasUserId, request.ChannelId);

            var channelSummary = await _channelService.GetChannelSummaryAsync(request.SaasUserId, request.ChannelId);

            await _channelNotificationService.OnCloseChannel(channelSummary);

            var channelMembers = await _memberService.GetChannelMembersAsync(request.ChannelId);

            foreach (var member in channelMembers)
            {
                // unsubscribe user from channel
                await _pushNotificationService.UnsubscribeUserFromTagAsync(member.SaasUserId, PushNotificationsTagTemplates.GetChatChannelTag(request.ChannelId.ToString()));
            }

            // TODO [az]: do we need this notification?
            await _channelNotificationService.OnUpdateChannel(channelSummary);

            var systemMessageRequest = await GetSystemMessageRequestAsync("ChannelClosed", request.SaasUserId, request.ChannelId);
            var systemMessageResponse = await _messageService.CreateSystemMessageAsync(systemMessageRequest);

            await _channelNotificationService.OnAddSystemMessage(systemMessageResponse);
        }

        public async Task JoinToChannelAsync(ChannelRequest request)
        {
            // Locate the room, does NOT have to be open
            await _channelService.JoinToChannelAsync(request.SaasUserId, request.ChannelId);

            var channel = await _channelService.GetChannelSummaryAsync(request.SaasUserId, request.ChannelId);

            var member = await _memberService.GetMemberBySaasUserIdAsync(request.SaasUserId);

            await _pushNotificationService.SubscribeUserOnTagAsync(member.SaasUserId, PushNotificationsTagTemplates.GetChatChannelTag(request.ChannelId.ToString()));

            await _channelNotificationService.OnJoinChannel(member, channel);

            var systemMessageRequest = await GetSystemMessageRequestAsync("ChannelJoined", request.SaasUserId, request.ChannelId);
            var systemMessageResponse = await _messageService.CreateSystemMessageAsync(systemMessageRequest);

            await _channelNotificationService.OnAddSystemMessage(systemMessageResponse);
        }

        public async Task<ChannelResponse> InviteMemberAsync(InviteMemberRequest request)
        {
            var inviteMemberResponse = await _memberService.InviteMemberAsync(request.MemberId, request.ChannelId);

            var invitedMember = await _memberService.GetMemberByIdAsync(request.MemberId);

            var channel = await _channelService.GetChannelSummaryAsync(request.SaasUserId, request.ChannelId);

            await _channelNotificationService.OnJoinChannel(invitedMember, channel);

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

            var systemMessageRequest = await GetSystemMessageRequestAsync("ChannelLeft", request.SaasUserId, request.ChannelId);
            var systemMessageResponse = await _messageService.CreateSystemMessageAsync(systemMessageRequest);

            await _channelNotificationService.OnAddSystemMessage(systemMessageResponse);
        }

        public async Task DeleteMemberFromChannelAsync(DeleteMemberRequest request)
        {
            var memberToDelete = await _memberService.GetMemberByIdAsync(request.MemberId);

            await _channelService.DeleteMemberFromChannelAsync(request.SaasUserId, request.ChannelId, memberToDelete.Id);

            await _pushNotificationService.UnsubscribeUserFromTagAsync(memberToDelete.SaasUserId, PushNotificationsTagTemplates.GetChatChannelTag(request.ChannelId.ToString()));

            await _channelNotificationService.OnDeletedFromChannel(memberToDelete, request.ChannelId);
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

        private async Task<CreateSystemMessageRequest> GetSystemMessageRequestAsync(string key, string saasUserId, Guid channelId)
        {
            var member = await _memberService.GetMemberBySaasUserIdAsync(saasUserId);
            var channel = await _channelService.GetChannelByIdAsync(channelId);
            var body = _chatSystemMessagesService.FormatSystemMessage(key, member.UserName, channel.Name);

            return new CreateSystemMessageRequest
            {
                MemberId = member.Id,
                ChannelId = channel.Id,
                Body = body
            };
        }
    }
}